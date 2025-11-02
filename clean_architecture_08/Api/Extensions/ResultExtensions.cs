using Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Konvertiert ein generisches Result<T> aus der Anwendungsschicht in ein passendes IActionResult.
    /// </summary>
    public static IActionResult ToActionResult<T>(this Result<T> result,
            ControllerBase controller,
            string? createdAtAction = null,
            object? routeValues = null)
    {
        return result.Type switch
        {
            ResultType.Success => controller.Ok(result.Value),
            // Für Created nach Möglichkeit immer CreatedAtAction verwenden, damit der Location-Header gesetzt wird
            ResultType.Created => createdAtAction is not null
                ? controller.CreatedAtAction(createdAtAction, routeValues, result.Value)
                : controller.StatusCode(StatusCodes.Status201Created, result.Value),
            ResultType.NoContent => controller.NoContent(),
            ResultType.NotFound => controller.NotFound(result.Message),
            ResultType.ValidationError => controller.BadRequest(result.Message),
            ResultType.Conflict => controller.Conflict(result.Message),
            _ => controller.Problem(
                detail: result.Message ?? "An unexpected error occurred.",
                statusCode: 500
            )
        };
    }

    /// <summary>
    /// Konvertiert ein nicht-generisches Result in ein passendes IActionResult.
    /// </summary>
    public static IActionResult ToActionResult(this Result result,
            ControllerBase controller,
            string? createdAtAction = null,
            object? routeValues = null)
    {
        return result.Type switch
        {
            // Ohne Value keinen Body zurückgeben
            ResultType.Success => controller.Ok(),
            // Für Created stets CreatedAtAction nutzen, Body weglassen
            ResultType.Created => createdAtAction is not null
                ? controller.CreatedAtAction(createdAtAction, routeValues, null)
                : controller.StatusCode(StatusCodes.Status201Created),
            ResultType.NoContent => controller.NoContent(),
            ResultType.NotFound => controller.NotFound(result.Message),
            ResultType.ValidationError => controller.BadRequest(result.Message),
            ResultType.Conflict => controller.Conflict(result.Message),
            _ => controller.Problem(
                detail: result.Message ?? "An unexpected error occurred.",
                statusCode: 500
            )
        };
    }
}

