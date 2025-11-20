using Application.Common.TelegramOptions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices
{
    public class TelegramWebhookService
    {
        private readonly HttpClient _httpClient;
        private readonly TelegramOptions _options;

        public TelegramWebhookService(HttpClient httpClient, IOptions<TelegramOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<bool> SetupWebhookAsync(string tunnelUrl)
        {
            var setWebhookUrl = $"https://api.telegram.org/bot{_options.BotToken}/setWebhook?url={tunnelUrl}/api/telegram/webhook";

            var response = await _httpClient.GetAsync(setWebhookUrl);
            return response.IsSuccessStatusCode;
        }
    }
}
