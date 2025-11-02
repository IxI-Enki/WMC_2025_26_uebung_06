using Api.Extensions;
using Application.Common.Results;
using Application.Features.Dtos;
using Application.Features.People.Commands.CreatePerson;
using Application.Features.People.Commands.DeletePerson;
using Application.Features.People.Commands.UpdatePerson;
using Application.Features.People.Queries.GetAllPeople;
using Application.Features.People.Queries.GetPersonById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Endpunkte rund um People (Personen).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PeopleController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Liefert alle Personen sortiert nach Nachname und Vorname.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetPersonDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllPeopleQuery(), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Liefert eine Person per Id, falls vorhanden.
    /// Sonst 404 Not Found.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetPersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPersonByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Legt eine neue Person an.
    /// </summary>
    /// <remarks>
    /// Regeln:
    /// - Alle Felder dürfen nicht leer sein
    /// - E-Mail-Adresse muss syntaktisch korrekt und eindeutig sein
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(GetPersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreatePersonCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this, createdAtAction: nameof(GetById),
            routeValues: new { id = result?.Value?.Id });
    }

    /// <summary>
    /// Aktualisiert eine Person.
    /// </summary>
    /// <remarks>
    /// Regeln wie beim Erstellen.
    /// </remarks>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GetPersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePersonCommand command, CancellationToken ct)
    {
        if (id != command.Id)
        {
            Result<GetPersonDto> badResult = Result<GetPersonDto>.ValidationError(
                "The route ID does not match the person ID in the request body.");
            return badResult.ToActionResult(this);
        }
        var result = await mediator.Send(new UpdatePersonCommand(id, command.LastName, command.FirstName, command.MailAddress), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Löscht eine Person.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeletePersonCommand(id), ct);
        return result.ToActionResult(this);
    }
}

