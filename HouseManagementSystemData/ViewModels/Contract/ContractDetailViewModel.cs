using HMS.Data.Models;
using System;
using System.Collections.Generic;

namespace HMS.Data.ViewModels.Contract.Base
{
    public class ContractDetailViewModel : ContractBaseViewModel
    {
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<ClockInContract> ClockInContracts { get; set; }
        public virtual ICollection<ServiceContract> ServiceContracts { get; set; }
    }
}
