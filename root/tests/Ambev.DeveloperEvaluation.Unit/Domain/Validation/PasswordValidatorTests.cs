using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Unit tests for <see cref="PasswordValidator"/> (non-empty, minimum length 6).
/// </summary>
public class PasswordValidatorTests
{
    private readonly PasswordValidator _validator = new();

    [Theory(DisplayName = "Passwords with at least 6 characters should pass")]
    [InlineData("123456")]
    [InlineData("password")]
    [InlineData("Test@123")]
    public void Given_PasswordWithMinimumLength_When_Validated_Then_ShouldNotHaveErrors(
        string password
    )
    {
        var result = _validator.TestValidate(password);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty password should fail validation")]
    public void Given_EmptyPassword_When_Validated_Then_ShouldHaveError()
    {
        var result = _validator.TestValidate(string.Empty);
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Theory(DisplayName = "Password shorter than 6 characters should fail validation")]
    [InlineData("12345")]
    [InlineData("abc")]
    [InlineData("12")]
    public void Given_PasswordShorterThanMinimum_When_Validated_Then_ShouldHaveError(
        string password
    )
    {
        var result = _validator.TestValidate(password);
        result
            .ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Password must be at least 6 characters.");
    }
}
