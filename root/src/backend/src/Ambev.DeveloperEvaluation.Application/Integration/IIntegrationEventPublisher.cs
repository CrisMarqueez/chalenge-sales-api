namespace Ambev.DeveloperEvaluation.Application.Integration;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;
}
