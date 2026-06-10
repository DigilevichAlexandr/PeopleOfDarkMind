using MediatR;
using PeopleOfDarkMind.Application.Common;
using PeopleOfDarkMind.Application.DTOs;
using PeopleOfDarkMind.Application.Interfaces;

namespace PeopleOfDarkMind.Application.Features.Auth;

public record LoginCommand(LoginRequest Request) : IRequest<Result<AuthResponse>>;

public class LoginCommandHandler(IAuthService authService)
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct) =>
        authService.LoginAsync(request.Request, ct);
}
