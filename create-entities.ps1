$basePath = "c:\Users\Admin\Desktop\asp.net\SupplyChain\SupplyChainSystem.Core\Entities"
New-Item -ItemType Directory -Force -Path $basePath

@'
using System;
using System.Collections.Generic;

namespace SupplyChainSystem.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
'@ | Out-File -FilePath "$basePath\User.cs" -Encoding utf8

@'
using System.Collections.Generic;

namespace SupplyChainSystem.Core.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
'@ | Out-File -FilePath "$basePath\Role.cs" -Encoding utf8

@'
using System.Collections.Generic;

namespace SupplyChainSystem.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
'@ | Out-File -FilePath "$basePath\Category.cs" -Encoding utf8

@'
using System.Collections.Generic;

namespace SupplyChainSystem.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Sku { get; set; } = string.Empty;
        
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        
        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    }
}
'@ | Out-File -FilePath "$basePath\Product.cs" -Encoding utf8

@'
using System.Collections.Generic;

namespace SupplyChainSystem.Core.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        
        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    }
}
'@ | Out-File -FilePath "$basePath\Warehouse.cs" -Encoding utf8

@'
using System;

namespace SupplyChainSystem.Core.Entities
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; }
        
        public DateTime LastUpdated { get; set; }
    }
}
'@ | Out-File -FilePath "$basePath\InventoryItem.cs" -Encoding utf8

@'
using System.Collections.Generic;

namespace SupplyChainSystem.Core.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
'@ | Out-File -FilePath "$basePath\Supplier.cs" -Encoding utf8

@'
using System;
using System.Collections.Generic;

namespace SupplyChainSystem.Core.Entities
{
    public enum POStatus { Pending, Approved, Shipped, Received, Cancelled }

    public class PurchaseOrder
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        
        public POStatus Status { get; set; }
        
        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    }
}
'@ | Out-File -FilePath "$basePath\PurchaseOrder.cs" -Encoding utf8

@'
namespace SupplyChainSystem.Core.Entities
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
'@ | Out-File -FilePath "$basePath\PurchaseOrderItem.cs" -Encoding utf8

@'
using System;
using System.Collections.Generic;

namespace SupplyChainSystem.Core.Entities
{
    public enum FulfillmentStatus { Pending, Processing, Shipped, Delivered, Cancelled }

    public class FulfillmentOrder
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        
        public DateTime OrderDate { get; set; }
        public FulfillmentStatus Status { get; set; }
        
        public ICollection<FulfillmentOrderItem> Items { get; set; } = new List<FulfillmentOrderItem>();
    }
}
'@ | Out-File -FilePath "$basePath\FulfillmentOrder.cs" -Encoding utf8

@'
namespace SupplyChainSystem.Core.Entities
{
    public class FulfillmentOrderItem
    {
        public int Id { get; set; }
        public int FulfillmentOrderId { get; set; }
        public FulfillmentOrder FulfillmentOrder { get; set; } = null!;
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
'@ | Out-File -FilePath "$basePath\FulfillmentOrderItem.cs" -Encoding utf8

@'
using System;

namespace SupplyChainSystem.Core.Entities
{
    public class StockAuditLog
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        
        public int UserId { get; set; } // The user who made the change
        
        public string ActionType { get; set; } = string.Empty; // e.g., "Manual Adjustment", "PO Received", "Order Fulfilled"
        
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}
'@ | Out-File -FilePath "$basePath\StockAuditLog.cs" -Encoding utf8
