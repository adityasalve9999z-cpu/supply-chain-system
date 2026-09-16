using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Application.DTOs;
using SupplyChainSystem.Application.Interfaces;
using SupplyChainSystem.Core.Entities;
using SupplyChainSystem.Infrastructure.Data;

namespace SupplyChainSystem.Infrastructure.Services;

public sealed class OrderService(SupplyChainDbContext db) : IOrderService
{
    public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        if (!await db.Suppliers.AnyAsync(x => x.Id == request.SupplierId, cancellationToken))
            throw new KeyNotFoundException("Supplier was not found.");
        var order = new PurchaseOrder { SupplierId = request.SupplierId, OrderDate = DateTime.UtcNow,
            ExpectedDeliveryDate = request.ExpectedDeliveryDate, Status = POStatus.Pending,
            Items = request.Items.Select(x => new PurchaseOrderItem { ProductId = x.ProductId, Quantity = x.Quantity, UnitPrice = x.UnitPrice }).ToList() };
        db.PurchaseOrders.Add(order);
        await db.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task ReceiveShipmentAsync(int purchaseOrderId, ReceiveShipmentRequest request, int userId, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var order = await db.PurchaseOrders.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == purchaseOrderId, cancellationToken)
            ?? throw new KeyNotFoundException("Purchase order was not found.");
        if (order.Status == POStatus.Received || order.Status == POStatus.Cancelled)
            throw new InvalidOperationException("This purchase order cannot receive a shipment.");
        foreach (var line in order.Items)
        {
            var item = await db.InventoryItems.SingleOrDefaultAsync(x => x.ProductId == line.ProductId && x.WarehouseId == request.WarehouseId, cancellationToken);
            if (item is null) { item = new InventoryItem { ProductId = line.ProductId, WarehouseId = request.WarehouseId, LastUpdated = DateTime.UtcNow }; db.InventoryItems.Add(item); }
            var old = item.Quantity; item.Quantity += line.Quantity; item.LastUpdated = DateTime.UtcNow;
            db.StockAuditLogs.Add(new StockAuditLog { ProductId = line.ProductId, WarehouseId = request.WarehouseId, UserId = userId,
                ActionType = "PO Received", OldQuantity = old, NewQuantity = item.Quantity, Timestamp = DateTime.UtcNow });
        }
        order.Status = POStatus.Received;
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<FulfillmentOrder> CreateFulfillmentOrderAsync(FulfillmentOrderRequest request, int userId, CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var lines = request.Items.ToList();
        foreach (var line in lines)
        {
            var stock = await db.InventoryItems.SingleOrDefaultAsync(x => x.ProductId == line.ProductId && x.WarehouseId == line.WarehouseId, cancellationToken);
            if (stock is null || stock.Quantity < line.Quantity) throw new InvalidOperationException($"Insufficient stock for product {line.ProductId}.");
        }
        var order = new FulfillmentOrder { CustomerName = request.CustomerName, ShippingAddress = request.ShippingAddress,
            OrderDate = DateTime.UtcNow, Status = FulfillmentStatus.Processing,
            Items = lines.Select(x => new FulfillmentOrderItem { ProductId = x.ProductId, Quantity = x.Quantity }).ToList() };
        db.FulfillmentOrders.Add(order);
        foreach (var line in lines)
        {
            var stock = await db.InventoryItems.SingleAsync(x => x.ProductId == line.ProductId && x.WarehouseId == line.WarehouseId, cancellationToken);
            var old = stock.Quantity; stock.Quantity -= line.Quantity; stock.LastUpdated = DateTime.UtcNow;
            db.StockAuditLogs.Add(new StockAuditLog { ProductId = line.ProductId, WarehouseId = line.WarehouseId, UserId = userId,
                ActionType = "Order Fulfilled", OldQuantity = old, NewQuantity = stock.Quantity, Timestamp = DateTime.UtcNow });
        }
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return order;
    }
}
