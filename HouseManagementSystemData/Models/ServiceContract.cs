using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class ServiceContract
    {
        public ServiceContract()
        {
            BillItems = new HashSet<BillItem>();
        }

        public int Id { get; set; }
        public int? ContractId { get; set; }
        public int? ServiceId { get; set; }
        public double? UnitPrice { get; set; }
        public bool? Status { get; set; }
        public string ClockId { get; set; }

        public virtual Clock Clock { get; set; }
        public virtual Contract Contract { get; set; }
        public virtual Service Service { get; set; }
        public virtual ICollection<BillItem> BillItems { get; set; }
    }
}
