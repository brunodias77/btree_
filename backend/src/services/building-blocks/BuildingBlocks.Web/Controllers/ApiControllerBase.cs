using BuildingBlocks.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Web.Controllers;

/// <summary>
/// Controller base para todos os controllers da API.
/// Fornece métodos auxiliares para respostas padronizadas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Retorna resultado baseado no Result pattern.
    /// Converte automaticamente para ActionResult apropriado.
    /// </summary>
    /// <param name="result">Resultado da operação.</param>
    /// <returns>ActionResult com resposta apropriada.</returns>
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse.Ok());
        }

        return HandleError(result.Error);
    }

    /// <summary>
    /// Retorna resultado com valor baseado no Result pattern.
    /// </summary>
    /// <typeparam name="T">Tipo do valor.</typeparam>
    /// <param name="result">Resultado da operação.</param>
    /// <returns>ActionResult com resposta apropriada.</returns>
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Ok(result.Value));
        }

        return HandleError(result.Error);
    }

    /// <summary>
    /// Retorna resultado para operação de criação (201 Created).
    /// </summary>
    /// <typeparam name="T">Tipo do valor.</typeparam>
    /// <param name="result">Resultado da operação.</param>
    /// <param name="actionName">Nome da action para Location header.</param>
    /// <param name="routeValues">Valores de rota para Location header.</param>
    /// <returns>ActionResult com resposta apropriada.</returns>
    protected IActionResult HandleCreatedResult<T>(
        Result<T> result,
        string actionName,
        object? routeValues = null)
    {
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                actionName,
                routeValues,
                ApiResponse<T>.Ok(result.Value));
        }

        return HandleError(result.Error);
    }

    /// <summary>
    /// Retorna resposta de erro apropriada baseada no código.
    /// </summary>
    private IActionResult HandleError(Error error)
    {
        var errorCode = error.Code.ToLowerInvariant();

        // NotFound
        if (errorCode.Contains("notfound"))
        {
            return NotFound(ApiErrorResponse.NotFound(error.Message));
        }

        // Conflict/Duplicate
        if (errorCode.Contains("conflict") || errorCode.Contains("duplicate") || errorCode.Contains("exists"))
        {
            return Conflict(ApiErrorResponse.Conflict(error.Message));
        }

        // Validation
        if (errorCode.Contains("validation"))
        {
            return BadRequest(ApiErrorResponse.ValidationError(error.Message));
        }

        // Unauthorized
        if (errorCode.Contains("unauthorized") || errorCode.Contains("authentication"))
        {
            return Unauthorized(ApiErrorResponse.Unauthorized(error.Message));
        }

        // Forbidden
        if (errorCode.Contains("forbidden") || errorCode.Contains("permission"))
        {
            return StatusCode(403, ApiErrorResponse.Forbidden(error.Message));
        }

        // Default: Bad Request
        return BadRequest(ApiErrorResponse.BadRequest(error.Code, error.Message));
    }

    /// <summary>
    /// Obtém o ID do usuário atual.
    /// </summary>
    protected Guid? CurrentUserId
    {
        get
        {
            var claim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (claim is null || !Guid.TryParse(claim.Value, out var userId))
                return null;

            return userId;
        }
    }

    /// <summary>
    /// Obtém o ID do usuário atual ou lança exceção.
    /// </summary>
    protected Guid RequiredUserId =>
        CurrentUserId ?? throw new UnauthorizedAccessException("Usuário não autenticado.");
}
