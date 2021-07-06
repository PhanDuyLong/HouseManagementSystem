using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.BillItem
{
    public class UpdateBillItemViewModel
    {
        public int? ServiceContractId { get; set; }
        public int StartValue { get; set; }
        public int EndValue { get; set; }
        public bool isDeleted { get; set; }
    }
}
