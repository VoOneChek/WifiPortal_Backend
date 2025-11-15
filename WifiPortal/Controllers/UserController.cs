using Application.DTOs.UserDto;
using Application.Interfaces.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WifiPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _users;

    public UserController(IUserService users)
    {
        _users = users;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _users.GetAllAsync();
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _users.GetByIdAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpGet("phone/{phone}")]
    public async Task<IActionResult> GetByPhone(string phone)
    {
        var result = await _users.GetByPhoneAsync(phone);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpGet("{id}/with-sessions")]
    public async Task<IActionResult> GetUserWithSessions(int id)
    {
        var result = await _users.GetUserWithSessionsAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto createUserDto)
    {
        var result = await _users.CreateAsync(createUserDto);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateUserDto updateUserDto)
    {
        var result = await _users.UpdateAsync(id, updateUserDto);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _users.DeleteAsync(id);
        return result.Success ? Ok() : BadRequest(result.Error);
    }
}