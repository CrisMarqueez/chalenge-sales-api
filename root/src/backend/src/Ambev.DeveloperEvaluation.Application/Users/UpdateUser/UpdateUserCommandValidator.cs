using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Email).SetValidator(new EmailValidator());
        RuleFor(x => x.Username).NotEmpty().Length(3, 50);
        RuleFor(x => x.Phone).Matches(@"^\+?[1-9]\d{1,14}$");
        RuleFor(x => x.Status).NotEqual(UserStatus.Unknown);
        RuleFor(x => x.Role).NotEqual(UserRole.None);
        When(x => !string.IsNullOrWhiteSpace(x.NewPassword), () =>
        {
            RuleFor(x => x.NewPassword!).SetValidator(new PasswordValidator());
        });
    }
}
