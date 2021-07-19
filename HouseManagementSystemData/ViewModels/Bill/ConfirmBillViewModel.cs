using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.ViewModels.Bill
{
    public class ConfirmBillViewModel
    {
        [Required]
        public int Id { get; set; }
        public string Note { get; set; }
    }
}
