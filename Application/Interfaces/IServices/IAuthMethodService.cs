using Application.Common.ResultInfo;
using Application.DTOs.AuthMethodDto;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IAuthMethodService
    {
        Task<Result<IEnumerable<ReadAuthMethodDto>>> GetAllAsync();
        Task<Result<ReadAuthMethodDto>> GetByIdAsync(int id);
        Task<Result> CreateAsync(CreateAuthMethodDto createMethodDto);
        Task<Result> UpdateAsync(int id, CreateAuthMethodDto updateMethodDto);
        Task<Result> ToggleStatusAsync(int id);
    }
}
