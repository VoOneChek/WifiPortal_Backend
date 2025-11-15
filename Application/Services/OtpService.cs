using Application.Common.ResultInfo;
using Application.DTOs.OtpCodeDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _cache;
        private readonly ILogger<OtpService> _logger;

        public OtpService(IDistributedCache cache, ILogger<OtpService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<Result> CreateAsync(CreateOtpCodeDto createOtpDto)
        {
            _logger.LogInformation("Создание OTP кода для номера {PhoneNumber}", createOtpDto.PhoneNumber);

            if (createOtpDto == null)
            {
                _logger.LogWarning("Переданный OTP код равен null");
                return Result.Fail("OTP код не может быть null");
            }

            if (string.IsNullOrWhiteSpace(createOtpDto.PhoneNumber))
            {
                _logger.LogWarning("Номер телефона не указан");
                return Result.Fail("Номер телефона обязателен");
            }

            var code = GenerateRandomCode();
            var cacheKey = $"otp:{createOtpDto.PhoneNumber}";

            // Сохраняем в Redis с expiration (5 минут)
            await _cache.SetStringAsync(cacheKey, code, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            // TODO: Отправка SMS с кодом
            _logger.LogInformation("OTP код для номера {PhoneNumber} создан и отправлен", createOtpDto.PhoneNumber);

            return Result.Ok();
        }

        public async Task<Result> VerifyOtpAsync(string phoneNumber, string code)
        {
            _logger.LogInformation("Проверка OTP кода для номера {PhoneNumber}", phoneNumber);

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.LogWarning("Номер телефона не указан");
                return Result.Fail("Номер телефона обязателен");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("OTP код не указан");
                return Result.Fail("OTP код обязателен");
            }

            var cacheKey = $"otp:{phoneNumber}";
            var storedCode = await _cache.GetStringAsync(cacheKey);

            if (storedCode == null)
            {
                _logger.LogWarning("OTP код для номера {PhoneNumber} не найден или истек", phoneNumber);
                return Result.Fail("Код истек или не найден");
            }

            if (storedCode != code)
            {
                _logger.LogWarning("Неверный OTP код для номера {PhoneNumber}", phoneNumber);
                return Result.Fail("Неверный код");
            }

            await _cache.RemoveAsync(cacheKey);

            _logger.LogInformation("OTP код для номера {PhoneNumber} успешно проверен", phoneNumber);
            return Result.Ok();
        }

        public async Task<Result> ResendOtpAsync(string phoneNumber)
        {
            _logger.LogInformation("Повторная отправка OTP для номера {PhoneNumber}", phoneNumber);

            var cacheKey = $"otp:{phoneNumber}";
            await _cache.RemoveAsync(cacheKey);

            return await CreateAsync(new CreateOtpCodeDto { PhoneNumber = phoneNumber });
        }

        public async Task<Result> InvalidateOtpAsync(string phoneNumber)
        {
            _logger.LogInformation("Инвалидация OTP кода для номера {PhoneNumber}", phoneNumber);

            var cacheKey = $"otp:{phoneNumber}";
            await _cache.RemoveAsync(cacheKey);

            _logger.LogInformation("OTP код для номера {PhoneNumber} инвалидирован", phoneNumber);
            return Result.Ok();
        }

        public async Task<Result<OtpStatusDto>> GetOtpStatusAsync(string phoneNumber)
        {
            var cacheKey = $"otp:{phoneNumber}";
            var exists = await _cache.GetStringAsync(cacheKey) != null;

            return Result<OtpStatusDto>.Ok(new OtpStatusDto
            {
                PhoneNumber = phoneNumber,
                HasActiveOtp = exists,
                Message = exists ? "Активный OTP код существует" : "Активный OTP код отсутствует"
            });
        }

        private static string GenerateRandomCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
