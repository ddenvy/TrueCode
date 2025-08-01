using FluentValidation;

namespace TrueCode.User.Application.Commands.UpdateFavoriteCurrencies;

/// <summary>
/// Валидатор для команды обновления избранных валют
/// </summary>
public class UpdateFavoriteCurrenciesCommandValidator : AbstractValidator<UpdateFavoriteCurrenciesCommand>
{
    public UpdateFavoriteCurrenciesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Идентификатор пользователя обязателен");
        
        RuleFor(x => x.CurrencyCodes)
            .NotNull()
            .WithMessage("Список валют не может быть null");
        
        RuleForEach(x => x.CurrencyCodes)
            .NotEmpty()
            .WithMessage("Код валюты не может быть пустым")
            .Length(3)
            .WithMessage("Код валюты должен содержать 3 символа")
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("Код валюты должен содержать только заглавные буквы");
    }
}