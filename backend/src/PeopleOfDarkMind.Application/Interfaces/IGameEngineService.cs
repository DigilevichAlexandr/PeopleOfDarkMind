using PeopleOfDarkMind.Application.Common;
using PeopleOfDarkMind.Application.DTOs;

namespace PeopleOfDarkMind.Application.Interfaces;

public interface IGameEngineService
{
    Task<Result<CharacterDto>> CreateNewGameAsync(string userId, string? characterName, CancellationToken ct = default);
    Task<Result<GameStateDto>> GetGameStateAsync(string userId, CancellationToken ct = default);
    Task<Result<GameStateDto>> PerformActionAsync(string userId, PerformActionRequest request, CancellationToken ct = default);
    Task<Result<GameStateDto>> MakeChoiceAsync(string userId, MakeChoiceRequest request, CancellationToken ct = default);
    Task<Result<GameStateDto>> TravelToLocationAsync(string userId, Guid locationId, CancellationToken ct = default);
    Task<Result<GameStateDto>> TriggerRandomEventAsync(string userId, CancellationToken ct = default);
}
