using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public sealed class ListUsersHandler : IRequestHandler<ListUsersQuery, ListUsersResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ListUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ListUsersResult> Handle(
        ListUsersQuery request,
        CancellationToken cancellationToken)
    {
        var validator = new ListUsersQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var (items, total) = await _userRepository.ListPagedAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        return new ListUsersResult
        {
            Items = _mapper.Map<IReadOnlyList<GetUserResult>>(items),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize,
        };
    }
}
