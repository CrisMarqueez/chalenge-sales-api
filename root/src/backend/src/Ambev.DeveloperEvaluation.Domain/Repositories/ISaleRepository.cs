using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    Task<Sale?> GetByIdAsync(Guid id, bool tracking = false, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Sale> Items, int TotalCount)> ListPagedAsync(
        int page,
        int pageSize,
        Guid? filterByOwnerUserId,
        CancellationToken cancellationToken = default);

    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Sale?> SetSaleCancelledAsync(Guid saleId, CancellationToken cancellationToken = default);

    Task<Sale?> SetLineItemCancelledAsync(
        Guid saleId,
        Guid lineItemId,
        CancellationToken cancellationToken = default);
}
