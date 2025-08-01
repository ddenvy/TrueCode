using System.Globalization;
using System.Xml.Linq;
using TrueCode.Finance.Application.DTOs;

namespace TrueCode.CurrencyUpdater.Services;

/// <summary>
/// Сервис для получения курсов валют с сайта ЦБ РФ
/// </summary>
public class CbrCurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CbrCurrencyService> _logger;
    private const string CBR_URL = "http://www.cbr.ru/scripts/XML_daily.asp";

    public CbrCurrencyService(HttpClient httpClient, ILogger<CbrCurrencyService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Получает актуальные курсы валют с сайта ЦБ РФ
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список курсов валют</returns>
    public async Task<List<UpdateCurrencyRateDto>> GetCurrencyRatesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Начинаем загрузку курсов валют с ЦБ РФ: {Url}", CBR_URL);

            // Загружаем XML данные с сайта ЦБ РФ
            var xmlContent = await _httpClient.GetStringAsync(CBR_URL, cancellationToken);
            
            if (string.IsNullOrEmpty(xmlContent))
            {
                _logger.LogError("Получен пустой ответ от ЦБ РФ");
                return new List<UpdateCurrencyRateDto>();
            }

            _logger.LogDebug("Получен XML размером {Size} символов", xmlContent.Length);

            // Парсим XML
            var currencies = ParseCurrencyXml(xmlContent);
            
            _logger.LogInformation("Успешно загружено {Count} курсов валют с ЦБ РФ", currencies.Count);
            
            return currencies;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ошибка HTTP при загрузке курсов валют с ЦБ РФ");
            throw new InvalidOperationException("Не удалось загрузить курсы валют с ЦБ РФ", ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Загрузка курсов валют была отменена");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Неожиданная ошибка при загрузке курсов валют");
            throw;
        }
    }

    /// <summary>
    /// Парсит XML с курсами валют от ЦБ РФ
    /// </summary>
    /// <param name="xmlContent">XML контент</param>
    /// <returns>Список курсов валют</returns>
    private List<UpdateCurrencyRateDto> ParseCurrencyXml(string xmlContent)
    {
        try
        {
            var doc = XDocument.Parse(xmlContent);
            var currencies = new List<UpdateCurrencyRateDto>();

            var valuteElements = doc.Descendants("Valute");
            
            foreach (var valute in valuteElements)
            {
                try
                {
                    var charCode = valute.Element("CharCode")?.Value;
                    var nominal = valute.Element("Nominal")?.Value;
                    var value = valute.Element("Value")?.Value;

                    if (string.IsNullOrEmpty(charCode) || 
                        string.IsNullOrEmpty(nominal) || 
                        string.IsNullOrEmpty(value))
                    {
                        _logger.LogWarning("Пропущена валюта с неполными данными: CharCode={CharCode}, Nominal={Nominal}, Value={Value}", 
                            charCode, nominal, value);
                        continue;
                    }

                    // Парсим номинал
                    if (!int.TryParse(nominal, out var nominalInt))
                    {
                        _logger.LogWarning("Не удалось распарсить номинал для валюты {CharCode}: {Nominal}", charCode, nominal);
                        continue;
                    }

                    // Парсим курс (заменяем запятую на точку для корректного парсинга)
                    var normalizedValue = value.Replace(',', '.');
                    if (!decimal.TryParse(normalizedValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var rate))
                    {
                        _logger.LogWarning("Не удалось распарсить курс для валюты {CharCode}: {Value}", charCode, value);
                        continue;
                    }

                    // Приводим курс к рублю за единицу валюты
                    var ratePerUnit = rate / nominalInt;

                    currencies.Add(new UpdateCurrencyRateDto(charCode, ratePerUnit));
                    
                    _logger.LogDebug("Обработана валюта: {CharCode} = {Rate} руб. (номинал: {Nominal})", 
                        charCode, ratePerUnit, nominalInt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке валюты: {ValuteXml}", valute.ToString());
                }
            }

            return currencies;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при парсинге XML с курсами валют");
            throw new InvalidOperationException("Не удалось распарсить XML с курсами валют", ex);
        }
    }

    /// <summary>
    /// Получает курсы валют для конкретной даты
    /// </summary>
    /// <param name="date">Дата для получения курсов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список курсов валют</returns>
    public async Task<List<UpdateCurrencyRateDto>> GetCurrencyRatesForDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        try
        {
            var dateString = date.ToString("dd/MM/yyyy");
            var urlWithDate = $"{CBR_URL}?date_req={dateString}";
            
            _logger.LogInformation("Загружаем курсы валют на дату {Date} с URL: {Url}", date.ToShortDateString(), urlWithDate);

            var xmlContent = await _httpClient.GetStringAsync(urlWithDate, cancellationToken);
            
            if (string.IsNullOrEmpty(xmlContent))
            {
                _logger.LogError("Получен пустой ответ от ЦБ РФ для даты {Date}", date.ToShortDateString());
                return new List<UpdateCurrencyRateDto>();
            }

            var currencies = ParseCurrencyXml(xmlContent);
            
            _logger.LogInformation("Успешно загружено {Count} курсов валют на дату {Date}", currencies.Count, date.ToShortDateString());
            
            return currencies;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке курсов валют на дату {Date}", date.ToShortDateString());
            throw;
        }
    }
}