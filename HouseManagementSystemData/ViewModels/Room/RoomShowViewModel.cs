using HMS.Data.ViewModels.Contract.Base;
using System;
using System.Collections.Generic;

namespace HMS.Data.ViewModels
{
    public class RoomShowViewModel : RoomBaseViewModel
    {
        public virtual ContractBaseViewModel Contract { get; set; }
    }
}
