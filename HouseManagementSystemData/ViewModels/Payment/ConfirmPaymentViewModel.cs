using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.ViewModels.Payment
{
    public class ConfirmPaymentViewModel
    {
        [Required]
        public int BillId { get; set; }
        public DateTime? Date { get; set; }
        public string Note { get; set; }
        public bool? Status { get; set; }
    }
}
