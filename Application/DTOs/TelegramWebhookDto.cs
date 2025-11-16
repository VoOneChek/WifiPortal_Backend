using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TelegramWebhookDto
    {
        [JsonPropertyName("update_id")]
        public long UpdateId { get; set; }

        [JsonPropertyName("message")]
        public TelegramMessageDto? Message { get; set; }
    }

    public class TelegramMessageDto
    {
        [JsonPropertyName("message_id")]
        public long MessageId { get; set; }

        [JsonPropertyName("from")]
        public TelegramUserDto From { get; set; } = null!;

        [JsonPropertyName("chat")]
        public TelegramChatDto Chat { get; set; } = null!;

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("contact")]
        public TelegramContactDto? Contact { get; set; }
    }

    public class TelegramUserDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = null!;

        [JsonPropertyName("username")]
        public string? Username { get; set; }
    }

    public class TelegramChatDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = null!;

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;
    }

    public class TelegramContactDto
    {
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; } = null!;

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = null!;
    }

}
