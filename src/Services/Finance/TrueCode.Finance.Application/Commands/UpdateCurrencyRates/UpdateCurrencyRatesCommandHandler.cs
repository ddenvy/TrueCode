using MediatR;
using Microsoft.Extensions.Logging;
using TrueCode.Finance.Domain.Entities;
using TrueCode.Finance.Domain.Interfaces;

namespace TrueCode.Finance.Application.Commands.UpdateCurrencyRates;

/// <summary>
/// Обработчик команды обновления курсов валют
/// </summary>
public class UpdateCurrencyRatesCommandHandler : IRequestHandler<UpdateCurrencyRatesCommand, int>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ILogger<UpdateCurrencyRatesCommandHandler> _logger;

    public UpdateCurrencyRatesCommandHandler(
        ICurrencyRepository currencyRepository,
        ILogger<UpdateCurrencyRatesCommandHandler> logger)
    {
        _currencyRepository = currencyRepository;
        _logger = logger;
    }

    /// <summary>
    /// Обрабатывает команду обновления курсов валют
    /// </summary>
    /// <param name="request">Команда с данными валют</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество обновленных валют</returns>
    public async Task<int> Handle(UpdateCurrencyRatesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Начало обновления курсов валют. Количество валют: {Count}", request.Currencies.Count);

            var updatedCount = 0;
            var newCurrencies = new List<Currency>();

            foreach (var currencyUpdate in request.Currencies)
            {
                // Ищем существующую валюту
                var existingCurrency = await _currencyRepository.GetByNameAsync(currencyUpdate.Name, cancellationToken);
                
                if (existingCurrency != null)
                {
                    // Обновляем курс существующей валюты
                    existingCurrency.UpdateRate(currencyUpdate.Rate);
                    await _currencyRepository.UpdateAsync(existingCurrency, cancellationToken);
                    updatedCount++;
                    
                    _logger.LogDebug("Обновлен курс валюты {Currency}: {Rate}", currencyUpdate.Name, currencyUpdate.Rate);
                }
                else
                {
                    // Создаем новую валюту с базовой информацией
                    var newCurrency = Currency.Create(
                        currencyUpdate.Name,
                        currencyUpdate.Rate,
                        currencyUpdate.Name, // Используем код как полное название пока не получим данные из ЦБ
                        1
                    );
                    
                    newCurrencies.Add(newCurrency);
                    _logger.LogDebug("Создана новая валюта {Currency}: {Rate}", currencyUpdate.Name, currencyUpdate.Rate);
                }
            }

            // Добавляем новые валюты массово
            if (newCurrencies.Any())
            {
                await _currencyRepository.AddRangeAsync(newCurrencies, cancellationToken);
                updatedCount += newCurrencies.Count;
            }

            // Сохраняем изменения
            await _currencyRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Обновление курсов валют завершено. Обновлено/создано: {Count} валют", updatedCount);
            return updatedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении курсов валют");
            throw;
        }
    }
} 