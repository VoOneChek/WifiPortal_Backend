using Application.Common.ResultInfo;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OtpSenderService: IOtpSenderService
    {
        private readonly ILogger<OtpService> _logger;
        private readonly ISmsService _smsService;
        private readonly ITelegramService _telegramService;
        private readonly IOtpService _otpService;

        public OtpSenderService(ILogger<OtpService> logger, IOtpService otpService,
            ISmsService smsService, ITelegramService telegramService)
        {
            _logger = logger;
            _otpService = otpService;
            _smsService = smsService;
            _telegramService = telegramService;
        }

        public async Task<Result> SendSmsAsync(string phoneNumber)
        {
            var codeResult = await _otpService.CreateOtpCodeAsync(phoneNumber);
            if (!codeResult.Success)
                return Result.Fail(codeResult.Error!);

            var smsResult = await _smsService.SendSmsAsync(phoneNumber, codeResult.Data!);
            if (!smsResult.Success)
            {
                await _otpService.InvalidateOtpAsync(phoneNumber);
                return Result.Fail(smsResult.Error ?? "Ошибка отправки SMS");
            }

            _logger.LogInformation("SMS с кодом отправлено на {PhoneNumber}", phoneNumber);
            return Result.Ok();
        }

        public async Task<Result> ResendSmsAsync(string phoneNumber)
        {
            _logger.LogInformation("Повторная отправка OTP для номера {PhoneNumber}", phoneNumber);

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.LogWarning("Номер телефона не указан");
                return Result.Fail("Номер телефона обязателен");
            }

            await _otpService.InvalidateOtpAsync(phoneNumber);
            var code = await _otpService.CreateOtpCodeAsync(phoneNumber);

            var smsResult = await _smsService.SendSmsAsync(phoneNumber, code.Data!);
            if (!smsResult.Success)
            {
                await _otpService.InvalidateOtpAsync(phoneNumber);
                return Result.Fail(smsResult.Error ?? "Ошибка отправки SMS");
            }

            _logger.LogInformation("Новый OTP код отправлен через SMS для {PhoneNumber}", phoneNumber);
            return Result.Ok();
        }

        public async Task<Result> SendTelegramAsync(string phoneNumber)
        {
            var codeResult = await _otpService.CreateOtpCodeAsync(phoneNumber);
            if (!codeResult.Success)
                return Result.Fail(codeResult.Error!);

            var telegramResult = await _telegramService.SendTelegramAsync(phoneNumber, codeResult.Data!);
            if (!telegramResult.Success)
            {
                await _otpService.InvalidateOtpAsync(phoneNumber);
                return Result.Fail(telegramResult.Error ?? "Ошибка отправки Telegram");
            }

            _logger.LogInformation("Код отправлен в Telegram для {PhoneNumber}", phoneNumber);
            return Result.Ok();
        }

        public async Task<Result> ResendTelegramAsync(string phoneNumber)
        {
            _logger.LogInformation("Повторная отправка OTP для номера {PhoneNumber}", phoneNumber);

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.LogWarning("Номер телефона не указан");
                return Result.Fail("Номер телефона обязателен");
            }

            await _otpService.InvalidateOtpAsync(phoneNumber);
            var code = await _otpService.CreateOtpCodeAsync(phoneNumber);

            var telegramResult = await _telegramService.SendTelegramAsync(phoneNumber, code.Data!);
            if (!telegramResult.Success)
            {
                await _otpService.InvalidateOtpAsync(phoneNumber);
                return Result.Fail(telegramResult.Error ?? "Ошибка отправки Telegram");
            }

            _logger.LogInformation("Новый OTP код отправлен через Telegram для {PhoneNumber}", phoneNumber);
            return Result.Ok();
        }
    }
}
