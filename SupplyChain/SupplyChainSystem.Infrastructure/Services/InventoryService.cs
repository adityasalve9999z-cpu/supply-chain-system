using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Application.DTOs;
using SupplyChainSystem.Application.Interfaces;
using SupplyChainSystem.Core.Entities;
using SupplyChainSystem.Infrastructure.Data;

namespace SupplyChainSystem.Infrastructure.Services;

public sealed class InventoryService(SupplyChainDbContext db) : IInventoryService
{
    public async Task<IReadOnlyCollection<InventoryResponse>> GetAsync(bool lowStockOnly, CancellationToken cancellationToken)
    {
        IQueryable<InventoryItem> query = db.InventoryItems.AsNoTracking().Include(x => x.Product).Include(x => x.Warehouse);
        if (lowStockOnly) query = query.Where(x => x.Quantity <= x.LowStockThreshold);
        return await query.Select(x => new InventoryResponse(x.Id, x.ProductId, x.Product.Name, x.WarehouseId, x.Warehouse.Name,
            x.Quantity, x.LowStockThreshold, x.Quantity <= x.LowStockThreshold, x.LastUpdated)).ToListAsync(cancellationToken);
    }

    public async Task<InventoryResponse> AdjustAsync(InventoryAdjustmentRequest request, int userId, CancellationToken cancellationToken)
    {
        var item = await db.InventoryItems.Include(x => x.Product).Include(x => x.Warehouse)
            .SingleOrDefaultAsync(x => x.ProductId == request.ProductId && x.WarehouseId == request.WarehouseId, cancellationToken);
        if (item is null)
        {
            item = new InventoryItem { ProductId = request.ProductId, WarehouseId = request.WarehouseId, Quantity = 0, LastUpdated = DateTime.UtcNow };
            db.InventoryItems.Add(item);
            await db.Entry(item).Reference(x => x.Product).LoadAsync(cancellationToken);
            await db.Entry(item).Reference(x => x.Warehouse).LoadAsync(cancellationToken);
        }
        var oldQuantity = item.Quantity;
        var newQuantity = oldQuantity + request.Quantity;
        if (newQuantity < 0) throw new InvalidOperationException("The adjustment would make stock negative.");
        item.Quantity = newQuantity;
        item.LowStockThreshold = request.LowStockThreshold;
        item.LastUpdated = DateTime.UtcNow;
        db.StockAuditLogs.Add(new StockAuditLog { ProductId = item.ProductId, WarehouseId = item.WarehouseId, UserId = userId,
            ActionType = request.Reason, OldQuantity = oldQuantity, NewQuantity = newQuantity, Timestamp = DateTime.UtcNow });
        await db.SaveChangesAsync(cancellationToken);
        return new InventoryResponse(item.Id, item.ProductId, item.Product.Name, item.WarehouseId, item.Warehouse.Name,
            item.Quantity, item.LowStockThreshold, item.Quantity <= item.LowStockThreshold, item.LastUpdated);
    }
}
