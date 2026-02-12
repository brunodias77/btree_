
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.BackgroundJobs;

/// <summary>
/// Executor de jobs em background.
/// Gerencia múltiplos IBackgroundJob e executa-os em seus intervalos.
/// </summary>
public sealed class BackgroundJobRunner : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundJobRunner> _logger;

    public BackgroundJobRunner(
        IServiceProvider serviceProvider,
        ILogger<BackgroundJobRunner> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BackgroundJobRunner iniciado");

        // Executa cada job em sua própria task
        using var scope = _serviceProvider.CreateScope();
        var jobs = scope.ServiceProvider.GetServices<IBackgroundJob>().ToList();

        if (jobs.Count == 0)
        {
            _logger.LogWarning("Nenhum job registrado");
            return;
        }

        _logger.LogInformation("{Count} jobs registrados: {Jobs}",
            jobs.Count,
            string.Join(", ", jobs.Select(j => j.JobName)));

        var tasks = jobs.Select(job => RunJobLoop(job, stoppingToken));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Loop de execução de um job individual.
    /// </summary>
    private async Task RunJobLoop(IBackgroundJob job, CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Job {JobName} iniciado com intervalo de {Interval}",
            job.JobName,
            job.Interval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(job.Interval, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                    break;

                _logger.LogDebug("Executando job {JobName}", job.JobName);

                using var scope = _serviceProvider.CreateScope();

                // Obtém nova instância do job com escopo de DI
                var scopedJob = scope.ServiceProvider
                    .GetServices<IBackgroundJob>()
                    .FirstOrDefault(j => j.JobName == job.JobName);

                if (scopedJob is not null)
                {
                    await scopedJob.ExecuteAsync(stoppingToken);
                }

                _logger.LogDebug("Job {JobName} concluído", job.JobName);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Shutdown normal
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar job {JobName}", job.JobName);
                // Aguarda um pouco antes de tentar novamente
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        _logger.LogInformation("Job {JobName} finalizado", job.JobName);
    }
}
