using System.ComponentModel.DataAnnotations;

namespace SupplyChainSystem.Application.DTOs;

public sealed record SupplierRequest([Required, MaxLength(200)] string Name, [Required, EmailAddress, MaxLength(256)] string ContactEmail, [MaxLength(40)] string? Phone);
public sealed record PurchaseOrderItemRequest([Range(1, int.MaxValue)] int ProductId, [Range(1, int.MaxValue)] int Quantity, [Range(0, double.MaxValue)] decimal UnitPrice);
public sealed record PurchaseOrderRequest([Range(1, int.MaxValue)] int SupplierId, DateTime? ExpectedDeliveryDate, [Required, MinLength(1)] IReadOnlyCollection<PurchaseOrderItemRequest> Items);
public sealed record ReceiveShipmentRequest([Range(1, int.MaxValue)] int WarehouseId);
public sealed record FulfillmentOrderItemRequest([Range(1, int.MaxValue)] int ProductId, [Range(1, int.MaxValue)] int Quantity, [Range(1, int.MaxValue)] int WarehouseId);
public sealed record FulfillmentOrderRequest([Required, MaxLength(200)] string CustomerName, [Required, MaxLength(1000)] string ShippingAddress, [Required, MinLength(1)] IReadOnlyCollection<FulfillmentOrderItemRequest> Items);
