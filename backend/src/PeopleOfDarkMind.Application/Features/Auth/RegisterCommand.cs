using MediatR;
using PeopleOfDarkMind.Application.Common;
using PeopleOfDarkMind.Application.DTOs;
using PeopleOfDarkMind.Application.Interfaces;

namespace PeopleOfDarkMind.Application.Features.Auth;

public record RegisterCommand(RegisterRequest Request) : IRequest<Result<AuthResponse>>;

public class RegisterCommandHandler(IAuthService authService)
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    public Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken ct) =>
        authService.RegisterAsync(request.Request, ct);
}
