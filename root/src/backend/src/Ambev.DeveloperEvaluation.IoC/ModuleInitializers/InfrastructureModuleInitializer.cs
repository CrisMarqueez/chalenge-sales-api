using Ambev.DeveloperEvaluation.Application.Integration;
using Ambev.DeveloperEvaluation.Application.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.IoC.Integration;
using Ambev.DeveloperEvaluation.IoC.Security;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Serialization.Json;
using Rebus.Transport.InMem;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(new InMemNetwork());

        builder.Services.AddRebus(
            (configure, provider) => configure
                .Serialization(s => s.UseSystemTextJson())
                .Transport(t =>
                    t.UseInMemoryTransport(provider.GetRequiredService<InMemNetwork>(), "sales-api")));

        builder.Services.AutoRegisterHandlersFromAssemblyOf<SaleCreatedIntegrationEventHandler>();

        builder.Services.AddTransient<IIntegrationEventPublisher, RebusIntegrationEventPublisher>();

        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentPrincipal, HttpContextCurrentPrincipal>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();
    }
}