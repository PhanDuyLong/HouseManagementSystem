using HMS.Data.Models;
using HMS.Data.ViewModels.Contract.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.RoomViewModels
{
    public class RoomShowViewModel : RoomBaseViewModel
    {
        public virtual ICollection<ContractBaseViewModel> Contracts { get; set; }
    }
}
