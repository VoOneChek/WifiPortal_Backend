using Application.Common.ResultInfo;
using Application.DTOs.OtpCodeDto;
using Application.DTOs.UserDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<OtpService> _logger;
        private readonly IUserRepository _user;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ISmsService _smsService;
        private readonly ITelegramService _telegramService;

        public OtpService(IMemoryCache cache, ILogger<OtpService> logger, IUserRepository user, IJwtService jwtService, IMapper mapper, 
            ISmsService smsService, ITelegramService telegramService)
        {
            _cache = cache;
            _logger = logger;
            _user = user;
            _jwtService = jwtService;
            _mapper = mapper;
            _smsService = smsService;
            _telegramService = telegramService;
        }

        private async Task<Result<string>> CreateOtpCodeAsync(string phoneNumber)
        {
            _logger.LogInformation("Создание OTP кода для номера {PhoneNumber}", phoneNumber);

            if (string.IsNullOrWhiteSpace(phoneNumber))
                return Result<string>.Fail("Номер телефона обязателен");

            if (await HasActiveOtpAsync(phoneNumber))
            {
                _logger.LogWarning("Для номера {PhoneNumber} уже есть активный OTP код", phoneNumber);
                return Result<string>.Fail("Код уже отправлен. Попробуйте позже");
            }

            var code = GenerateRandomCode();
            var cacheKey = $"otp:{phoneNumber}";

            _cache.Set(cacheKey, code, TimeSpan.FromMinutes(5));
            return Result<string>.Ok(code);
        }

        public async Task<Result> CreateAndSendSmsAsync(string phoneNumber)
        {
            var codeResult = await CreateOtpCodeAsync(phoneNumber);
            if (!codeResult.Success)
                return Result.Fail(codeResult.Error!);

            var smsResult = await _smsService.SendSmsAsync(phoneNumber, $"Ваш код: {codeResult.Data}");
            if (!smsResult.Success)
            {
                _cache.Remove($"otp:{phoneNumber}");
                return Result.Fail(smsResult.Error ?? "Ошибка отправки SMS");
            }

            _logger.LogInformation("SMS с кодом отправлено на {PhoneNumber}", phoneNumber);
            return Result.Ok();
        }

        public async Task<Result> CreateAndSendTelegramAsync(string phoneNumber)
        {
            var codeResult = await CreateOtpCodeAsync(phoneNumber);
            if (!codeResult.Success)
                return Result.Fail(codeResult.Error!);

            var telegramResult = await _telegramService.SendTelegramAsync(phoneNumber, codeResult.Data!);
            if (!telegramResult.Success)
            {
                _cache.Remove($"otp:{phoneNumber}");
                return Result.Fail(telegramResult.Error ?? "Ошибка отправки Telegram");
            }

            _logger.LogInformation("Код отправлен в Telegram для {PhoneNumber}", phoneNumber);
            return Result.Ok();
        }

        public async Task<Result<AuthResponseDto>> VerifyOtpAsync(string phoneNumber, string code)
        {
            _logger.LogInformation("Проверка OTP кода для номера {PhoneNumber}", phoneNumber);

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.LogWarning("Номер телефона не указан");
                return Result<AuthResponseDto>.Fail("Номер телефона обязателен");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("OTP код не указан");
                return Result<AuthResponseDto>.Fail("OTP код обязателен");
            }

            var cacheKey = $"otp:{phoneNumber}";
            if (!_cache.TryGetValue(cacheKey, out string? storedCode))
            {
                return Result<AuthResponseDto>.Fail("Код истек или не найден");
            }

            if (storedCode != code)
            {
                _logger.LogWarning("Неверный OTP код для номера {PhoneNumber}", phoneNumber);
                return Result<AuthResponseDto>.Fail("Неверный код");
            }

            _cache.Remove(cacheKey);

            var user = await _user.GetByPhoneAsync(phoneNumber);
            
            if (user == null)
            {
                user = new User
                {
                    PhoneNumber = phoneNumber,
                    Role = UserRole.Guest
                };

                await _user.AddAsync(user);
                await _user.SaveChangesAsync();
            }

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation("OTP код для номера {PhoneNumber} успешно проверен", phoneNumber);
            return Result<AuthResponseDto>.Ok(new AuthResponseDto
            {
                Token = token,
                User = _mapper.Map<ReadUserDto>(user)
            });
        }

        public Task<Result> ResendOtpAsync(string phoneNumber)
        {
            _logger.LogInformation("Повторная отправка OTP для номера {PhoneNumber}", phoneNumber);

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.LogWarning("Номер телефона не указан");
                return Task.FromResult(Result.Fail("Номер телефона обязателен"));
            }

            var code = GenerateRandomCode();
            var cacheKey = $"otp:{phoneNumber}";

            _cache.Set(cacheKey, code, TimeSpan.FromMinutes(5));

            _logger.LogInformation("Новый OTP код для номера {PhoneNumber} создан", phoneNumber);
            return Task.FromResult(Result.Ok());
        }

        public Task<Result> InvalidateOtpAsync(string phoneNumber)
        {
            _logger.LogInformation("Инвалидация OTP кода для номера {PhoneNumber}", phoneNumber);

            var cacheKey = $"otp:{phoneNumber}";
            _cache.Remove(cacheKey);

            _logger.LogInformation("OTP код для номера {PhoneNumber} инвалидирован", phoneNumber);
            return Task.FromResult(Result.Ok());
        }

        public Task<bool> HasActiveOtpAsync(string phoneNumber)
        {
            var cacheKey = $"otp:{phoneNumber}";
            return Task.FromResult(_cache.TryGetValue(cacheKey, out _));
        }

        private static string GenerateRandomCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
