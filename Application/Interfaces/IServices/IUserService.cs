using Application.Common.ResultInfo;
using Application.DTOs.UserDto;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<Result<IEnumerable<ReadUserDto>>> GetAllAsync();
        Task<Result<ReadUserDto>> GetByIdAsync(int id);
        Task<Result<ReadUserDto>> GetByPhoneAsync(string phone);
        Task<Result> CreateAsync(CreateUserDto createUserDto);
        Task<Result> UpdateAsync(int id, UpdateUserDto updateUserDto);
        Task<Result> UpdatePhoneNumberAsync(int id, UpdatePhoneNumberDto updatePhoneNumberDto);
        Task<Result> DeleteAsync(int id);
        Task<Result<UserDetailDto>> GetUserWithSessionsAsync(int id);
        Task<Result> LinkTelegramAsync(string phoneNumber, long chatId, string? username);
    }
}
