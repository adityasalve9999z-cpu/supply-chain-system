using SupplyChainSystem.Application.DTOs;

namespace SupplyChainSystem.Application.Interfaces;

public interface IInventoryService
{
    Task<IReadOnlyCollection<InventoryResponse>> GetAsync(bool lowStockOnly, CancellationToken cancellationToken);
    Task<InventoryResponse> AdjustAsync(InventoryAdjustmentRequest request, int userId, CancellationToken cancellationToken);
}
