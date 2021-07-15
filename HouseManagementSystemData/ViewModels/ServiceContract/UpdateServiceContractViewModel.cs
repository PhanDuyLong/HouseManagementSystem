using HMS.Data.ViewModels.Clock;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.ViewModels.ServiceContract
{
    public class UpdateServiceContractViewModel
    {
        [Required]
        public int Id { get; set; }

       
        public double? UnitPrice { get; set; }
    }
}
