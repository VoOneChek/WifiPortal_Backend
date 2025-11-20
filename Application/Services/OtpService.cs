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
using System.Text.RegularExpressions;
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

        public OtpService(IMemoryCache cache, ILogger<OtpService> logger, IUserRepository user, IJwtService jwtService, IMapper mapper)
        {
            _cache = cache;
            _logger = logger;
            _user = user;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<Result<string>> CreateOtpCodeAsync(string phoneNumber)
        {
            _logger.LogInformation("Создание OTP кода для номера {PhoneNumber}", phoneNumber);

            if (string.IsNullOrWhiteSpace(phoneNumber) ||
                !Regex.IsMatch(phoneNumber, @"^\+7\d{10}$"))
            {
                return Result<string>.Fail("Укажите корректный номер телефона в формате +7XXXXXXXXXX");
            }

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

        public async Task<Result<AuthResponseDto>> VerifyOtpAsync(string phoneNumber, string code)
        {
            var cacheKey = ValidateVerifyData(phoneNumber, code);

            if (cacheKey != $"otp:{phoneNumber}")
                return Result<AuthResponseDto>.Fail(cacheKey);

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

        public Task<Result> VerifyPhoneUpdateOtpAsync(string phoneNumber, string code)
        {
            var cacheKey = ValidateVerifyData(phoneNumber, code);

            if (cacheKey != $"otp:{phoneNumber}")
                return Task.FromResult(Result.Fail(cacheKey));

            _cache.Remove(cacheKey);

            _logger.LogInformation("OTP код для обновления номера {PhoneNumber} успешно проверен", phoneNumber);
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

        private string ValidateVerifyData (string phoneNumber, string code)
        {
            _logger.LogInformation("Проверка OTP кода для номера {PhoneNumber}", phoneNumber);

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.LogWarning("Номер телефона не указан");
                return "Номер телефона обязателен";
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("OTP код не указан");
                return "OTP код обязателен";
            }

            var cacheKey = $"otp:{phoneNumber}";
            if (!_cache.TryGetValue(cacheKey, out string? storedCode))
            {
                return "Код истек или не найден";
            }

            if (storedCode != code)
            {
                _logger.LogWarning("Неверный OTP код для номера {PhoneNumber}", phoneNumber);
                return "Неверный код";
            }
            return cacheKey;
        }
    }
}
