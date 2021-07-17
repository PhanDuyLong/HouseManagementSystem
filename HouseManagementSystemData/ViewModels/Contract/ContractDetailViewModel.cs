using HMS.Data.Models;
using HMS.Data.ViewModels.Room;
using HMS.Data.ViewModels.ServiceContract;
using System;
using System.Collections.Generic;

namespace HMS.Data.ViewModels.Contract.Base
{
    public class ContractDetailViewModel : ContractBaseViewModel
    {
        public string HouseName { get; set; }
        public string RoomName { get; set; }
        public string OwnerName { get; set; }
        public virtual ICollection<ServiceContractDetailViewModel> ServiceContracts { get; set; }
    }
}
