using Ambev.DeveloperEvaluation.Application.Integration;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.IoC.Integration;

public class RebusIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IBus _bus;

    public RebusIntegrationEventPublisher(IBus bus) => _bus = bus;

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class =>
        _bus.Publish(@event);
}
