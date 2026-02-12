namespace BuildingBlocks.Infrastructure.BackgroundJobs;
/// <summary>
/// Interface para jobs em background.
/// Jobs são executados periodicamente pelo BackgroundJobRunner.
/// </summary>
public interface IBackgroundJob
{
    /// <summary>
    /// Nome do job para identificação e logging.
    /// </summary>
    string JobName { get; }

    /// <summary>
    /// Intervalo de execução do job.
    /// </summary>
    TimeSpan Interval { get; }

    /// <summary>
    /// Executa o job.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task ExecuteAsync(CancellationToken cancellationToken);
}
