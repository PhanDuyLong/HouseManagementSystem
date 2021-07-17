using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.ViewModels.ServiceContract
{
    public class CreateServiceContractViewModel
    {
        public int ServiceId { get; set; }
        public int StartClockValue { get; set; }
    }
}
