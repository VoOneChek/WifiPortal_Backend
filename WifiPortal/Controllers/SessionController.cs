using Application.DTOs.AuthSessionDto;
using Application.Interfaces.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WifiPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{
    private readonly IAuthSessionService _sessions;

    public SessionController(IAuthSessionService sessions)
    {
        _sessions = sessions;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sessions.GetAllAsync();
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _sessions.GetByIdAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAuthSessionDto createSessionDto)
    {
        var result = await _sessions.CreateAsync(createSessionDto);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _sessions.DeactivateAsync(id);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpPatch("user/{userId}/deactivate-all")]
    public async Task<IActionResult> DeactivateAllUserSessions(int userId)
    {
        var result = await _sessions.DeactivateAllUserSessionsAsync(userId);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpGet("user/{userId}/active")]
    public async Task<IActionResult> GetActiveSessionsByUserId(int userId)
    {
        var result = await _sessions.GetActiveSessionsByUserIdAsync(userId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }
}
