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
