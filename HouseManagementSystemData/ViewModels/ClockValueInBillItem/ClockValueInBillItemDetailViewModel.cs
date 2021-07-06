using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.ClockValueInBillItem
{
    public class ClockValueInBillItemDetailViewModel
    {
        public int Id { get; set; }
        public int? BillItemId { get; set; }
        public int? StartValueId { get; set; }
        public int? EndValueId { get; set; }
        public bool? Status { get; set; }

        public virtual ClockValueDetailViewModel EndValue { get; set; }
        public virtual ClockValueDetailViewModel StartValue { get; set; }
    }
}
