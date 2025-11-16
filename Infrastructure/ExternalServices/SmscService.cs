using Application.Common.ResultInfo;
using Application.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices
{
    public class SmscService : ISmsService
    {
        private readonly string _login = "free";
        private readonly string _password = "free";
        private readonly HttpClient _httpClient;
        private readonly ILogger<SmscService> _logger;

        public SmscService(HttpClient httpClient, ILogger<SmscService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Result> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var cleanPhone = phoneNumber.Replace("+", "").Replace(" ", "");

                var url = $"https://smsc.ru/sys/send.php?login={_login}&psw={_password}" +
                         $"&phones={cleanPhone}&mes={Uri.EscapeDataString(message)}&fmt=3&charset=utf-8";

                var response = await _httpClient.GetAsync(url);

                // Принудительно указываем кодировку UTF-8
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Ответ от SMSC: {Response}", content);

                if (response.IsSuccessStatusCode && !content.Contains("ERROR"))
                {
                    _logger.LogInformation("SMS отправлено на номер {PhoneNumber}", phoneNumber);
                    return Result.Ok();
                }

                _logger.LogError("Ошибка отправки SMS: {Error}", content);
                return Result.Fail($"Ошибка отправки SMS: {content}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке SMS на номер {PhoneNumber}", phoneNumber);
                return Result.Fail($"Ошибка отправки SMS: {ex.Message}");
            }
        }
    }
}
