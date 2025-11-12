using App.Data;
using App.Domain.Services;
using App.WebApi.Infrastructure;
using App.WebApi.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
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

    var connectionString = builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' was not found.");

    builder.Services.AddDataAccess(options => options.UseSqlServer(connectionString));

    builder.Services.AddScoped<IItemService, ItemService>();

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>();

    var app = builder.Build();

    await app.ApplyMigrationsAsync().ConfigureAwait(false);

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
