using HMS.Data.ViewModels.Clock;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.ServiceContract
{
    public class ServiceContractDetailViewModel
    {
        public int Id { get; set; }
        public int? ContractId { get; set; }
        public int? ServiceId { get; set; }
        public double? UnitPrice { get; set; }
        public bool? Status { get; set; }
        public string ClockId { get; set; }

        public virtual ServiceViewModel Service { get; set; }
    }
}
