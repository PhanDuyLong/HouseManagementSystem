using HMS.Data.ViewModels.Contract.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.RoomViewModels
{
    public class RoomDetailViewModel : RoomBaseViewModel
    {
        public virtual ContractDetailViewModel Contract { get; set; }
    }
}
