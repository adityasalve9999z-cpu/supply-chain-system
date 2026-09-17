using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChainSystem.Application.DTOs;
using SupplyChainSystem.Core.Entities;
using SupplyChainSystem.Infrastructure.Data;

namespace SupplyChainSystem.API.Controllers;

[ApiController, Route("api/catalog"), Authorize]
public sealed class CatalogController(SupplyChainDbContext db) : ControllerBase
{
    [HttpGet("products")] public Task<List<Product>> Products(CancellationToken ct) => db.Products.AsNoTracking().ToListAsync(ct);
    [HttpPost("products"), Authorize(Roles = "Admin,Warehouse Manager")] public async Task<Product> CreateProduct(ProductRequest request, CancellationToken ct) { var product = new Product { Name = request.Name, Description = request.Description ?? "", Price = request.Price, Sku = request.Sku, CategoryId = request.CategoryId }; db.Products.Add(product); await db.SaveChangesAsync(ct); return product; }
    [HttpPut("products/{id:int}"), Authorize(Roles = "Admin,Warehouse Manager")] public async Task<IActionResult> UpdateProduct(int id, ProductRequest request, CancellationToken ct) { var p = await db.Products.FindAsync([id], ct) ?? throw new KeyNotFoundException("Product was not found."); p.Name = request.Name; p.Description = request.Description ?? ""; p.Price = request.Price; p.Sku = request.Sku; p.CategoryId = request.CategoryId; await db.SaveChangesAsync(ct); return Ok(p); }
    [HttpDelete("products/{id:int}"), Authorize(Roles = "Admin")] public async Task<IActionResult> DeleteProduct(int id, CancellationToken ct) { var p = await db.Products.FindAsync([id], ct) ?? throw new KeyNotFoundException("Product was not found."); db.Products.Remove(p); await db.SaveChangesAsync(ct); return NoContent(); }
    [HttpGet("categories")] public Task<List<Category>> Categories(CancellationToken ct) => db.Categories.AsNoTracking().ToListAsync(ct);
    [HttpPost("categories"), Authorize(Roles = "Admin,Warehouse Manager")] public async Task<Category> CreateCategory(CategoryRequest request, CancellationToken ct) { var c = new Category { Name = request.Name, Description = request.Description ?? "" }; db.Categories.Add(c); await db.SaveChangesAsync(ct); return c; }
    [HttpGet("warehouses")] public Task<List<Warehouse>> Warehouses(CancellationToken ct) => db.Warehouses.AsNoTracking().ToListAsync(ct);
    [HttpPost("warehouses"), Authorize(Roles = "Admin,Warehouse Manager")] public async Task<Warehouse> CreateWarehouse(WarehouseRequest request, CancellationToken ct) { var w = new Warehouse { Name = request.Name, Location = request.Location }; db.Warehouses.Add(w); await db.SaveChangesAsync(ct); return w; }
    [HttpGet("suppliers")] public Task<List<Supplier>> Suppliers(CancellationToken ct) => db.Suppliers.AsNoTracking().ToListAsync(ct);
    [HttpPost("suppliers"), Authorize(Roles = "Admin,Warehouse Manager")] public async Task<Supplier> CreateSupplier(SupplierRequest request, CancellationToken ct) { var s = new Supplier { Name = request.Name, ContactEmail = request.ContactEmail, Phone = request.Phone ?? "" }; db.Suppliers.Add(s); await db.SaveChangesAsync(ct); return s; }
}
