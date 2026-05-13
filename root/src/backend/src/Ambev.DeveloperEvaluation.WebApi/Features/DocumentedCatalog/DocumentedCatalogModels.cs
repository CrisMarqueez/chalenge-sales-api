using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.DocumentedCatalog;

public sealed class ProductRatingDto
{
    public decimal Rate { get; set; }
    public int Count { get; set; }
}

public sealed class ProductDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public ProductRatingDto Rating { get; set; } = new();
}

public sealed class CartLineDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public sealed class CartDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Date { get; set; } = string.Empty;
    public List<CartLineDto> Products { get; set; } = [];
}

public sealed class GeoDto
{
    public string Lat { get; set; } = string.Empty;
    public string Long { get; set; } = string.Empty;
}

public sealed class UserAddressDto
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public GeoDto Geolocation { get; set; } = new();
}

public sealed class UserNameDto
{
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
}

public sealed class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserNameDto Name { get; set; } = new();
    public UserAddressDto Address { get; set; } = new();
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string Role { get; set; } = "Customer";

    /// <summary>Identificador do usuário persistido (domínio Ambev). Não faz parte do contrato JSON da doc.</summary>
    [JsonIgnore]
    public Guid? DomainId { get; set; }
}

public sealed class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class LoginTokenResponseDto
{
    public string Token { get; set; } = string.Empty;
}
