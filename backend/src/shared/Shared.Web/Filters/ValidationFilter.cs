using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Web.Models;

namespace Shared.Web.Filters;

/// <summary>
/// Action filter que valida o ModelState antes de executar a action.
/// Retorna 400 com ApiErrorResponse contendo os erros de validação por campo.
/// Aplicar globalmente em AddControllers ou por controller/action.
/// </summary>
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
            return;

        var errors = context.ModelState
            .Where(e => e.Value != null && e.Value.Errors.Count > 0)
            .ToDictionary(
                e => e.Key,
                e => e.Value!.Errors.Select(err => err.ErrorMessage).ToArray()
            );

        var errorResponse = ApiErrorResponse.ValidationError("Erro de validação.", errors);

        context.Result = new BadRequestObjectResult(errorResponse);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Nenhuma ação necessária após a execução
    }
}