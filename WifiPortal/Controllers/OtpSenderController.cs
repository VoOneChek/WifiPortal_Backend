using Application.DTOs.OtpCodeDto;
using Application.Interfaces.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WifiPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OtpSenderController : ControllerBase
{
    private readonly IOtpSenderService _otps;

    public OtpSenderController(IOtpSenderService otps)
    {
        _otps = otps;
    }

    [HttpPost("sms")]
    public async Task<IActionResult> CreateSms(CreateOtpCodeDto createOtpDto)
    {
        var result = await _otps.SendSmsAsync(createOtpDto.PhoneNumber);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("telegram")]
    public async Task<IActionResult> CreateTelegram(CreateOtpCodeDto createOtpDto)
    {
        var result = await _otps.SendTelegramAsync(createOtpDto.PhoneNumber);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("resend-sms")]
    public async Task<IActionResult> ResendSMS(CreateOtpCodeDto createOtpDto)
    {
        var result = await _otps.ResendSmsAsync(createOtpDto.PhoneNumber);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("resend-telegram")]
    public async Task<IActionResult> ResendTelegram(CreateOtpCodeDto createOtpDto)
    {
        var result = await _otps.ResendTelegramAsync(createOtpDto.PhoneNumber);
        return result.Success ? Ok() : BadRequest(result.Error);
    }
}
