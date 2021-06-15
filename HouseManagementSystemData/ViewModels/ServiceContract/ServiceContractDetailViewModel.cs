using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.ServiceContract
{
    public class ServiceContractDetailViewModel
    {
        public int Id { get; set; }
        public int? ContractId { get; set; }
        public string ServiceId { get; set; }
        public double? UnitPrice { get; set; }
        public bool? Status { get; set; }

        public virtual ServiceViewModel Service { get; set; }
    }
}
