using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels
{
    public class BillPaymentViewModel
    {
        public DateTime? Date { get; set; }
        public int? BillId { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }


    }
}
