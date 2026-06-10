using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleOfDarkMind.Application.DTOs;
using PeopleOfDarkMind.Application.Features.Auth;

namespace PeopleOfDarkMind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new RegisterCommand(request), ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new LoginCommand(request), ct);
        return result.Success ? Ok(result.Data) : Unauthorized(new { error = result.Error });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() =>
        Ok(new { userId = User.FindFirstValue(ClaimTypes.NameIdentifier), email = User.FindFirstValue(ClaimTypes.Email) });
}
