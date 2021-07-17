﻿using HMS.Data.ViewModels.Clock;
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
        public int? ContractId { get; set; }
        public int? ServiceId { get; set; }
        public double? UnitPrice { get; set; }
        public int ClockId { get; set; }

    }
}
