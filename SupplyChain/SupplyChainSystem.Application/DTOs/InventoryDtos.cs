using System.ComponentModel.DataAnnotations;

namespace SupplyChainSystem.Application.DTOs;

public sealed record ProductRequest([Required, MaxLength(200)] string Name, [MaxLength(2000)] string? Description, [Range(0, double.MaxValue)] decimal Price, [Required, MaxLength(50)] string Sku, [Range(1, int.MaxValue)] int CategoryId);
public sealed record CategoryRequest([Required, MaxLength(150)] string Name, [MaxLength(1000)] string? Description);
public sealed record WarehouseRequest([Required, MaxLength(150)] string Name, [Required, MaxLength(500)] string Location);
public sealed record InventoryAdjustmentRequest([Range(1, int.MaxValue)] int ProductId, [Range(1, int.MaxValue)] int WarehouseId, int Quantity, [Range(0, int.MaxValue)] int LowStockThreshold, [Required, MaxLength(500)] string Reason);
public sealed record InventoryResponse(int Id, int ProductId, string ProductName, int WarehouseId, string WarehouseName, int Quantity, int LowStockThreshold, bool IsLowStock, DateTime LastUpdated);
