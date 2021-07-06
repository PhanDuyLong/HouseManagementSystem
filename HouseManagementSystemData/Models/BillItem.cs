using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class BillItem
    {
        public int Id { get; set; }
        public int? ServiceContractId { get; set; }
        public int? BillId { get; set; }
        public double? StartValue { get; set; }
        public double? EndValue { get; set; }
        public double? TotalPrice { get; set; }
        public bool? Status { get; set; }

        public virtual Bill Bill { get; set; }
        public virtual ServiceContract ServiceContract { get; set; }
    }
}
