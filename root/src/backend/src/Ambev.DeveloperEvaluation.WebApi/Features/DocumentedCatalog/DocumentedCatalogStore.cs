using System.Globalization;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.DocumentedCatalog;

/// <summary>
/// Protótipo em memória para os contratos de <c>.doc/products-api.md</c>, <c>carts-api.md</c> e <c>users-api.md</c>.
/// </summary>
public sealed class DocumentedCatalogStore
{
    private readonly object _lock = new();
    private readonly List<ProductDto> _products = [];
    private readonly List<CartDto> _carts = [];
    private readonly List<UserDto> _users = [];
    private int _nextProductId = 1;
    private int _nextCartId = 1;
    private int _nextUserId = 1;

    public DocumentedCatalogStore()
    {
        Seed();
    }

    private void Seed()
    {
        _products.Add(new ProductDto
        {
            Id = _nextProductId++,
            Title = "Fjallraven - Foldsack No. 1 Backpack, Fits 15 Laptops",
            Price = 109.95m,
            Description = "Your perfect pack for everyday use and walks in the forest.",
            Category = "men's clothing",
            Image = "https://fakestoreapi.com/img/81fPKd-2AYL._AC_SL1500_.jpg",
            Rating = new ProductRatingDto { Rate = 3.9m, Count = 120 },
        });
        _products.Add(new ProductDto
        {
            Id = _nextProductId++,
            Title = "Mens Casual Premium Slim Fit T-Shirts",
            Price = 22.3m,
            Description = "Slim-fitting style.",
            Category = "men's clothing",
            Image = "https://fakestoreapi.com/img/71-3HjGNDUL._AC_SY879._SX._UX._SY._UY_.jpg",
            Rating = new ProductRatingDto { Rate = 4.1m, Count = 259 },
        });
        _products.Add(new ProductDto
        {
            Id = _nextProductId++,
            Title = "John Hardy Women's Legends Naga Gold & Silver Dragon Station Chain Bracelet",
            Price = 695m,
            Description = "From our Legends Collection.",
            Category = "jewelery",
            Image = "https://fakestoreapi.com/img/71pWzhdJNwL._AC_UL640_SR640,430_.jpg",
            Rating = new ProductRatingDto { Rate = 4.6m, Count = 400 },
        });

        _users.Add(CloneUser(new UserDto
        {
            Id = _nextUserId++,
            Email = "john@example.com",
            Username = "johnd",
            Password = "m38rmF$",
            Name = new UserNameDto { Firstname = "John", Lastname = "Doe" },
            Address = new UserAddressDto
            {
                City = "kilcoole",
                Street = "7835 new road",
                Number = 3,
                Zipcode = "12926-3874",
                Geolocation = new GeoDto { Lat = "-37.3159", Long = "81.1496" },
            },
            Phone = "1-570-236-7033",
            Status = "Active",
            Role = "Customer",
        }));

        _carts.Add(new CartDto
        {
            Id = _nextCartId++,
            UserId = 1,
            Date = "2020-10-10",
            Products =
            [
                new CartLineDto { ProductId = 1, Quantity = 2 },
                new CartLineDto { ProductId = 2, Quantity = 1 },
            ],
        });
    }

    private static UserDto CloneUser(UserDto u) => new()
    {
        Id = u.Id,
        Email = u.Email,
        Username = u.Username,
        Password = u.Password,
        Name = new UserNameDto { Firstname = u.Name.Firstname, Lastname = u.Name.Lastname },
        Address = new UserAddressDto
        {
            City = u.Address.City,
            Street = u.Address.Street,
            Number = u.Address.Number,
            Zipcode = u.Address.Zipcode,
            Geolocation = new GeoDto { Lat = u.Address.Geolocation.Lat, Long = u.Address.Geolocation.Long },
        },
        Phone = u.Phone,
        Status = u.Status,
        Role = u.Role,
        DomainId = u.DomainId,
    };

    public IReadOnlyList<string> GetProductCategories()
    {
        lock (_lock)
        {
            return _products.Select(p => p.Category).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(c => c).ToList();
        }
    }

    public DocumentedPagedListResponse<ProductDto> ListProducts(
        int page,
        int size,
        string? order,
        string? title,
        string? category,
        decimal? price,
        decimal? minPrice,
        decimal? maxPrice)
    {
        lock (_lock)
        {
            IEnumerable<ProductDto> q = _products.Select(CloneProduct);

            if (!string.IsNullOrEmpty(category))
                q = q.Where(p => MatchStringFilter(p.Category, category));

            if (!string.IsNullOrEmpty(title))
                q = q.Where(p => MatchStringFilter(p.Title, title));

            if (price.HasValue)
                q = q.Where(p => p.Price == price.Value);

            if (minPrice.HasValue)
                q = q.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                q = q.Where(p => p.Price <= maxPrice.Value);

            q = ApplyProductOrder(q, order);

            var list = q.ToList();
            return Paginate(list, page, size);
        }
    }

    public DocumentedPagedListResponse<ProductDto> ListProductsByCategory(
        string category,
        int page,
        int size,
        string? order)
    {
        lock (_lock)
        {
            IEnumerable<ProductDto> q = _products
                .Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase))
                .Select(CloneProduct);

            q = ApplyProductOrder(q, order);
            return Paginate(q.ToList(), page, size);
        }
    }

    public ProductDto? GetProduct(int id)
    {
        lock (_lock)
        {
            var p = _products.FirstOrDefault(x => x.Id == id);
            return p == null ? null : CloneProduct(p);
        }
    }

    public ProductDto CreateProduct(ProductDto input)
    {
        lock (_lock)
        {
            var p = CloneProduct(input);
            p.Id = _nextProductId++;
            _products.Add(p);
            return CloneProduct(p);
        }
    }

    public ProductDto? UpdateProduct(int id, ProductDto input)
    {
        lock (_lock)
        {
            var idx = _products.FindIndex(x => x.Id == id);
            if (idx < 0) return null;
            var p = CloneProduct(input);
            p.Id = id;
            _products[idx] = p;
            return CloneProduct(p);
        }
    }

    public bool DeleteProduct(int id)
    {
        lock (_lock)
        {
            var idx = _products.FindIndex(x => x.Id == id);
            if (idx < 0) return false;
            _products.RemoveAt(idx);
            return true;
        }
    }

    public DocumentedPagedListResponse<CartDto> ListCarts(
        int page,
        int size,
        string? order,
        DateTime? minDate,
        DateTime? maxDate)
    {
        lock (_lock)
        {
            IEnumerable<CartDto> q = _carts.Select(CloneCart);

            if (minDate.HasValue)
            {
                var min = minDate.Value.Date;
                q = q.Where(c => DateTime.TryParse(c.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) && d.Date >= min);
            }

            if (maxDate.HasValue)
            {
                var max = maxDate.Value.Date;
                q = q.Where(c => DateTime.TryParse(c.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) && d.Date <= max);
            }

            q = ApplyCartOrder(q, order);
            return Paginate(q.ToList(), page, size);
        }
    }

    public CartDto? GetCart(int id)
    {
        lock (_lock)
        {
            var c = _carts.FirstOrDefault(x => x.Id == id);
            return c == null ? null : CloneCart(c);
        }
    }

    public CartDto CreateCart(CartDto input)
    {
        lock (_lock)
        {
            var c = CloneCart(input);
            c.Id = _nextCartId++;
            _carts.Add(c);
            return CloneCart(c);
        }
    }

    public CartDto? UpdateCart(int id, CartDto input)
    {
        lock (_lock)
        {
            var idx = _carts.FindIndex(x => x.Id == id);
            if (idx < 0) return null;
            var c = CloneCart(input);
            c.Id = id;
            _carts[idx] = c;
            return CloneCart(c);
        }
    }

    public bool DeleteCart(int id)
    {
        lock (_lock)
        {
            var idx = _carts.FindIndex(x => x.Id == id);
            if (idx < 0) return false;
            _carts.RemoveAt(idx);
            return true;
        }
    }

    public DocumentedPagedListResponse<UserDto> ListUsers(int page, int size, string? order)
    {
        lock (_lock)
        {
            IEnumerable<UserDto> q = _users.Select(CloneUser);
            q = ApplyUserOrder(q, order);
            return Paginate(q.ToList(), page, size);
        }
    }

    public UserDto? GetUser(int id)
    {
        lock (_lock)
        {
            var u = _users.FirstOrDefault(x => x.Id == id);
            return u == null ? null : CloneUser(u);
        }
    }

    public UserDto CreateUser(UserDto input)
    {
        lock (_lock)
        {
            if (_users.Exists(x => string.Equals(x.Email, input.Email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"User with email {input.Email} already exists");

            if (_users.Exists(x => string.Equals(x.Username, input.Username, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"User with username {input.Username} already exists");

            var u = CloneUser(input);
            u.Id = _nextUserId++;
            u.DomainId = null;
            _users.Add(u);
            return CloneUser(u);
        }
    }

    /// <summary>Regista utilizador já persistido em BD na vista documentada (lista GET /users).</summary>
    public UserDto AddLinkedDomainUser(User domainUser, string passwordDisplay)
    {
        lock (_lock)
        {
            var id = _nextUserId++;
            var dto = DocUserMapper.FromDomainUser(domainUser, id, passwordDisplay);
            _users.Add(CloneUser(dto));
            return CloneUser(dto);
        }
    }

    public UserDto? UpdateUser(int id, UserDto input)
    {
        lock (_lock)
        {
            var idx = _users.FindIndex(x => x.Id == id);
            if (idx < 0) return null;
            var u = CloneUser(input);
            u.Id = id;
            _users[idx] = u;
            return CloneUser(u);
        }
    }

    public UserDto? DeleteUser(int id)
    {
        lock (_lock)
        {
            var idx = _users.FindIndex(x => x.Id == id);
            if (idx < 0) return null;
            var removed = _users[idx];
            _users.RemoveAt(idx);
            return CloneUser(removed);
        }
    }

    /// <summary>Usuário do catálogo documentado (login por username/senha em texto plano — apenas protótipo).</summary>
    public UserDto? FindCatalogUserByUsername(string username)
    {
        lock (_lock)
        {
            return _users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        }
    }

    private static bool MatchStringFilter(string fieldValue, string filter)
    {
        if (filter.Length == 0) return true;
        if (filter is "*") return true;
        if (filter.EndsWith('*') && filter.StartsWith('*'))
        {
            var inner = filter[1..^1];
            return fieldValue.Contains(inner, StringComparison.OrdinalIgnoreCase);
        }

        if (filter.EndsWith('*'))
            return fieldValue.StartsWith(filter[..^1], StringComparison.OrdinalIgnoreCase);
        if (filter.StartsWith('*'))
            return fieldValue.EndsWith(filter[1..], StringComparison.OrdinalIgnoreCase);
        return string.Equals(fieldValue, filter, StringComparison.OrdinalIgnoreCase);
    }

    private static IEnumerable<ProductDto> ApplyProductOrder(IEnumerable<ProductDto> source, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return source.OrderBy(p => p.Id);

        IOrderedEnumerable<ProductDto>? ordered = null;
        var first = true;
        foreach (var part in order.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (tokens.Length == 0) continue;
            var prop = string.Join(
                '.',
                tokens[0].Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .ToLowerInvariant();
            var desc = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
            Func<ProductDto, IComparable> key = prop switch
            {
                "price" => p => p.Price,
                "title" => p => p.Title,
                "category" => p => p.Category,
                "id" => p => p.Id,
                "description" => p => p.Description,
                "rating.rate" => p => p.Rating.Rate,
                "rating.count" => p => p.Rating.Count,
                _ => p => p.Id,
            };

            if (first)
            {
                ordered = desc ? source.OrderByDescending(key) : source.OrderBy(key);
                first = false;
            }
            else if (ordered != null)
                ordered = desc ? ordered.ThenByDescending(key) : ordered.ThenBy(key);
        }

        return ordered ?? source.OrderBy(p => p.Id);
    }

    private static IEnumerable<CartDto> ApplyCartOrder(IEnumerable<CartDto> source, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return source.OrderBy(c => c.Id);

        IOrderedEnumerable<CartDto>? ordered = null;
        var first = true;
        foreach (var part in order.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (tokens.Length == 0) continue;
            var prop = tokens[0];
            var desc = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
            Func<CartDto, IComparable> key = prop.ToLowerInvariant() switch
            {
                "userid" => c => c.UserId,
                "date" => c => c.Date,
                _ => c => c.Id,
            };

            if (first)
            {
                ordered = desc ? source.OrderByDescending(key) : source.OrderBy(key);
                first = false;
            }
            else if (ordered != null)
                ordered = desc ? ordered.ThenByDescending(key) : ordered.ThenBy(key);
        }

        return ordered ?? source.OrderBy(c => c.Id);
    }

    private static IEnumerable<UserDto> ApplyUserOrder(IEnumerable<UserDto> source, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return source.OrderBy(u => u.Id);

        IOrderedEnumerable<UserDto>? ordered = null;
        var first = true;
        foreach (var part in order.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (tokens.Length == 0) continue;
            var prop = tokens[0];
            var desc = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
            Func<UserDto, IComparable> key = prop.ToLowerInvariant() switch
            {
                "username" => u => u.Username,
                "email" => u => u.Email,
                _ => u => u.Id,
            };

            if (first)
            {
                ordered = desc ? source.OrderByDescending(key) : source.OrderBy(key);
                first = false;
            }
            else if (ordered != null)
                ordered = desc ? ordered.ThenByDescending(key) : ordered.ThenBy(key);
        }

        return ordered ?? source.OrderBy(u => u.Id);
    }

    private static DocumentedPagedListResponse<T> Paginate<T>(IReadOnlyList<T> all, int page, int size)
    {
        page = page < 1 ? 1 : page;
        size = size < 1 ? 10 : size;
        var total = all.Count;
        var totalPages = (int)Math.Ceiling(total / (double)size);
        var slice = all.Skip((page - 1) * size).Take(size).ToList();
        return new DocumentedPagedListResponse<T>
        {
            Data = slice,
            TotalItems = total,
            CurrentPage = page,
            TotalPages = totalPages == 0 ? 1 : totalPages,
        };
    }

    private static ProductDto CloneProduct(ProductDto p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Price = p.Price,
        Description = p.Description,
        Category = p.Category,
        Image = p.Image,
        Rating = new ProductRatingDto { Rate = p.Rating.Rate, Count = p.Rating.Count },
    };

    private static CartDto CloneCart(CartDto c) => new()
    {
        Id = c.Id,
        UserId = c.UserId,
        Date = c.Date,
        Products = c.Products.Select(l => new CartLineDto { ProductId = l.ProductId, Quantity = l.Quantity }).ToList(),
    };
}
