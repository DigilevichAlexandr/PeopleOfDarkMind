using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleOfDarkMind.Application.DTOs;
using PeopleOfDarkMind.Application.Features.Game;

namespace PeopleOfDarkMind.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GameController(IMediator mediator) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost("new")]
    public async Task<IActionResult> NewGame([FromBody] NewGameRequest? request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateNewGameCommand(UserId, request?.CharacterName), ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("state")]
    public async Task<IActionResult> GetState(CancellationToken ct)
    {
        var result = await mediator.Send(new GetGameStateQuery(UserId), ct);
        return result.Success ? Ok(result.Data) : NotFound(new { error = result.Error });
    }

    [HttpGet("character")]
    public async Task<IActionResult> GetCharacter(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCharacterQuery(UserId), ct);
        return result.Success ? Ok(result.Data) : NotFound(new { error = result.Error });
    }

    [HttpGet("locations")]
    public async Task<IActionResult> GetLocations(CancellationToken ct)
    {
        var result = await mediator.Send(new GetLocationsQuery(UserId), ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("npcs")]
    public async Task<IActionResult> GetNpcs(CancellationToken ct)
    {
        var result = await mediator.Send(new GetNpcsQuery(UserId), ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("evidence")]
    public async Task<IActionResult> GetEvidence(CancellationToken ct)
    {
        var result = await mediator.Send(new GetEvidenceQuery(UserId), ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("journal")]
    public async Task<IActionResult> GetJournal(CancellationToken ct)
    {
        var result = await mediator.Send(new GetJournalQuery(UserId), ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("saves")]
    public async Task<IActionResult> GetSaves(CancellationToken ct)
    {
        var result = await mediator.Send(new GetSavesQuery(UserId), ct);
        return Ok(result.Data);
    }

    [HttpPost("action")]
    public async Task<IActionResult> PerformAction([FromBody] PerformActionRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new PerformActionCommand(UserId, request), ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("choice")]
    public async Task<IActionResult> MakeChoice([FromBody] MakeChoiceRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new MakeChoiceCommand(UserId, request), ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("travel/{locationId:guid}")]
    public async Task<IActionResult> Travel(Guid locationId, CancellationToken ct)
    {
        var result = await mediator.Send(new TravelCommand(UserId, locationId), ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("random-event")]
    public async Task<IActionResult> RandomEvent(CancellationToken ct)
    {
        var result = await mediator.Send(new TriggerRandomEventCommand(UserId), ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}

public record NewGameRequest(string? CharacterName);
