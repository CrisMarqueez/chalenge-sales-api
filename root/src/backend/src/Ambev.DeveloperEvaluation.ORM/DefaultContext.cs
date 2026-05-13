using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.ORM;

public class DefaultContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleLineItem> SaleLineItems { get; set; }

    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
public class YourDbContextFactory : IDesignTimeDbContextFactory<DefaultContext>
{
    public DefaultContext CreateDbContext(string[] args)
    {
        var configuration = BuildDesignTimeConfiguration();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection não encontrada. Verifique appsettings.Development.json na WebApi.");

        var builder = new DbContextOptionsBuilder<DefaultContext>();
        builder.UseNpgsql(
            connectionString,
            b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
        );

        return new DefaultContext(builder.Options);
    }

    private static IConfigurationRoot BuildDesignTimeConfiguration()
    {
        var candidates = new[]
        {
            Directory.GetCurrentDirectory(),
            Path.Combine(Directory.GetCurrentDirectory(), "src", "Ambev.DeveloperEvaluation.WebApi"),
            Path.Combine(Directory.GetCurrentDirectory(), "Ambev.DeveloperEvaluation.WebApi"),
        };

        foreach (var basePath in candidates)
        {
            var json = Path.Combine(basePath, "appsettings.json");
            if (!File.Exists(json)) continue;

            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }

        throw new InvalidOperationException(
            "Não foi possível localizar appsettings.json da WebApi para design-time (migrations). " +
            "Execute os comandos `dotnet ef` a partir da pasta template/backend ou da pasta da WebApi.");
    }
}