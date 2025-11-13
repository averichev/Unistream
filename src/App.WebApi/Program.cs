using App.Data;
using App.Domain.Services;
using App.WebApi.Infrastructure;
using App.WebApi.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("Logs/app-.log", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateItemRequestValidator>();
    builder.Services.AddProblemDetails();

    // Configure Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "App API", 
            Version = "v1",
            Description = "Clean Architecture API with CRUD operations for Items"
        });
    });

    var connectionString = builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' was not found.");

    builder.Services.AddDataAccess(options => options.UseSqlServer(connectionString));

    builder.Services.AddScoped<IItemService, ItemService>();

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>();

    var app = builder.Build();

    await app.ApplyMigrationsAsync().ConfigureAwait(false);

    // Configure Swagger middleware
    if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "App API v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseSerilogRequestLogging();
    app.UseGlobalExceptionHandling();

    app.MapControllers();
    app.MapHealthChecks("/health");

    await app.RunAsync().ConfigureAwait(false);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
