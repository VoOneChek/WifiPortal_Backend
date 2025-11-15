using Application.Common.ResultInfo;
using Application.DTOs.AuthSessionDto;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthSessionService : IAuthSessionService
    {
        private readonly IAuthSessionRepository _sessions;
        private readonly ILogger<AuthSessionService> _logger;
        private readonly IMapper _mapper;

        public AuthSessionService(IAuthSessionRepository sessions, ILogger<AuthSessionService> logger, IMapper mapper)
        {
            _sessions = sessions;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<ReadAuthSessionDto>>> GetAllAsync()
        {
            _logger.LogInformation("Запрос всех сессий аутентификации");

            var sessions = await _sessions.GetAllAsync();

            if (sessions == null || !sessions.Any())
            {
                _logger.LogWarning("Сессии аутентификации не найдены");
                return Result<IEnumerable<ReadAuthSessionDto>>.Fail("Сессии аутентификации не найдены");
            }

            var dtos = _mapper.Map<IEnumerable<ReadAuthSessionDto>>(sessions);

            _logger.LogInformation("Найдено {Count} сессий аутентификации", dtos.Count());
            return Result<IEnumerable<ReadAuthSessionDto>>.Ok(dtos);
        }

        public async Task<Result<ReadAuthSessionDto>> GetByIdAsync(int id)
        {
            _logger.LogInformation("Запрос сессии аутентификации по ID {Id}", id);

            var session = await _sessions.GetByIdAsync(id);

            if (session == null)
            {
                _logger.LogWarning("Сессия аутентификации с ID {Id} не найдена", id);
                return Result<ReadAuthSessionDto>.Fail("Сессия аутентификации не найдена");
            }

            var dto = _mapper.Map<ReadAuthSessionDto>(session);

            _logger.LogInformation("Сессия аутентификации с ID {Id} успешно найдена", id);
            return Result<ReadAuthSessionDto>.Ok(dto);
        }

        public async Task<Result> CreateAsync(CreateAuthSessionDto createSessionDto)
        {
            _logger.LogInformation("Создание сессии аутентификации для пользователя {UserId}", createSessionDto.UserId);

            if (createSessionDto == null)
            {
                _logger.LogWarning("Переданная сессия аутентификации равна null");
                return Result.Fail("Сессия аутентификации не может быть null");
            }

            if (createSessionDto.UserId <= 0)
            {
                _logger.LogWarning("Некорректный ID пользователя: {UserId}", createSessionDto.UserId);
                return Result.Fail("Некорректный ID пользователя");
            }

            if (string.IsNullOrWhiteSpace(createSessionDto.MacAddress))
            {
                _logger.LogWarning("MAC-адрес не указан");
                return Result.Fail("MAC-адрес обязателен");
            }

            var session = _mapper.Map<AuthSession>(createSessionDto);
            await _sessions.AddAsync(session);
            await _sessions.SaveChangesAsync();

            _logger.LogInformation("Сессия аутентификации для пользователя {UserId} успешно создана", createSessionDto.UserId);
            return Result.Ok();
        }

        public async Task<Result> DeactivateAsync(int id)
        {
            _logger.LogInformation("Деактивация сессии аутентификации с ID {Id}", id);

            var session = await _sessions.GetByIdAsync(id);
            if (session == null)
            {
                _logger.LogWarning("Сессия аутентификации с ID {Id} не найдена", id);
                return Result.Fail("Сессия аутентификации не найдена");
            }

            session.IsActive = false;
            _sessions.Update(session);
            await _sessions.SaveChangesAsync();

            _logger.LogInformation("Сессия аутентификации с ID {Id} успешно деактивирована", id);
            return Result.Ok();
        }

        public async Task<Result> DeactivateAllUserSessionsAsync(int userId)
        {
            _logger.LogInformation("Деактивация всех сессий пользователя {UserId}", userId);

            var sessions = await _sessions.GetByUserAsync(userId);
            if (sessions == null || !sessions.Any())
            {
                _logger.LogWarning("Активные сессии пользователя {UserId} не найдены", userId);
                return Result.Fail("Активные сессии не найдены");
            }

            foreach (var session in sessions)
            {
                session.IsActive = false;
            }

            await _sessions.SaveChangesAsync();

            _logger.LogInformation("Все сессии пользователя {UserId} успешно деактивированы. Деактивировано {Count} сессий", userId, sessions.Count());
            return Result.Ok();
        }

        public async Task<Result<IEnumerable<ReadAuthSessionDto>>> GetActiveSessionsByUserIdAsync(int userId)
        {
            _logger.LogInformation("Запрос активных сессий пользователя {UserId}", userId);

            var sessions = await _sessions.GetByUserAsync(userId);

            if (sessions == null || !sessions.Any())
            {
                _logger.LogWarning("Активные сессии пользователя {UserId} не найдены", userId);
                return Result<IEnumerable<ReadAuthSessionDto>>.Fail("Активные сессии не найдены");
            }

            var dtos = _mapper.Map<IEnumerable<ReadAuthSessionDto>>(sessions);

            _logger.LogInformation("Найдено {Count} активных сессий пользователя {UserId}", dtos.Count(), userId);
            return Result<IEnumerable<ReadAuthSessionDto>>.Ok(dtos);
        }
    }
}
