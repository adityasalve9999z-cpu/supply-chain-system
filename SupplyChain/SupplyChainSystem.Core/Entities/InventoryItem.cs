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
