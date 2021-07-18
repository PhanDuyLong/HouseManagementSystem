using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class Room
    {
        public Room()
        {
            Clocks = new HashSet<Clock>();
            Contracts = new HashSet<Contract>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string HouseId { get; set; }
        public double? RoomSquare { get; set; }
        public double? DefaultPrice { get; set; }
        public bool? Status { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual House House { get; set; }
        public virtual ICollection<Clock> Clocks { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
