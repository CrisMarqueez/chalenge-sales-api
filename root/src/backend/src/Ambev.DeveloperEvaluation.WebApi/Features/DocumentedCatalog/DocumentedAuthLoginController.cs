using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.DocumentedCatalog;

[ApiController]
[Route("auth")]
public sealed class DocumentedAuthLoginController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserRepository _users;

    public DocumentedAuthLoginController(IMediator mediator, IUserRepository users)
    {
        _mediator = mediator;
        _users = users;
    }

    /// <summary>Contrato <c>.doc/auth-api.md</c>: POST /auth/login com username e password; resposta apenas token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginTokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto body, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(body.Username) || string.IsNullOrWhiteSpace(body.Password))
        {
            return BadRequest(new DocumentedErrorResponse
            {
                Type = "ValidationError",
                Error = "Invalid input data",
                Detail = "The 'username' and 'password' fields are required",
            });
        }

        var user = await _users.GetByEmailAsync(body.Username.Trim(), cancellationToken)
                   ?? await _users.GetByUsernameAsync(body.Username.Trim(), cancellationToken);

        if (user == null)
        {
            return Unauthorized(new DocumentedErrorResponse
            {
                Type = "AuthenticationError",
                Error = "Invalid authentication token",
                Detail = "Invalid credentials",
            });
        }

        var result = await _mediator.Send(
            new AuthenticateUserCommand { Email = user.Email, Password = body.Password },
            cancellationToken);
        return Ok(new LoginTokenResponseDto { Token = result.Token });
    }
}
