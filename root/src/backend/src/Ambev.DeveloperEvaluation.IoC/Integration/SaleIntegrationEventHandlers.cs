using Ambev.DeveloperEvaluation.Application.Integration.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.IoC.Integration;

public class SaleCreatedIntegrationEventHandler : IHandleMessages<SaleCreatedIntegrationEvent>
{
    private readonly ILogger<SaleCreatedIntegrationEventHandler> _logger;

    public SaleCreatedIntegrationEventHandler(ILogger<SaleCreatedIntegrationEventHandler> logger) =>
        _logger = logger;

    public Task Handle(SaleCreatedIntegrationEvent message)
    {
        _logger.LogInformation("SaleCreated: SaleId={SaleId}", message.SaleId);
        return Task.CompletedTask;
    }
}

public class SaleModifiedIntegrationEventHandler : IHandleMessages<SaleModifiedIntegrationEvent>
{
    private readonly ILogger<SaleModifiedIntegrationEventHandler> _logger;

    public SaleModifiedIntegrationEventHandler(ILogger<SaleModifiedIntegrationEventHandler> logger) =>
        _logger = logger;

    public Task Handle(SaleModifiedIntegrationEvent message)
    {
        _logger.LogInformation("SaleModified: SaleId={SaleId}", message.SaleId);
        return Task.CompletedTask;
    }
}

public class SaleCancelledIntegrationEventHandler : IHandleMessages<SaleCancelledIntegrationEvent>
{
    private readonly ILogger<SaleCancelledIntegrationEventHandler> _logger;

    public SaleCancelledIntegrationEventHandler(ILogger<SaleCancelledIntegrationEventHandler> logger) =>
        _logger = logger;

    public Task Handle(SaleCancelledIntegrationEvent message)
    {
        _logger.LogInformation("SaleCancelled: SaleId={SaleId}", message.SaleId);
        return Task.CompletedTask;
    }
}

public class ItemCancelledIntegrationEventHandler : IHandleMessages<ItemCancelledIntegrationEvent>
{
    private readonly ILogger<ItemCancelledIntegrationEventHandler> _logger;

    public ItemCancelledIntegrationEventHandler(ILogger<ItemCancelledIntegrationEventHandler> logger) =>
        _logger = logger;

    public Task Handle(ItemCancelledIntegrationEvent message)
    {
        _logger.LogInformation(
            "ItemCancelled: SaleId={SaleId}, LineItemId={LineItemId}",
            message.SaleId,
            message.LineItemId);
        return Task.CompletedTask;
    }
}
