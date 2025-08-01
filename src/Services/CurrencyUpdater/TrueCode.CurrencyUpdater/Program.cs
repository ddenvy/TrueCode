using MediatR;
using TrueCode.CurrencyUpdater;
using TrueCode.CurrencyUpdater.Services;
using TrueCode.Finance.Application.Commands.UpdateCurrencyRates;
using TrueCode.Finance.Infrastructure.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

// Добавляем Worker сервис
builder.Services.AddHostedService<Worker>();

// Добавляем HttpClient для работы с ЦБ РФ
builder.Services.AddHttpClient<CbrCurrencyService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "TrueCode-CurrencyUpdater/1.0");
});

// Добавляем MediatR для отправки команд
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateCurrencyRatesCommand).Assembly));

// Добавляем инфраструктуру Finance для работы с базой данных
builder.Services.AddFinanceInfrastructure(builder.Configuration);

// Добавляем сервис для работы с ЦБ РФ
builder.Services.AddScoped<CbrCurrencyService>();

// Настройка логирования
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Устанавливаем уровень логирования
builder.Logging.SetMinimumLevel(LogLevel.Information);

var host = builder.Build();

// Логируем информацию о запуске
var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("=== TrueCode Currency Updater Service ===");
logger.LogInformation("Сервис обновления курсов валют запускается...");
logger.LogInformation("Конфигурация:");
logger.LogInformation("- Интервал обновления: {Interval} минут", 
    builder.Configuration.GetValue<int>("CurrencyUpdate:IntervalMinutes", 240));
logger.LogInformation("- Строка подключения к БД: {ConnectionString}", 
    builder.Configuration.GetConnectionString("DefaultConnection")?.Substring(0, 50) + "...");

try
{
    await host.RunAsync();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Критическая ошибка при запуске сервиса обновления валют");
    throw;
}
