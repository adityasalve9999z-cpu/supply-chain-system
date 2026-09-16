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
