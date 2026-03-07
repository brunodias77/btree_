using Shared.Security;
using Shared.Web;
using Shared.Outbox;
using Shared.Application;
using Shared.Infrastructure;
using Users.Infrastructure;
using Users.Application;
using Catalog.Infrastructure;
using Catalog.Application;
using Btree.Api.Extensions;
using Shared.Web.Extensions;
using Shared.Web.Logging;

await SerilogApplicationRunner.RunWithSerilog(async () =>
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Serviços compartilhados ──
    builder.Services
        .AddApplicationServices()
        .AddInfrastructureServices()
        .AddWebServices(builder.Configuration, "Btree.API")
        .AddOutBoxServices(builder.Configuration)
        .AddUsersInfrastructure(builder.Configuration)
        .AddUsersApplication()
        .AddSecurityServices(builder.Configuration)
        .AddCatalogInfrastructure(builder.Configuration)
        .AddCatalogApplication();

    // ── File Storage ──
    var uploadsPath = Path.Combine(builder.Environment.WebRootPath ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot"), "uploads");
    builder.Services.AddSingleton<Shared.Application.Abstractions.IFileStorageService>(
        new Shared.Infrastructure.Services.LocalFileStorageService(uploadsPath));

    // ── CORS (ajustar origens conforme necessário) ──
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // ── Aplica Migrations automaticamente no Startup ──
    await app.ApplyMigrationsAsync();

    // ── Pipeline HTTP ──
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors();

    // Serve arquivos estáticos (imagens de uploads em wwwroot/uploads)
    app.UseStaticFiles();

    // Middlewares compartilhados (CorrelationId → Security Headers → Exception → Logging)
    app.UseSharedMiddlewares();

    // Serilog request logging (enriquecido com CorrelationId, UserId, etc.)
    app.UseSerilogRequestLoggingMiddleware();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
});