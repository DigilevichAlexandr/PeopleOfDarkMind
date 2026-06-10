using MediatR;
using PeopleOfDarkMind.Application.Common;
using PeopleOfDarkMind.Application.DTOs;
using PeopleOfDarkMind.Application.Interfaces;

namespace PeopleOfDarkMind.Application.Features.Game;

public record CreateNewGameCommand(string UserId, string? CharacterName) : IRequest<Result<CharacterDto>>;
public record PerformActionCommand(string UserId, PerformActionRequest Request) : IRequest<Result<GameStateDto>>;
public record MakeChoiceCommand(string UserId, MakeChoiceRequest Request) : IRequest<Result<GameStateDto>>;
public record TravelCommand(string UserId, Guid LocationId) : IRequest<Result<GameStateDto>>;
public record TriggerRandomEventCommand(string UserId) : IRequest<Result<GameStateDto>>;

public class CreateNewGameHandler(IGameEngineService engine)
    : IRequestHandler<CreateNewGameCommand, Result<CharacterDto>>
{
    public Task<Result<CharacterDto>> Handle(CreateNewGameCommand request, CancellationToken ct) =>
        engine.CreateNewGameAsync(request.UserId, request.CharacterName, ct);
}

public class PerformActionHandler(IGameEngineService engine)
    : IRequestHandler<PerformActionCommand, Result<GameStateDto>>
{
    public Task<Result<GameStateDto>> Handle(PerformActionCommand request, CancellationToken ct) =>
        engine.PerformActionAsync(request.UserId, request.Request, ct);
}

public class MakeChoiceHandler(IGameEngineService engine)
    : IRequestHandler<MakeChoiceCommand, Result<GameStateDto>>
{
    public Task<Result<GameStateDto>> Handle(MakeChoiceCommand request, CancellationToken ct) =>
        engine.MakeChoiceAsync(request.UserId, request.Request, ct);
}

public class TravelHandler(IGameEngineService engine)
    : IRequestHandler<TravelCommand, Result<GameStateDto>>
{
    public Task<Result<GameStateDto>> Handle(TravelCommand request, CancellationToken ct) =>
        engine.TravelToLocationAsync(request.UserId, request.LocationId, ct);
}

public class TriggerRandomEventHandler(IGameEngineService engine)
    : IRequestHandler<TriggerRandomEventCommand, Result<GameStateDto>>
{
    public Task<Result<GameStateDto>> Handle(TriggerRandomEventCommand request, CancellationToken ct) =>
        engine.TriggerRandomEventAsync(request.UserId, ct);
}
