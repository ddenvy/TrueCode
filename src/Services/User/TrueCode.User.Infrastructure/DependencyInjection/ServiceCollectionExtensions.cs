using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueCode.Shared.Domain.Interfaces;
using TrueCode.Shared.Infrastructure.UnitOfWork;
using TrueCode.User.Domain.Interfaces;
using TrueCode.User.Infrastructure.Data;
using TrueCode.User.Infrastructure.Repositories;

namespace TrueCode.User.Infrastructure.DependencyInjection;

/// <summary>
/// Расширения для регистрации сервисов инфраструктуры пользователей
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавляет сервисы инфраструктуры пользователей
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddUserInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Регистрация DbContext
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Регистрация репозиториев
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Регистрация Unit of Work
        services.AddScoped<IUnitOfWork>(provider => 
            new UnitOfWork(provider.GetRequiredService<UserDbContext>()));

        return services;
    }
}