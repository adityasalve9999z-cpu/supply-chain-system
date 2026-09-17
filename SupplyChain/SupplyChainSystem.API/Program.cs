using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SupplyChainSystem.API.Middleware;
using SupplyChainSystem.Application.Interfaces;
using SupplyChainSystem.Infrastructure.Data;
using SupplyChainSystem.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key must be configured.");
if (Encoding.UTF8.GetByteCount(jwtKey) < 32) throw new InvalidOperationException("Jwt:Key must be at least 256 bits.");

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddDbContext<SupplyChainDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true, ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true, ClockSkew = TimeSpan.FromMinutes(1)
    };
});
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Supply Chain API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>()
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SupplyChainDbContext>();
    await db.Database.EnsureCreatedAsync();
    await SeedDataAsync(db);
}

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();

static async Task SeedDataAsync(SupplyChainDbContext db)
{
    if (await db.Categories.AnyAsync()) return;

    var category = new SupplyChainSystem.Core.Entities.Category
    {
        Name = "Electronics",
        Description = "Electronic equipment"
    };
    var warehouse = new SupplyChainSystem.Core.Entities.Warehouse
    {
        Name = "Main Warehouse",
        Location = "Local Development"
    };
    db.Categories.Add(category);
    db.Warehouses.Add(warehouse);
    await db.SaveChangesAsync();

    var product = new SupplyChainSystem.Core.Entities.Product
    {
        Name = "Laptop",
        Description = "Business laptop",
        Price = 999.99m,
        Sku = "LAP-001",
        CategoryId = category.Id
    };
    db.Products.Add(product);
    await db.SaveChangesAsync();

    db.InventoryItems.Add(new SupplyChainSystem.Core.Entities.InventoryItem
    {
        ProductId = product.Id,
        WarehouseId = warehouse.Id,
        Quantity = 50,
        LowStockThreshold = 10,
        LastUpdated = DateTime.UtcNow
    });
    await db.SaveChangesAsync();
}
