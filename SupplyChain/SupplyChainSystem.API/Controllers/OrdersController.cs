using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyChainSystem.Application.DTOs;
using SupplyChainSystem.Application.Interfaces;
using SupplyChainSystem.Core.Entities;

namespace SupplyChainSystem.API.Controllers;

[ApiController, Route("api/orders"), Authorize]
public sealed class OrdersController(IOrderService orders) : ControllerBase
{
    [HttpPost("purchase"), Authorize(Roles = "Admin,Warehouse Manager")]
    public Task<PurchaseOrder> CreatePurchaseOrder(PurchaseOrderRequest request, CancellationToken cancellationToken) => orders.CreatePurchaseOrderAsync(request, cancellationToken);

    [HttpPost("purchase/{id:int}/receive"), Authorize(Roles = "Admin,Warehouse Manager")]
    public async Task<IActionResult> ReceiveShipment(int id, ReceiveShipmentRequest request, CancellationToken cancellationToken)
    {
        await orders.ReceiveShipmentAsync(id, request, UserId(), cancellationToken);
        return NoContent();
    }

    [HttpPost("fulfillment"), Authorize(Roles = "Admin,Warehouse Manager,Staff")]
    public Task<FulfillmentOrder> CreateFulfillment(FulfillmentOrderRequest request, CancellationToken cancellationToken) => orders.CreateFulfillmentOrderAsync(request, UserId(), cancellationToken);

    private int UserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
