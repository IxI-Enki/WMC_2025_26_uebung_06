using Api.Extensions;
using Application.Common.Results;
using Application.Features.Dtos;
using Application.Features.Usages.Commands.CreateUsage;
using Application.Features.Usages.Commands.DeleteUsage;
using Application.Features.Usages.Commands.UpdateUsage;
using Application.Features.Usages.Queries.GetAllUsages;
using Application.Features.Usages.Queries.GetUsageById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Endpunkte rund um Usages (Gerätenutzungen).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsagesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Liefert alle Usages sortiert nach Startdatum.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetUsageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllUsagesQuery(), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Liefert eine Usage per Id, falls vorhanden.
    /// Sonst 404 Not Found.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetUsageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUsageByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Legt eine neue Usage an.
    /// </summary>
    /// <remarks>
    /// Regeln:
    /// - Device und Person müssen existieren
    /// - To muss >= From sein
    /// - Usages können nur für die Zukunft gebucht werden
    /// - Keine Überlappung mit anderen Usages desselben Devices
    /// - Auch am Rückgabetag darf keine neue Usage beginnen
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(GetUsageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUsageCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this, createdAtAction: nameof(GetById),
            routeValues: new { id = result?.Value?.Id });
    }

    /// <summary>
    /// Aktualisiert eine Usage.
    /// </summary>
    /// <remarks>
    /// Regeln wie beim Erstellen.
    /// </remarks>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GetUsageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUsageCommand command, CancellationToken ct)
    {
        if (id != command.Id)
        {
            Result<GetUsageDto> badResult = Result<GetUsageDto>.ValidationError(
                "The route ID does not match the usage ID in the request body.");
            return badResult.ToActionResult(this);
        }
        var result = await mediator.Send(new UpdateUsageCommand(id, command.DeviceId, command.PersonId, command.From, command.To), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Löscht eine Usage.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteUsageCommand(id), ct);
        return result.ToActionResult(this);
    }
}

