using HMS.Data.ViewModels.BillItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.Bill
{
    public class CreateBillViewModel
    {
        public int ContractId { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ICollection<CreateBillItemViewModel> CreateBillItems { get; set; }
    }
}
