using App.Data.Repositories;
using App.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDbContext)
    {
        services.AddDbContext<AppDbContext>(configureDbContext);
        services.AddScoped<IItemRepository, ItemRepository>();
        return services;
    }
}
