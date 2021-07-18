using HMS.Data.ViewModels.Clock;
using HMS.Data.ViewModels.Room;
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
        public double UnitPrice { get; set; }
        public bool? Status { get; set; }
        public int ClockId { get; set; }

        public virtual ClockDetailViewModel Clock { get; set; }
        public virtual ServiceViewModel Service { get; set; }
    }
}
