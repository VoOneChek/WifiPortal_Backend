using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class WifiPortalContextFactory : IDesignTimeDbContextFactory<WifiPortalContext>
{
    public WifiPortalContext CreateDbContext(string[] args)
    {
        // Получаем конфигурацию из appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // путь к проекту с appsettings.json
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Берём строку подключения
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<WifiPortalContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new WifiPortalContext(optionsBuilder.Options);
    }
}
