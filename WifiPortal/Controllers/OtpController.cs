using Application.DTOs.OtpCodeDto;
using Application.Interfaces.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WifiPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OtpController : ControllerBase
{
    private readonly IOtpService _otps;

    public OtpController(IOtpService otps)
    {
        _otps = otps;
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpRequestDto verifyOtpDto)
    {
        var result = await _otps.VerifyOtpAsync(verifyOtpDto.PhoneNumber, verifyOtpDto.Code);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpDelete("{phoneNumber}")]
    public async Task<IActionResult> InvalidateOtp(string phoneNumber)
    {
        var result = await _otps.InvalidateOtpAsync(phoneNumber);
        return result.Success ? Ok() : BadRequest(result.Error);
    }
}
