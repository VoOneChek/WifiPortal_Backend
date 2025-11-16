using Application.DTOs.AuthMethodDto;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WifiPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MethodController : ControllerBase
{
    private readonly IAuthMethodService _methods;

    public MethodController(IAuthMethodService methods)
    {
        _methods = methods;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _methods.GetAllAsync();
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _methods.GetByIdAsync(id);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateAuthMethodDto createMethodDto)
    {
        var result = await _methods.CreateAsync(createMethodDto);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CreateAuthMethodDto updateMethodDto)
    {
        var result = await _methods.UpdateAsync(id, updateMethodDto);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpPatch("{id}/toggle")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _methods.ToggleStatusAsync(id);
        return result.Success ? Ok() : BadRequest(result.Error);
    }
}
