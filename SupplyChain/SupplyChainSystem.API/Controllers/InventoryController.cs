using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyChainSystem.Application.DTOs;
using SupplyChainSystem.Application.Interfaces;

namespace SupplyChainSystem.API.Controllers;

[ApiController, Route("api/inventory"), Authorize]
public sealed class InventoryController(IInventoryService inventory) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<InventoryResponse>> Get([FromQuery] bool lowStockOnly, CancellationToken cancellationToken) => inventory.GetAsync(lowStockOnly, cancellationToken);

    [HttpPost("adjust"), Authorize(Roles = "Admin,Warehouse Manager")]
    public Task<InventoryResponse> Adjust(InventoryAdjustmentRequest request, CancellationToken cancellationToken) =>
        inventory.AdjustAsync(request, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!), cancellationToken);
}
