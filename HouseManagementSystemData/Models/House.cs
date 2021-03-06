using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class House
    {
        public House()
        {
            HouseInfos = new HashSet<HouseInfo>();
            Rooms = new HashSet<Room>();
            Services = new HashSet<Service>();
        }

        public string Id { get; set; }
        public string OwnerUserId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? Status { get; set; }

        public virtual Account OwnerUser { get; set; }
        public virtual ICollection<HouseInfo> HouseInfos { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
