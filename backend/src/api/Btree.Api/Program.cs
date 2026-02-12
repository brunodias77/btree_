using BuildingBlocks.Application;
using BuildingBlocks.Infrastructure;
using BuildingBlocks.Security;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions;
using BuildingBlocks.Web.Logging;
using Serilog;
using Users.Infrastructure;


SerilogApplicationRunner.RunWithSerilog(() =>
{
    var builder = WebApplication.CreateBuilder(args);

    // ============================================
    // Serilog - Configuração via BuildingBlocks.Web
    // ============================================
    builder.Services.AddSerilogLogging(builder.Configuration, "Btree.Api");

    // ============================================
    // Configuração de Serviços
    // ============================================

    // BuildingBlocks.Application - MediatR, FluentValidation, Behaviors
    builder.Services.AddApplicationServices(typeof(Program).Assembly);

    // BuildingBlocks.Infrastructure - Interceptors, Event Bus, Outbox, Cache
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // BuildingBlocks.Security - JWT, Autenticação, Autorização
    builder.Services.AddSecurityServices(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddPermissionAuthorization();

    // BuildingBlocks.Web - Controllers, Filtros, CORS, Swagger, Health Checks
    builder.Services.AddWebServices();


    // ============================================
    // Modules
    // ============================================
    // builder.Services.AddUsersApplication();
     builder.Services.AddUsersInfrastructure(builder.Configuration);
    //
    // builder.Services.AddCatalogApplication();
    // builder.Services.AddCatalogInfrastructure(builder.Configuration);
    //
    // builder.Services.AddCartsApplication();

    // Background Jobs
    builder.Services.AddOutboxProcessorJob();

    var app = builder.Build();

    // ============================================
    // Pipeline de Middleware
    // ============================================

    // CORS deve ser PRIMEIRO para preflight requests funcionarem
    if (app.Environment.IsDevelopment())
    {
        app.UseDevelopmentCors();
    }
    
    // Serilog Request Logging
    app.UseSerilogRequestLoggingMiddleware();

    // BuildingBlocks.Web - Correlation ID, Exception Handling
    app.UseBuildingBlocksMiddlewares();
    
    // Swagger (apenas em desenvolvimento)
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerWithUI();
    }
    
    app.UseHttpsRedirection();

    // Servir arquivos estáticos (wwwroot)
    app.UseStaticFiles();

    // Autenticação e Autorização
    app.UseAuthentication();
    app.UseAuthorization();

    // Health Checks
    app.UseHealthChecksEndpoints();

    // Map Controllers
    app.MapControllers();

    Log.Information("🚀 Aplicação iniciada com sucesso");

    app.Run();

});


