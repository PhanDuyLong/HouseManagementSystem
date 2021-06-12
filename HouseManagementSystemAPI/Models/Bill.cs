using System;
using System.Collections.Generic;

#nullable disable

namespace HMSAPI.Models
{
    public partial class Bill
    {
        public Bill()
        {
            BillItems = new HashSet<BillItem>();
            Payments = new HashSet<Payment>();
        }

        public string Id { get; set; }
        public int? ContractId { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Status { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual ICollection<BillItem> BillItems { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
