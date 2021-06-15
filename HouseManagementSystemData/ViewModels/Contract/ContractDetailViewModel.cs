using HMS.Data.Models;
using HMS.Data.ViewModels.ClockInContract;
using HMS.Data.ViewModels.ServiceContract;
using System;
using System.Collections.Generic;

namespace HMS.Data.ViewModels.Contract.Base
{
    public class ContractDetailViewModel : ContractBaseViewModel
    {
        public virtual ICollection<ClockInContractDetailViewModel> ClockInContracts { get; set; }
        public virtual ICollection<ServiceContractDetailViewModel> ServiceContracts { get; set; }
    }
}
