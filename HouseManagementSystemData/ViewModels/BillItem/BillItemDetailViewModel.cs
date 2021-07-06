using HMS.Data.ViewModels.ClockValueInBillItem;
using HMS.Data.ViewModels.ServiceContract;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels
{
    public class BillItemDetailViewModel
    {
        public int Id { get; set; }
        public int? ServiceContractId { get; set; }
        public int? BillId { get; set; }
        public double? TotalPrice { get; set; }
        public double? StartValue { get; set; }
        public double? EndValue { get; set; }
        public bool? Status { get; set; }

        public virtual ServiceContractDetailViewModel ServiceContract { get; set; }
        public virtual ClockValueInBillItemDetailViewModel ClockValueInBillItem { get; set; }
    }
}
