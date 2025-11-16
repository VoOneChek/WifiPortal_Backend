using Application.Common.MappingProfile;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.ExternalServices;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/wifi-portal-.txt",
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 10 * 1024 * 1024,
        retainedFileCountLimit: 5,
        shared: true)
    .CreateLogger();

builder.Host.UseSerilog();

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new Exception("JWT_SECRET not set in environment variables!");
}
var key = Encoding.UTF8.GetBytes(jwtSecret);

// Настройка аутентификации JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                Environment.GetEnvironmentVariable("JWT_SECRET_KEY")!)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WifiPortalContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthSessionRepository, AuthSessionRepository>();
builder.Services.AddScoped<IAuthMethodRepository, AuthMethodRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IAuthSessionService, AuthSessionService>();
builder.Services.AddScoped<IAuthMethodService, AuthMethodService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITelegramService, TelegramService>();
builder.Services.AddScoped<ISmsService, SmscService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
    // другие профили
});

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ITelegramService, TelegramService>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "WifiPortal API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token in format: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var telegramToken = builder.Configuration["Telegram:BotToken"];
var tunnelUrl = "https://833o0l-95-26-99-46.ru.tuna.am";

using var httpClient = new HttpClient();
var setWebhookUrl = $"https://api.telegram.org/bot{telegramToken}/setWebhook?url={tunnelUrl}/api/telegram/webhook";
var response = await httpClient.GetAsync(setWebhookUrl);
var content = await response.Content.ReadAsStringAsync();

app.Run();
