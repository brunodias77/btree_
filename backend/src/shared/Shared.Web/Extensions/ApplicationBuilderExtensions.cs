using Microsoft.AspNetCore.Builder;
using Shared.Web.Middlewares;

namespace Shared.Web.Extensions;

/// <summary>
/// Extension methods para registrar middlewares compartilhados na pipeline.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adiciona os middlewares compartilhados na ordem correta.
    /// Deve ser chamado antes de UseAuthentication/UseAuthorization.
    /// </summary>
    public static IApplicationBuilder UseSharedMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        return app;
    }
}