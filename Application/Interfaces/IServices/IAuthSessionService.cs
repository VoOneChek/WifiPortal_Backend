using Application.Common.ResultInfo;
using Application.DTOs.AuthSessionDto;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IAuthSessionService
    {
        Task<Result<IEnumerable<ReadAuthSessionDto>>> GetAllAsync();
        Task<Result<ReadAuthSessionDto>> GetByIdAsync(int id);
        Task<Result> CreateAsync(CreateAuthSessionDto createSessionDto);
        Task<Result> DeleteSessionAsync(int id);
        Task<Result> DeactivateAsync(int id);
        Task<Result> DeactivateAllUserSessionsAsync(int userId);
        Task<Result<IEnumerable<ReadAuthSessionDto>>> GetSessionsByUserIdAsync(int userId);
        Task<Result<IEnumerable<ReadAuthSessionDto>>> GetActiveSessionsByUserIdAsync(int userId);
        Task<Result<ReadAuthSessionDto>> ReactivateSessionByMacAsync(string macAddress);
    }
}
