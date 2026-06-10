using PeopleOfDarkMind.Application.Common;
using PeopleOfDarkMind.Application.DTOs;

namespace PeopleOfDarkMind.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
