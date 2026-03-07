using Microsoft.AspNetCore.Http;

namespace Shared.Web.Middlewares;

/// <summary>
/// Adiciona headers de segurança recomendados pela OWASP em todas as responses.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Previne que o browser interprete o content type de forma diferente
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        // Previne clickjacking
        context.Response.Headers["X-Frame-Options"] = "DENY";

        // Desabilita XSS filter do browser (CSP é mais seguro)
        context.Response.Headers["X-XSS-Protection"] = "0";

        // Controla o que é enviado no header Referer
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Remove header que expõe a tecnologia do servidor
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("Server");

        // Força HTTPS (em produção, ajustar max-age conforme necessidade)
        context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";

        // Content Security Policy básica para APIs
        context.Response.Headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";

        // Permissões de APIs do browser desabilitadas
        context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

        await _next(context);
    }
}