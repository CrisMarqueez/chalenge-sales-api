namespace Ambev.DeveloperEvaluation.Application.Integration.Events;

public record SaleCreatedIntegrationEvent(Guid SaleId);

public record SaleModifiedIntegrationEvent(Guid SaleId);

public record SaleCancelledIntegrationEvent(Guid SaleId);

public record ItemCancelledIntegrationEvent(Guid SaleId, Guid LineItemId);
