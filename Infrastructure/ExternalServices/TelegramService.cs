using Application.Common.ResultInfo;
using Application.DTOs.OtpCodeDto;
using Application.DTOs.UserDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices
{
    public class TelegramService : ITelegramService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TelegramService> _logger;
        private readonly string _botToken;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public TelegramService(HttpClient httpClient, ILogger<TelegramService> logger, IConfiguration configuration, 
            IUserRepository userRepository, IJwtService jwtService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _botToken = configuration["Telegram:BotToken"]
                ?? throw new ArgumentNullException("Telegram:BotToken not configured");
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result> SendMessageAsync(long chatId, string message)
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            var requestData = new { chat_id = chatId, text = message, parse_mode = "Markdown" };

            var response = await _httpClient.PostAsJsonAsync(url, requestData);
            return response.IsSuccessStatusCode ? Result.Ok() : Result.Fail("Ошибка отправки");
        }

        public async Task<Result> SendPhoneRequestAsync(long chatId)
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            var requestData = new
            {
                chat_id = chatId,
                text = "📱 *Привяжите ваш номер телефона для входа через Telegram:*",
                reply_markup = new
                {
                    keyboard = new[] { new[] { new { text = "📱 Привязать номер", request_contact = true } } },
                    resize_keyboard = true
                },
                parse_mode = "Markdown"
            };

            var response = await _httpClient.PostAsJsonAsync(url, requestData);
            return response.IsSuccessStatusCode ? Result.Ok() : Result.Fail("Ошибка отправки кнопки");
        }

        public async Task<Result> SendTelegramAsync(string phoneNumber, string code)
        {
            var user = await _userRepository.GetByPhoneAsync(phoneNumber);
            if (user?.TelegramChatId == null)
                return Result.Fail("Telegram не привязан");

            return await SendMessageAsync(user.TelegramChatId.Value, $"🔐 *Код для входа:* `{code}`");
        }
    }
}
