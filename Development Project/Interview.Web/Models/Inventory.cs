using System;

namespace Interview.Web.Models
{ 
    public class Inventory
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public int Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }
    }

}
