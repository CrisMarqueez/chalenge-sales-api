using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

public sealed class UpdateUserHandler : IRequestHandler<UpdateUserCommand, GetUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateUserHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<GetUserResult> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new UpdateUserCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await _userRepository.GetByIdAsync(command.Id, tracking: true, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {command.Id} not found");

        var email = command.Email.Trim();
        var byEmail = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (byEmail != null && byEmail.Id != command.Id)
            throw new InvalidOperationException($"User with email {email} already exists");

        var username = command.Username.Trim();
        var byUsername = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (byUsername != null && byUsername.Id != command.Id)
            throw new InvalidOperationException($"Username {username} is already taken");

        user.Username = username;
        user.Email = email;
        user.Phone = command.Phone.Trim();
        user.Role = command.Role;
        user.Status = command.Status;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(command.NewPassword))
            user.Password = _passwordHasher.HashPassword(command.NewPassword.Trim());

        await _userRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<GetUserResult>(user);
    }
}
