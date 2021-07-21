using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class Bill
    {
        public Bill()
        {
            BillItems = new HashSet<BillItem>();
            Payments = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public int? ContractId { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? TotalPrice { get; set; }
        public string Note { get; set; }
        public bool? Status { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsWaiting { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual ICollection<BillItem> BillItems { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
