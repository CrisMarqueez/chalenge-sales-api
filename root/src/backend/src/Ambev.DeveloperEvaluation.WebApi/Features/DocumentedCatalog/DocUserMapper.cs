using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.WebApi.Features.DocumentedCatalog;

/// <summary>
/// Projeta <see cref="User"/> (domínio) para o DTO público da <c>users-api.md</c>.
/// </summary>
public static class DocUserMapper
{
    private static UserAddressDto EmptyAddress() => new()
    {
        City = string.Empty,
        Street = string.Empty,
        Number = 0,
        Zipcode = string.Empty,
        Geolocation = new GeoDto { Lat = string.Empty, Long = string.Empty },
    };

    public static UserDto FromDomainUser(User u, int catalogId, string passwordDisplay)
    {
        var parts = u.Username.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var first = parts.Length > 0 ? parts[0] : u.Username;
        var last = parts.Length > 1 ? parts[1] : string.Empty;

        return new UserDto
        {
            Id = catalogId,
            DomainId = u.Id,
            Email = u.Email,
            Username = u.Username,
            Password = passwordDisplay,
            Name = new UserNameDto { Firstname = first, Lastname = last },
            Address = EmptyAddress(),
            Phone = u.Phone,
            Status = u.Status.ToString(),
            Role = u.Role.ToString(),
        };
    }
}
