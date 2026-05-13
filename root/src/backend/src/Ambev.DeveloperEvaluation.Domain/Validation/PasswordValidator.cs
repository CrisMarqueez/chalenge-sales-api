using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(password => password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.");
    }
}