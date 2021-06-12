using HMS.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels
{
    public class HouseDetailViewModel
    {
        public string Id { get; set; }
        public string OwnerUsername { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? Status { get; set; }

        public virtual HouseInfoViewModel HouseInfo { get; set; }
        public virtual ICollection<RoomBaseViewModel> Rooms { get; set; }
        public virtual ICollection<ServiceViewModel> Services { get; set; }
    }
}
