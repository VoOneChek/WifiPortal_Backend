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
        private readonly string _login;
        private readonly string _password;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SmscService> _logger;
        private const string SenderName = "WIFI-PORTAL";

        public SmscService(HttpClient httpClient, ILogger<SmscService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _login = Environment.GetEnvironmentVariable("SMS_LOGIN")
                     ?? throw new Exception("Переменная окружения SMS_LOGIN не установлена");

            _password = Environment.GetEnvironmentVariable("SMS_PASSWORD")
                        ?? throw new Exception("Переменная окружения SMS_PASSWORD не установлена");
        }

        public async Task<Result> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var cleanPhone = phoneNumber.Replace("+", "").Replace(" ", "");

                var finalMessage = $"Ваш код: {message}. Никому не сообщайте его. {SenderName}";

                var url =
                    $"https://smsc.ru/sys/send.php?" +
                    $"login={_login}&psw={_password}" +
                    $"&phones={cleanPhone}" +
                    $"&sender={SenderName}" +
                    $"&mes={Uri.EscapeDataString(finalMessage)}" +
                    $"&fmt=3&charset=utf-8";

                var response = await _httpClient.GetAsync(url);
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
