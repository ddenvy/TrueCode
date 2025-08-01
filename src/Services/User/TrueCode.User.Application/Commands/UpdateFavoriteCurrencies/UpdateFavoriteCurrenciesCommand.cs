using MediatR;
using TrueCode.User.Application.DTOs;

namespace TrueCode.User.Application.Commands.UpdateFavoriteCurrencies;

/// <summary>
/// Команда для обновления избранных валют пользователя
/// </summary>
public class UpdateFavoriteCurrenciesCommand : IRequest<UserDto>
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Список кодов валют
    /// </summary>
    public List<string> CurrencyCodes { get; set; } = new();
    
    public UpdateFavoriteCurrenciesCommand(Guid userId, List<string> currencyCodes)
    {
        UserId = userId;
        CurrencyCodes = currencyCodes;
    }
}