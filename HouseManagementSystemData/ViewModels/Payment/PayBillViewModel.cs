using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.ViewModels
{
    public class PayBillViewModel
    {
        public DateTime? Date { get; set; }
        [Required]
        public int BillId { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }


    }
}
