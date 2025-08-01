using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrueCode.User.Infrastructure.Data;
using TrueCode.Finance.Infrastructure.Data;

var builder = Host.CreateApplicationBuilder(args);

// Настройка конфигурации
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

// Добавляем логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Добавляем DbContext для обеих баз данных
var userConnectionString = builder.Configuration.GetConnectionString("UserConnection") 
    ?? throw new InvalidOperationException("User connection string не настроена");
var financeConnectionString = builder.Configuration.GetConnectionString("FinanceConnection") 
    ?? throw new InvalidOperationException("Finance connection string не настроена");

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(userConnectionString));

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseNpgsql(financeConnectionString));

// Создаем хост
var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("=== TrueCode Database Migrator ===");
logger.LogInformation("Начинаем выполнение миграций баз данных...");

try
{
    // Выполняем миграции для User DB
    logger.LogInformation("Выполняем миграции для User Database...");
    using (var scope = host.Services.CreateScope())
    {
        var userContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        
        logger.LogInformation("Проверяем подключение к User Database...");
        await userContext.Database.CanConnectAsync();
        
        logger.LogInformation("Применяем миграции для User Database...");
        await userContext.Database.MigrateAsync();
        
        logger.LogInformation("✅ Миграции User Database выполнены успешно");
    }

    // Выполняем миграции для Finance DB
    logger.LogInformation("Выполняем миграции для Finance Database...");
    using (var scope = host.Services.CreateScope())
    {
        var financeContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
        
        logger.LogInformation("Проверяем подключение к Finance Database...");
        await financeContext.Database.CanConnectAsync();
        
        logger.LogInformation("Применяем миграции для Finance Database...");
        await financeContext.Database.MigrateAsync();
        
        logger.LogInformation("✅ Миграции Finance Database выполнены успешно");
    }

    logger.LogInformation("🎉 Все миграции выполнены успешно!");
    
    // Проверяем статус миграций
    logger.LogInformation("Проверяем статус миграций...");
    using (var scope = host.Services.CreateScope())
    {
        var userContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        var financeContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

        var userMigrations = await userContext.Database.GetAppliedMigrationsAsync();
        var financeMigrations = await financeContext.Database.GetAppliedMigrationsAsync();

        logger.LogInformation("User Database - применено миграций: {Count}", userMigrations.Count());
        foreach (var migration in userMigrations)
        {
            logger.LogInformation("  - {Migration}", migration);
        }

        logger.LogInformation("Finance Database - применено миграций: {Count}", financeMigrations.Count());
        foreach (var migration in financeMigrations)
        {
            logger.LogInformation("  - {Migration}", migration);
        }
    }

    Environment.Exit(0);
}
catch (Exception ex)
{
    logger.LogCritical(ex, "❌ Критическая ошибка при выполнении миграций");
    Environment.Exit(1);
}
