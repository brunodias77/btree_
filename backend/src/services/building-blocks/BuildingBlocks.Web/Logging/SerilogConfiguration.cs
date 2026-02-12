using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace BuildingBlocks.Web.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; // Add this using
using Serilog.Events;

/// <summary>
/// Tema Dracula para console do Serilog.
/// Cores baseadas na paleta oficial do Dracula Theme.
/// </summary>
public static class DraculaConsoleTheme
{
    /// <summary>
    /// Tema Dracula com cores ANSI.
    /// </summary>
    public static AnsiConsoleTheme Theme { get; } = new AnsiConsoleTheme(
        new Dictionary<ConsoleThemeStyle, string>
        {
            // Background: #282A36 (não aplicável em ANSI direto)
            // Foreground: #F8F8F2

            // Texto padrão - Foreground (#F8F8F2)
            [ConsoleThemeStyle.Text] = "\x1b[38;2;248;248;242m",

            // Texto secundário - Comment (#6272A4)
            [ConsoleThemeStyle.SecondaryText] = "\x1b[38;2;98;114;164m",

            // Texto com ênfase - Cyan (#8BE9FD)
            [ConsoleThemeStyle.TertiaryText] = "\x1b[38;2;139;233;253m",

            // Valores inválidos - Red (#FF5555)
            [ConsoleThemeStyle.Invalid] = "\x1b[38;2;255;85;85m",

            // Null - Purple (#BD93F9)
            [ConsoleThemeStyle.Null] = "\x1b[38;2;189;147;249m",

            // Nome de propriedade - Pink (#FF79C6)
            [ConsoleThemeStyle.Name] = "\x1b[38;2;255;121;198m",

            // String - Yellow (#F1FA8C)
            [ConsoleThemeStyle.String] = "\x1b[38;2;241;250;140m",

            // Número - Purple (#BD93F9)
            [ConsoleThemeStyle.Number] = "\x1b[38;2;189;147;249m",

            // Boolean - Purple (#BD93F9)
            [ConsoleThemeStyle.Boolean] = "\x1b[38;2;189;147;249m",

            // Scalar - Green (#50FA7B)
            [ConsoleThemeStyle.Scalar] = "\x1b[38;2;80;250;123m",

            // Level Verbose - Comment (#6272A4)
            [ConsoleThemeStyle.LevelVerbose] = "\x1b[38;2;98;114;164m",

            // Level Debug - Cyan (#8BE9FD)
            [ConsoleThemeStyle.LevelDebug] = "\x1b[38;2;139;233;253m",

            // Level Information - Green (#50FA7B)
            [ConsoleThemeStyle.LevelInformation] = "\x1b[38;2;80;250;123m",

            // Level Warning - Orange (#FFB86C)
            [ConsoleThemeStyle.LevelWarning] = "\x1b[38;2;255;184;108m",

            // Level Error - Red (#FF5555)
            [ConsoleThemeStyle.LevelError] = "\x1b[38;2;255;85;85m",

            // Level Fatal - Red com background (#FF5555 em bold)
            [ConsoleThemeStyle.LevelFatal] = "\x1b[1;38;2;255;85;85m",
        });

    /// <summary>
    /// Template de output padrão com cores do tema.
    /// </summary>
    public const string OutputTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] {SourceContext}{NewLine}      {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Template de output compacto.
    /// </summary>
    public const string CompactOutputTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] {Message:lj}{NewLine}{Exception}";
}

/// <summary>
/// Configuração do Serilog para aplicações ASP.NET Core.
/// </summary>
public static class SerilogConfiguration
{
    /// <summary>
    /// Cria o logger de bootstrap para capturar erros durante a inicialização.
    /// Deve ser chamado no início do Program.cs.
    /// </summary>
    /// <returns>O logger configurado.</returns>
    public static ILogger CreateBootstrapLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                theme: DraculaConsoleTheme.Theme,
                outputTemplate: DraculaConsoleTheme.OutputTemplate)
            .CreateBootstrapLogger();
    }

    /// <summary>
    /// Configura o Serilog no IServiceCollection.
    /// Define o logger global e adiciona o provider do Serilog.
    /// </summary>
    /// <param name="services">O IServiceCollection.</param>
    /// <param name="appName">Nome da aplicação para enriquecimento.</param>
    /// <returns>O IServiceCollection.</returns>
    public static IServiceCollection AddSerilogLogging(
        this IServiceCollection services,
        IConfiguration configuration, // Add this parameter
        string appName)
    {
        // Theme Dracula
        var draculaTheme = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
        {
            // Texto base
            [ConsoleThemeStyle.Text] = "\u001b[38;2;248;248;242m",        // Foreground
            [ConsoleThemeStyle.SecondaryText] = "\u001b[38;2;98;114;164m",
            [ConsoleThemeStyle.TertiaryText]  = "\u001b[38;2;98;114;164m",

            // Levels
            [ConsoleThemeStyle.LevelVerbose] = "\u001b[38;2;98;114;164m",      // Comment
            [ConsoleThemeStyle.LevelDebug] = "\u001b[38;2;139;233;253m",      // Cyan
            [ConsoleThemeStyle.LevelInformation] = "\u001b[38;2;80;250;123m", // Green
            [ConsoleThemeStyle.LevelWarning] = "\u001b[38;2;241;250;140m",    // Yellow
            [ConsoleThemeStyle.LevelError] = "\u001b[38;2;255;85;85m",        // Red
            [ConsoleThemeStyle.LevelFatal] = "\u001b[1;38;2;255;85;85m",      // Red bold

            // Estrutura / propriedades
            [ConsoleThemeStyle.Name]   = "\u001b[38;2;189;147;249m", // Purple
            [ConsoleThemeStyle.String] = "\u001b[38;2;80;250;123m",  // Green
            [ConsoleThemeStyle.Number] = "\u001b[38;2;139;233;253m", // Cyan
            [ConsoleThemeStyle.Boolean] = "\u001b[38;2;255;184;108m", // Orange
            [ConsoleThemeStyle.Null]   = "\u001b[38;2;98;114;164m",
            [ConsoleThemeStyle.Scalar] = "\u001b[38;2;248;248;242m"
        });

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration) // Add this line

            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Aplicacao", appName)
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .WriteTo.Console(
                theme: draculaTheme,
                outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] " +
                    "{Message:lj} " +
                    "{Properties:j}{NewLine}{NewLine}{Exception}"
            )
            .WriteTo.File(
                path: $"logs/{appName}-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate:
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] " +
                    "[{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
            )
            .CreateLogger();

        services.AddSerilog(dispose: true);

        return services;
    }

    /// <summary>
    /// Adiciona middleware de logging de requisições do Serilog.
    /// Deve ser chamado no início do pipeline.
    /// </summary>
    /// <param name="app">O WebApplication.</param>
    /// <returns>O WebApplication.</returns>
    public static WebApplication UseSerilogRequestLoggingMiddleware(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000}ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                diagnosticContext.Set("CorrelationId", httpContext.Items["CorrelationId"]?.ToString() ?? "N/A");

                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value ?? "N/A");
                }
            };
        });

        return app;
    }
}

/// <summary>
/// Extensões para execução segura da aplicação com Serilog.
/// </summary>
public static class SerilogApplicationRunner
{
    /// <summary>
    /// Executa a aplicação com tratamento de exceções e logging do Serilog.
    /// </summary>
    /// <param name="action">A ação de configuração e execução da aplicação.</param>
    public static void RunWithSerilog(Action action)
    {
        Log.Logger = SerilogConfiguration.CreateBootstrapLogger();

        try
        {
            Log.Information("🚀 Iniciando aplicação...");
            action();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "☠️ Aplicação falhou ao iniciar");
            throw;
        }
        finally
        {
            Log.Information("🛑 Aplicação encerrada");
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Executa a aplicação assíncrona com tratamento de exceções e logging do Serilog.
    /// </summary>
    /// <param name="action">A ação assíncrona de configuração e execução da aplicação.</param>
    public static async Task RunWithSerilogAsync(Func<Task> action)
    {
        Log.Logger = SerilogConfiguration.CreateBootstrapLogger();

        try
        {
            Log.Information("🚀 Iniciando aplicação...");
            await action();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "☠️ Aplicação falhou ao iniciar");
            throw;
        }
        finally
        {
            Log.Information("🛑 Aplicação encerrada");
            await Log.CloseAndFlushAsync();
        }
    }
}
