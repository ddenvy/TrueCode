using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueCode.Finance.Application.Interfaces;
using TrueCode.Finance.Domain.Interfaces;
using TrueCode.Finance.Infrastructure.Data;
using TrueCode.Finance.Infrastructure.Repositories;
using TrueCode.Finance.Infrastructure.Services;

namespace TrueCode.Finance.Infrastructure.DependencyInjection;

/// <summary>
/// Расширения для регистрации сервисов инфраструктурного слоя Finance
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавляет сервисы инфраструктурного слоя Finance
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация приложения</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddFinanceInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Добавляем DbContext
        services.AddDbContext<FinanceDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Строка подключения DefaultConnection не найдена");
                
            options.UseNpgsql(connectionString);
        });

        // Регистрируем репозитории
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();

        // Регистрируем сервисы
        services.AddScoped<IUserService, GrpcUserService>();

        return services;
    }
} 