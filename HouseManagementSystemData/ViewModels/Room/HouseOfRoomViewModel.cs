using HMS.Data.ViewModels.HouseViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.Room
{
    public class HouseOfRoomViewModel : RoomBaseViewModel
    {
        public virtual HouseBaseViewModel House { get; set; }
    }
}
