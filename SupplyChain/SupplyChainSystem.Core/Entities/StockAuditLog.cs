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
