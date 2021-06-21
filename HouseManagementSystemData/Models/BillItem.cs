using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class BillItem
    {
        public int Id { get; set; }
        public int? ServiceContractId { get; set; }
        public string BillId { get; set; }
        public double? TotalPrice { get; set; }

        public virtual Bill Bill { get; set; }
        public virtual ServiceContract ServiceContract { get; set; }
    }
}
