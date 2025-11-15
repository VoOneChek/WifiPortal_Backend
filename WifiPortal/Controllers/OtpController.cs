using Application.DTOs.OtpCodeDto;
using Application.Interfaces.IServices;
using Domain.Entities;
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

    [HttpPost]
    public async Task<IActionResult> Create(CreateOtpCodeDto createOtpDto)
    {
        var result = await _otps.CreateAsync(createOtpDto);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpRequestDto verifyOtpDto)
    {
        var result = await _otps.VerifyOtpAsync(verifyOtpDto.PhoneNumber, verifyOtpDto.Code);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("{phoneNumber}/resend")]
    public async Task<IActionResult> ResendOtp(string phoneNumber)
    {
        var result = await _otps.ResendOtpAsync(phoneNumber);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("{phoneNumber}")]
    public async Task<IActionResult> InvalidateOtp(string phoneNumber)
    {
        var result = await _otps.InvalidateOtpAsync(phoneNumber);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpGet("{phoneNumber}/status")]
    public async Task<IActionResult> GetOtpStatus(string phoneNumber)
    {
        var result = await _otps.GetOtpStatusAsync(phoneNumber);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }
}
