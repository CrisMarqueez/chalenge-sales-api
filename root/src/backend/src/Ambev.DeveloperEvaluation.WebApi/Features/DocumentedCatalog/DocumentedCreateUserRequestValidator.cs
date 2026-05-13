using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.DocumentedCatalog;

/// <summary>
/// Validação do corpo de <c>POST /users</c> conforme <c>.doc/users-api.md</c>.
/// </summary>
public sealed class DocumentedCreateUserRequestValidator : AbstractValidator<UserDto>
{
    public DocumentedCreateUserRequestValidator()
    {
        RuleFor(u => u.Email).NotEmpty().SetValidator(new EmailValidator());
        RuleFor(u => u.Username).NotEmpty().Length(3, 50);
        RuleFor(u => u.Password).SetValidator(new PasswordValidator());
        RuleFor(u => u.Phone).NotEmpty();

        RuleFor(u => u.Name).NotNull();
        RuleFor(u => u.Name!.Firstname).NotEmpty();
        RuleFor(u => u.Name!.Lastname).NotEmpty();

        RuleFor(u => u.Address).NotNull();
        RuleFor(u => u.Address!.City).NotEmpty();
        RuleFor(u => u.Address!.Street).NotEmpty();
        RuleFor(u => u.Address!.Zipcode).NotEmpty();
        RuleFor(u => u.Address!.Geolocation).NotNull();

        RuleFor(u => u.Status)
            .Must(s => Enum.TryParse<UserStatus>(s, ignoreCase: true, out var st) && st != UserStatus.Unknown)
            .WithMessage("status must be Active, Inactive, or Suspended.");

        RuleFor(u => u.Role)
            .Must(r => Enum.TryParse<UserRole>(r, ignoreCase: true, out var ro) && ro != UserRole.None)
            .WithMessage("role must be Customer, Manager, or Admin.");
    }
}
