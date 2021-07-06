using HMS.Data.ViewModels.BillItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.Bill
{
    public class UpdateBillViewModel
    {
        public int Id { get; set; }
        public int? ContractId { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Note { get; set; }

        public virtual ICollection<UpdateBillItemViewModel> UpdateBillItems { get; set; }
    }
}
