using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, bool tracking = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.AsQueryable();
        if (!tracking)
            query = query.AsNoTracking();

        return await query
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Sale> Items, int TotalCount)> ListPagedAsync(
        int page,
        int pageSize,
        Guid? filterByOwnerUserId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Sale> query = _context.Sales.AsNoTracking().OrderByDescending(s => s.SaleDate);
        if (filterByOwnerUserId.HasValue)
            query = query.Where(s => s.OwnerUserId == filterByOwnerUserId.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        await _context.SaleLineItems
            .Where(i => i.SaleId == sale.Id)
            .ExecuteDeleteAsync(cancellationToken);

        var updatedRows = await _context.Sales
            .Where(s => s.Id == sale.Id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(s => s.SaleDate, sale.SaleDate)
                    .SetProperty(s => s.CustomerId, sale.CustomerId)
                    .SetProperty(s => s.CustomerName, sale.CustomerName)
                    .SetProperty(s => s.BranchId, sale.BranchId)
                    .SetProperty(s => s.BranchName, sale.BranchName)
                    .SetProperty(s => s.IsCancelled, sale.IsCancelled)
                    .SetProperty(s => s.TotalAmount, sale.TotalAmount)
                    .SetProperty(s => s.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        if (updatedRows == 0)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new KeyNotFoundException($"Sale with ID {sale.Id} not found");
        }

        foreach (var item in sale.Items)
        {
            item.SaleId = sale.Id;
            item.Sale = null;
            _context.SaleLineItems.Add(item);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        _context.ChangeTracker.Clear();

        return await GetByIdAsync(sale.Id, tracking: false, cancellationToken)
               ?? throw new KeyNotFoundException($"Sale with ID {sale.Id} not found");
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<Sale?> SetSaleCancelledAsync(Guid saleId, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == saleId, cancellationToken);

        if (sale == null)
            return null;

        sale.IsCancelled = true;
        sale.UpdatedAt = DateTime.UtcNow;
        sale.RecalculateTotals();
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> SetLineItemCancelledAsync(
        Guid saleId,
        Guid lineItemId,
        CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == saleId, cancellationToken);

        if (sale == null)
            return null;

        var line = sale.Items.FirstOrDefault(i => i.Id == lineItemId);
        if (line == null)
            return null;

        line.IsCancelled = true;
        sale.UpdatedAt = DateTime.UtcNow;
        sale.RecalculateTotals();
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }
}
