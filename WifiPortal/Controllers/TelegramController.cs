using Application.DTOs;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WifiPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelegramController : ControllerBase
{
    private readonly ITelegramService _telegramService;
    private readonly IUserService _userService;
    private readonly ILogger<TelegramController> _logger;

    public TelegramController(ITelegramService telegramService, IUserService userService, ILogger<TelegramController> logger)
    {
        _telegramService = telegramService;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook([FromBody] TelegramWebhookDto webhookDto)
    {
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync();
        _logger.LogInformation("Raw body: {Body}", rawBody);

        _logger.LogInformation("Webhook DTO: {Json}", JsonSerializer.Serialize(webhookDto));

        try
        {
            if (webhookDto.Message?.Text == "/start")
            {
                await _telegramService.SendPhoneRequestAsync(webhookDto.Message.Chat.Id);
            }
            else if (webhookDto.Message?.Contact != null)
            {
                await HandleContact(webhookDto.Message);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обработки webhook");
            return Ok();
        }
    }

    private async Task HandleContact(TelegramMessageDto message)
    {
        var phoneNumber = message.Contact?.PhoneNumber;
        if (string.IsNullOrEmpty(phoneNumber))
        {
            _logger.LogWarning("Номер телефона отсутствует");
            return;
        }
        var chatId = message.Chat.Id;
        var username = message.Chat.Username;

        var result = await _userService.LinkTelegramAsync(phoneNumber, chatId, username);

        if (result.Success)
        {
            await _telegramService.SendMessageAsync(chatId,
                "✅ *Аккаунт привязан!* Теперь вы можете входить через Telegram.");
        }
        else
        {
            await _telegramService.SendMessageAsync(chatId,
                "❌ *Ошибка:* Не удалось привязать аккаунт. Убедитесь что номер совпадает с зарегистрированным.");
        }
    }
}
