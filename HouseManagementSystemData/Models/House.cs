using HouseManagementSystem.Data.Models;
using System;
using System.Collections.Generic;

#nullable disable

namespace HouseManagementSystem.Data.Models
{
    public partial class House
    {
        public House()
        {
            HouseInfos = new HashSet<HouseInfo>();
            Rooms = new HashSet<Room>();
        }

        public string Id { get; set; }
        public string OwnerUsername { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? Status { get; set; }

        public virtual Account OwnerUsernameNavigation { get; set; }
        public virtual ICollection<HouseInfo> HouseInfos { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
