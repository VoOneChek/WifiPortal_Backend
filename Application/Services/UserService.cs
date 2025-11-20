using Application.Common.ResultInfo;
using Application.DTOs.AuthSessionDto;
using Application.DTOs.UserDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IAuthSessionRepository _sessions;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IOtpService _otpService;

        public UserService(IUserRepository users, IAuthSessionRepository sessions, ILogger<UserService> logger, IMapper mapper, IOtpService otpService)
        {
            _users = users;
            _sessions = sessions;
            _logger = logger;
            _mapper = mapper;
            _otpService = otpService;
        }

        public async Task<Result<IEnumerable<ReadUserDto>>> GetAllAsync()
        {
            _logger.LogInformation("Запрос всех пользователей");

            var users = await _users.GetAllAsync();

            if (users == null || !users.Any())
            {
                _logger.LogWarning("Пользователи не найдены");
                return Result<IEnumerable<ReadUserDto>>.Fail("Пользователи не найдены");
            }

            var dtos = _mapper.Map<IEnumerable<ReadUserDto>>(users);

            _logger.LogInformation("Найдено {Count} пользователей", dtos.Count());
            return Result<IEnumerable<ReadUserDto>>.Ok(dtos);
        }

        public async Task<Result<ReadUserDto>> GetByIdAsync(int id)
        {
            _logger.LogInformation("Запрос пользователя по ID {Id}", id);

            var user = await _users.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("Пользователь с ID {Id} не найден", id);
                return Result<ReadUserDto>.Fail("Пользователь не найден");
            }

            var dto = _mapper.Map<ReadUserDto>(user);

            _logger.LogInformation("Пользователь с ID {Id} успешно найден", id);
            return Result<ReadUserDto>.Ok(dto);
        }

        public async Task<Result<ReadUserDto>> GetByPhoneAsync(string phone)
        {
            _logger.LogInformation("Запрос пользователя по номеру телефона {Phone}", phone);

            if (string.IsNullOrWhiteSpace(phone))
            {
                _logger.LogWarning("Номер телефона не указан");
                return Result<ReadUserDto>.Fail("Номер телефона обязателен");
            }

            var user = await _users.GetByPhoneAsync(phone);

            if (user == null)
            {
                _logger.LogWarning("Пользователь с номером телефона {Phone} не найден", phone);
                return Result<ReadUserDto>.Fail("Пользователь не найден");
            }

            var dto = _mapper.Map<ReadUserDto>(user);

            _logger.LogInformation("Пользователь с номером телефона {Phone} успешно найден", phone);
            return Result<ReadUserDto>.Ok(dto);
        }

        public async Task<Result> CreateAsync(CreateUserDto createUserDto)
        {
            _logger.LogInformation("Создание пользователя с номером телефона {PhoneNumber}", createUserDto.PhoneNumber);

            if (createUserDto == null)
            {
                _logger.LogWarning("Переданный пользователь равен null");
                return Result.Fail("Пользователь не может быть null");
            }

            if (string.IsNullOrWhiteSpace(createUserDto.PhoneNumber))
            {
                _logger.LogWarning("Номер телефона не указан");
                return Result.Fail("Номер телефона обязателен");
            }

            var existingUser = await _users.GetByPhoneAsync(createUserDto.PhoneNumber);
            if (existingUser != null)
            {
                _logger.LogWarning("Пользователь с номером телефона {PhoneNumber} уже существует", createUserDto.PhoneNumber);
                return Result.Fail("Пользователь с таким номером телефона уже существует");
            }

            var user = _mapper.Map<User>(createUserDto);
            await _users.AddAsync(user);
            await _users.SaveChangesAsync();

            _logger.LogInformation("Пользователь с номером телефона {PhoneNumber} успешно создан", createUserDto.PhoneNumber);
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Обновление пользователя с ID {Id}", id);

            if (updateUserDto == null)
            {
                _logger.LogWarning("Переданные данные для обновления равны null");
                return Result.Fail("Данные для обновления не могут быть null");
            }

            var user = await _users.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Пользователь с ID {Id} не найден", id);
                return Result.Fail("Пользователь не найден");
            }

            _mapper.Map(updateUserDto, user);
            _users.Update(user);
            await _users.SaveChangesAsync();

            _logger.LogInformation("Пользователь с ID {Id} успешно обновлен", id);
            return Result.Ok();
        }

        public async Task<Result> UpdatePhoneNumberAsync(int id, UpdatePhoneNumberDto updatePhoneNumberDto)
        {
            _logger.LogInformation("Обновление номера телефона для пользователя с ID {Id}", id);

            if (updatePhoneNumberDto == null)
            {
                _logger.LogWarning("Переданные данные для обновления номера телефона равны null");
                return Result.Fail("Данные для обновления номера телефона не могут быть null");
            }

            if (string.IsNullOrWhiteSpace(updatePhoneNumberDto.PhoneNumber) ||
                !Regex.IsMatch(updatePhoneNumberDto.PhoneNumber, @"^\+7\d{10}$"))
            {
                return Result.Fail("Укажите корректный номер телефона в формате +7XXXXXXXXXX");
            }

            if (string.IsNullOrWhiteSpace(updatePhoneNumberDto.OtpCode))
            {
                _logger.LogWarning("Код подтверждения не указан");
                return Result.Fail("Код подтверждения обязателен для изменения номера телефона");
            }

            var user = await _users.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Пользователь с ID {Id} не найден", id);
                return Result.Fail("Пользователь не найден");
            }

            var existingUser = await _users.GetByPhoneAsync(updatePhoneNumberDto.PhoneNumber);
            if (existingUser != null && existingUser.Id != id)
            {
                _logger.LogWarning("Номер телефона {PhoneNumber} уже используется другим пользователем", updatePhoneNumberDto.PhoneNumber);
                return Result.Fail("Этот номер телефона уже используется другим пользователем");
            }

            var otpVerificationResult = await _otpService.VerifyPhoneUpdateOtpAsync(updatePhoneNumberDto.PhoneNumber, updatePhoneNumberDto.OtpCode);
            if (!otpVerificationResult.Success)
            {
                _logger.LogWarning(otpVerificationResult.Error);
                return Result.Fail(otpVerificationResult.Error!);
            }

            var oldPhoneNumber = user.PhoneNumber;
            user.PhoneNumber = updatePhoneNumberDto.PhoneNumber;

            _users.Update(user);
            await _users.SaveChangesAsync();

            _logger.LogInformation("Номер телефона пользователя с ID {Id} успешно изменен с {OldPhone} на {NewPhone}",
                id, oldPhoneNumber, user.PhoneNumber);

            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            _logger.LogInformation("Удаление пользователя с ID {Id}", id);

            var user = await _users.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Пользователь с ID {Id} не найден", id);
                return Result.Fail("Пользователь не найден");
            }

            // Деактивируем все сессии пользователя перед удалением
            var sessions = await _sessions.GetByUserAsync(id);
            foreach (var session in sessions)
            {
                session.IsActive = false;
                _sessions.Update(session);
            }

            _users.Remove(user);
            await _users.SaveChangesAsync();

            _logger.LogInformation("Пользователь с ID {Id} успешно удален", id);
            return Result.Ok();
        }

        public async Task<Result<UserDetailDto>> GetUserWithSessionsAsync(int id)
        {
            _logger.LogInformation("Запрос пользователя с ID {Id} вместе с сессиями", id);

            var user = await _users.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Пользователь с ID {Id} не найден", id);
                return Result<UserDetailDto>.Fail("Пользователь не найден");
            }

            var sessions = await _sessions.GetByUserAsync(id);
            var dto = _mapper.Map<UserDetailDto>(user);
            dto.Sessions = _mapper.Map<List<ReadAuthSessionDto>>(sessions);

            _logger.LogInformation("Пользователь с ID {Id} и {SessionCount} сессиями успешно найден", id, dto.Sessions.Count);
            return Result<UserDetailDto>.Ok(dto);
        }

        public async Task<Result> LinkTelegramAsync(string phoneNumber, long chatId, string? username)
        {
            _logger.LogInformation("Привязка Telegram для номера {PhoneNumber}", phoneNumber);

            var user = await _users.GetByPhoneAsync(phoneNumber);
            if (user == null)
            {
                _logger.LogWarning("Пользователь с номером {PhoneNumber} не найден", phoneNumber);
                user = _mapper.Map<User>(new CreateUserDto { 
                    PhoneNumber = phoneNumber,
                    TelegramChatId = chatId,
                    FullName = username
                });
                await _users.AddAsync(user);
                await _users.SaveChangesAsync();

                _logger.LogInformation("Telegram привязан для нового пользователя {UserFullName}", user.FullName);
                return Result.Ok();
            }

            user.TelegramChatId = chatId;
            if (user.FullName == string.Empty)
                user.FullName = username;

            _users.Update(user);
            await _users.SaveChangesAsync();

            _logger.LogInformation("Telegram привязан для пользователя {UserId}", user.Id);
            return Result.Ok();
        }
    }
}
