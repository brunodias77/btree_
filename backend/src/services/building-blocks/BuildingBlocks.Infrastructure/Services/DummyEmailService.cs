using BuildingBlocks.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Services;

public class DummyEmailService : IEmailService
{
    private readonly ILogger<DummyEmailService> _logger;

    public DummyEmailService(ILogger<DummyEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("📧 [DummyEmailService] Enviando email para {To} com assunto '{Subject}'. Corpo: {Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
