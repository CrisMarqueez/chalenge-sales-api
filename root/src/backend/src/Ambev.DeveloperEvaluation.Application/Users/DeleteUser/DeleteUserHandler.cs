using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Application.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Users.DeleteUser;

/// <summary>
/// Handler for processing DeleteUserCommand requests
/// </summary>
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentPrincipal _currentPrincipal;

    /// <summary>
    /// Initializes a new instance of DeleteUserHandler
    /// </summary>
    public DeleteUserHandler(IUserRepository userRepository, ICurrentPrincipal currentPrincipal)
    {
        _userRepository = userRepository;
        _currentPrincipal = currentPrincipal;
    }

    /// <summary>
    /// Handles the DeleteUserCommand request
    /// </summary>
    /// <param name="request">The DeleteUser command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteUserValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (_currentPrincipal.UserId is { } selfId && selfId == request.Id)
            throw new InvalidOperationException("Cannot delete your own user account.");

        var success = await _userRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
            throw new KeyNotFoundException($"User with ID {request.Id} not found");

        return new DeleteUserResponse { Success = true };
    }
}
