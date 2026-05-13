using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public record ListUsersQuery(int Page = 1, int PageSize = 10) : IRequest<ListUsersResult>;

public sealed class ListUsersResult
{
    public IReadOnlyList<GetUserResult> Items { get; init; } = Array.Empty<GetUserResult>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}
