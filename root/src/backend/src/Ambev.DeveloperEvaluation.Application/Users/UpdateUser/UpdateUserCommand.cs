using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

public sealed class UpdateUserCommand : IRequest<GetUserResult>
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }

    /// <summary>When set, replaces the password (plain text; will be hashed).</summary>
    public string? NewPassword { get; set; }
}
