using MediatR;
using TrueCode.CurrencyUpdater.Services;
using TrueCode.Finance.Application.Commands.UpdateCurrencyRates;

namespace TrueCode.CurrencyUpdater;

/// <summary>
/// Фоновый сервис для обновления курсов валют с ЦБ РФ
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _updateInterval;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        // Читаем интервал обновления из конфигурации (по умолчанию каждые 4 часа)
        var intervalMinutes = _configuration.GetValue<int>("CurrencyUpdate:IntervalMinutes", 240);
        _updateInterval = TimeSpan.FromMinutes(intervalMinutes);
        
        _logger.LogInformation("Фоновый сервис обновления валют настроен с интервалом: {Interval}", _updateInterval);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Фоновый сервис обновления курсов валют запущен");

        // Выполняем первое обновление сразу при запуске
        await UpdateCurrenciesAsync(stoppingToken);

        // Запускаем периодическое обновление
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Ожидание следующего обновления курсов валют через {Interval}", _updateInterval);
                await Task.Delay(_updateInterval, stoppingToken);
                
                if (!stoppingToken.IsCancellationRequested)
                {
                    await UpdateCurrenciesAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Фоновый сервис обновления валют был остановлен");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка в фоновом сервисе обновления валют");
                
                // Ждем немного перед повторной попыткой
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        _logger.LogInformation("Фоновый сервис обновления курсов валют завершен");
    }

    /// <summary>
    /// Выполняет обновление курсов валют
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    private async Task UpdateCurrenciesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var cbrService = scope.ServiceProvider.GetRequiredService<CbrCurrencyService>();

        try
        {
            _logger.LogInformation("Начинается обновление курсов валют в {Time}", DateTime.UtcNow);

            // Получаем курсы валют с ЦБ РФ
            var currencyRates = await cbrService.GetCurrencyRatesAsync(cancellationToken);

            if (!currencyRates.Any())
            {
                _logger.LogWarning("Не получено ни одного курса валют с ЦБ РФ");
                return;
            }

            _logger.LogInformation("Получено {Count} курсов валют с ЦБ РФ", currencyRates.Count);

            // Отправляем команду на обновление курсов в базе данных
            var command = new UpdateCurrencyRatesCommand(currencyRates);
            var updatedCount = await mediator.Send(command, cancellationToken);

            _logger.LogInformation("Обновление курсов валют завершено успешно. Обновлено/создано: {Count} записей", updatedCount);

            // Логируем некоторые примеры курсов для отладки
            var sampleCurrencies = currencyRates.Take(5);
            foreach (var currency in sampleCurrencies)
            {
                _logger.LogDebug("Обновлен курс: {Currency} = {Rate:F4} руб.", currency.Name, currency.Rate);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Обновление курсов валют было отменено");
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ошибка сети при обновлении курсов валют. Повторная попытка будет выполнена позже");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Ошибка при обработке данных от ЦБ РФ");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при обновлении курсов валют");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Получен сигнал остановки фонового сервиса обновления валют");
        await base.StopAsync(cancellationToken);
    }
}
