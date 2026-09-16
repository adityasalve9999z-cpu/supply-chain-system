using SupplyChainSystem.Application.DTOs;
using SupplyChainSystem.Core.Entities;

namespace SupplyChainSystem.Application.Interfaces;

public interface IOrderService
{
    Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrderRequest request, CancellationToken cancellationToken);
    Task ReceiveShipmentAsync(int purchaseOrderId, ReceiveShipmentRequest request, int userId, CancellationToken cancellationToken);
    Task<FulfillmentOrder> CreateFulfillmentOrderAsync(FulfillmentOrderRequest request, int userId, CancellationToken cancellationToken);
}
