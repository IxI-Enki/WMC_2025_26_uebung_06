using Api.Extensions;
using Application.Common.Results;
using Application.Features.Devices.Commands.CreateDevice;
using Application.Features.Devices.Commands.DeleteDevice;
using Application.Features.Devices.Commands.UpdateDevice;
using Application.Features.Devices.Queries.GetAllDevices;
using Application.Features.Devices.Queries.GetDeviceById;
using Application.Features.Devices.Queries.GetDevicesWithCounts;
using Application.Features.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Endpunkte rund um Devices.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DevicesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Liefert alle Devices sortiert nach DeviceName.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetDeviceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllDevicesQuery(), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Liefert ein Device per Id, falls vorhanden.
    /// Sonst 404 Not Found.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetDeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetDeviceByIdQuery(id), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Liefert alle Devices mit Anzahl der Nutzungen (Usages).
    /// </summary>
    [HttpGet("with-counts")]
    [ProducesResponseType(typeof(IEnumerable<GetDeviceWithCountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllWithCounts(CancellationToken ct)
    {
        var result = await mediator.Send(new GetDevicesWithCountsQuery(), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Legt ein neues Device an.
    /// </summary>
    /// <remarks>
    /// Regeln:
    /// - DeviceName mindestens 2 Zeichen
    /// - SerialNumber mindestens 3 Zeichen und eindeutig
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(GetDeviceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateDeviceCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this, createdAtAction: nameof(GetById),
            routeValues: new { id = result?.Value?.Id });
    }

    /// <summary>
    /// Aktualisiert ein Device.
    /// </summary>
    /// <remarks>
    /// Regeln wie beim Erstellen.
    /// </remarks>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GetDeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDeviceCommand command, CancellationToken ct)
    {
        if (id != command.Id)
        {
            Result<GetDeviceDto> badResult = Result<GetDeviceDto>.ValidationError(
                "The route ID does not match the device ID in the request body.");
            return badResult.ToActionResult(this);
        }
        var result = await mediator.Send(new UpdateDeviceCommand(id, command.SerialNumber, command.DeviceName, command.DeviceType), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Löscht ein Device.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteDeviceCommand(id), ct);
        return result.ToActionResult(this);
    }
}

