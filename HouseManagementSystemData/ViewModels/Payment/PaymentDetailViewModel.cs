using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels
{
    public class PaymentDetailViewModel
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int? BillId { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
