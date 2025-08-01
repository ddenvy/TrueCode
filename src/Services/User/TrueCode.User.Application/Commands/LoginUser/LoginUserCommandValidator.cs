using FluentValidation;

namespace TrueCode.User.Application.Commands.LoginUser;

/// <summary>
/// Валидатор для команды входа пользователя
/// </summary>
public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Имя пользователя обязательно");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Пароль обязателен");
    }
}