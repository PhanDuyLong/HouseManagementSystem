using HMS.Data.ViewModels.Contract.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.Bill
{
    public class BillDetailViewModel
    {
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
        public bool? IsPaidInFull { get; set; }

        public virtual ContractBaseViewModel Contract { get; set; }
        public virtual ICollection<BillItemDetailViewModel> BillItems { get; set; }
        public virtual ICollection<PaymentDetailViewModel> Payments { get; set; }
    }
}
