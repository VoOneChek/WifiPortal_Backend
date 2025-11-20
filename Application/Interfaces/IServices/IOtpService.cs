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
        Task<Result<string>> CreateOtpCodeAsync(string phoneNumber);
        Task<Result<AuthResponseDto>> VerifyOtpAsync(string phoneNumber, string code);
        Task<Result> InvalidateOtpAsync(string phoneNumber);
        Task<bool> HasActiveOtpAsync(string phoneNumber);
    }
}
