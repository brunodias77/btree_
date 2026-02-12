using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuildingBlocks.Web.Filters;

/// <summary>
/// Filtro de validação automática do ModelState.
/// Retorna resposta padronizada em caso de erros de validação.
/// </summary>
public sealed class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(error => new ValidationError
                {
                    Field = e.Key,
                    Message = error.ErrorMessage
                }))
                .ToList();

            var response = ApiErrorResponse.WithValidationErrors(errors);
            context.Result = new BadRequestObjectResult(response);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Não faz nada após a action
    }
}

/// <summary>
/// Erro de validação individual.
/// </summary>
public sealed record ValidationError
{
    /// <summary>
    /// Nome do campo com erro.
    /// </summary>
    public required string Field { get; init; }

    /// <summary>
    /// Mensagem de erro.
    /// </summary>
    public required string Message { get; init; }
}
