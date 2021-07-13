using HMS.Data.ViewModels.Clock;
using HMS.Data.ViewModels.Contract.Base;
using System.Collections.Generic;

namespace HMS.Data.ViewModels.RoomViewModels
{
    public class RoomDetailViewModel : RoomBaseViewModel
    {
        public virtual ContractDetailViewModel Contract { get; set; }
        public virtual ICollection<ClockDetailViewModel> Clocks { get; set; }

    }
}
