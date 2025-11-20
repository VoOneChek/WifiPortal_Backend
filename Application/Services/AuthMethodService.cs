using Application.Common.ResultInfo;
using Application.DTOs.AuthMethodDto;
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
    public class AuthMethodService : IAuthMethodService
    {
        private readonly IAuthMethodRepository _methods;
        private readonly ILogger<AuthMethodService> _logger;
        private readonly IMapper _mapper;

        public AuthMethodService(IAuthMethodRepository methods, ILogger<AuthMethodService> logger, IMapper mapper)
        {
            _methods = methods;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<ReadAuthMethodDto>>> GetAllAsync()
        {
            _logger.LogInformation("Запрос всех методов аутентификации");

            var methods = await _methods.GetAllAsync();

            if (methods == null || !methods.Any())
            {
                _logger.LogWarning("Методы аутентификации не найдены");
                return Result<IEnumerable<ReadAuthMethodDto>>.Fail("Методы аутентификации не найдены");
            }

            var dtos = _mapper.Map<IEnumerable<ReadAuthMethodDto>>(methods);

            _logger.LogInformation("Найдено {Count} методов аутентификации", dtos.Count());
            return Result<IEnumerable<ReadAuthMethodDto>>.Ok(dtos);
        }

        public async Task<Result<ReadAuthMethodDto>> GetByIdAsync(int id)
        {
            _logger.LogInformation("Запрос метода аутентификации по ID {Id}", id);

            var method = await _methods.GetByIdAsync(id);

            if (method == null)
            {
                _logger.LogWarning("Метод аутентификации с ID {Id} не найден", id);
                return Result<ReadAuthMethodDto>.Fail("Метод аутентификации не найден");
            }

            var dto = _mapper.Map<ReadAuthMethodDto>(method);

            _logger.LogInformation("Метод аутентификации с ID {Id} успешно найден", id);
            return Result<ReadAuthMethodDto>.Ok(dto);
        }

        public async Task<Result> CreateAsync(CreateAuthMethodDto createMethodDto)
        {
            _logger.LogInformation("Создание метода аутентификации с названием \"{Name}\"", createMethodDto.Name);

            if (createMethodDto == null)
            {
                _logger.LogWarning("Переданный метод аутентификации равен null");
                return Result.Fail("Метод аутентификации не может быть null");
            }

            if (string.IsNullOrWhiteSpace(createMethodDto.Name))
            {
                _logger.LogWarning("Название метода аутентификации не указано");
                return Result.Fail("Название метода аутентификации обязательно");
            }

            if (string.IsNullOrWhiteSpace(createMethodDto.Description))
            {
                _logger.LogWarning("Описание метода аутентификации не указано");
                return Result.Fail("Описание метода аутентификации обязательно");
            }

            var existingMethod = await _methods.GetByNameAsync(createMethodDto.Name);
            if (existingMethod != null)
            {
                _logger.LogWarning("Метод аутентификации с названием \"{Name}\" уже существует", createMethodDto.Name);
                return Result.Fail("Метод аутентификации с таким названием уже существует");
            }

            var method = _mapper.Map<AuthMethod>(createMethodDto);
            await _methods.AddAsync(method);
            await _methods.SaveChangesAsync();

            _logger.LogInformation("Метод аутентификации \"{Name}\" успешно создан", createMethodDto.Name);
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(int id, CreateAuthMethodDto updateMethodDto)
        {
            _logger.LogInformation("Обновление метода аутентификации с ID {Id}", id);

            if (updateMethodDto == null)
            {
                _logger.LogWarning("Переданный метод аутентификации равен null");
                return Result.Fail("Метод аутентификации не может быть null");
            }

            var existingMethod = await _methods.GetByIdAsync(id);
            if (existingMethod == null)
            {
                _logger.LogWarning("Метод аутентификации с ID {Id} не найден", id);
                return Result.Fail("Метод аутентификации не найден");
            }

            if (string.IsNullOrWhiteSpace(updateMethodDto.Name))
            {
                _logger.LogWarning("Название метода аутентификации не указано");
                return Result.Fail("Название метода аутентификации обязательно");
            }

            if (string.IsNullOrWhiteSpace(updateMethodDto.Description))
            {
                _logger.LogWarning("Описание метода аутентификации не указано");
                return Result.Fail("Описание метода аутентификации обязательно");
            }

            existingMethod.Name = updateMethodDto.Name;
            existingMethod.Description = updateMethodDto.Description;
            existingMethod.IsEnabled = updateMethodDto.IsEnabled;

            _methods.Update(existingMethod);
            await _methods.SaveChangesAsync();

            _logger.LogInformation("Метод аутентификации с ID {Id} успешно обновлен", id);
            return Result.Ok();
        }

        public async Task<Result> ToggleStatusAsync(int id)
        {
            _logger.LogInformation("Переключение статуса метода аутентификации с ID {Id}", id);

            var method = await _methods.GetByIdAsync(id);
            if (method == null)
            {
                _logger.LogWarning("Метод аутентификации с ID {Id} не найден", id);
                return Result.Fail("Метод аутентификации не найден");
            }

            method.IsEnabled = !method.IsEnabled;
            _methods.Update(method);
            await _methods.SaveChangesAsync();

            _logger.LogInformation("Статус метода аутентификации с ID {Id} изменен на {Status}", id, method.IsEnabled ? "включен" : "выключен");
            return Result.Ok();
        }
    }
}
