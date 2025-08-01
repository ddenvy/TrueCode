using FluentValidation;

namespace TrueCode.User.Application.Commands.RegisterUser;

/// <summary>
/// Валидатор для команды регистрации пользователя
/// </summary>
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Имя пользователя обязательно")
            .MinimumLength(3)
            .WithMessage("Имя пользователя должно содержать минимум 3 символа")
            .MaximumLength(50)
            .WithMessage("Имя пользователя не должно превышать 50 символов")
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Имя пользователя может содержать только буквы, цифры и символ подчеркивания");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Пароль обязателен")
            .MinimumLength(6)
            .WithMessage("Пароль должен содержать минимум 6 символов")
            .MaximumLength(100)
            .WithMessage("Пароль не должен превышать 100 символов");
    }
}