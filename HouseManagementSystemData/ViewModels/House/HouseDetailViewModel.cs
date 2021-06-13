using HMS.Data.Models;
using HMS.Data.ViewModels.HouseViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels
{
    public class HouseDetailViewModel : HouseBaseViewModel
    {
        public virtual ICollection<RoomBaseViewModel> Rooms { get; set; }
        public virtual ICollection<ServiceViewModel> Services { get; set; }
    }
}
