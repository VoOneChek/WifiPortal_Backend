using Application.Common.ResultInfo;
using Application.DTOs.OtpCodeDto;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IOtpService
    {
        Task<Result> CreateAsync(CreateOtpCodeDto createOtpDto);
        Task<Result> VerifyOtpAsync(string phoneNumber, string code);
        Task<Result> ResendOtpAsync(string phoneNumber);
        Task<Result> InvalidateOtpAsync(string phoneNumber);
        Task<Result<OtpStatusDto>> GetOtpStatusAsync(string phoneNumber);
    }
}
