using HMS.Data.Models;
using HMS.Data.ViewModels.ServiceContract;
using System;
using System.Collections.Generic;

namespace HMS.Data.ViewModels.Contract.Base
{
    public class ContractDetailViewModel : ContractBaseViewModel
    {
        public virtual ICollection<ServiceContractDetailViewModel> ServiceContracts { get; set; }
    }
}
