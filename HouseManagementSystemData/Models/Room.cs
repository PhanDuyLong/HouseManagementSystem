using System;
using System.Collections.Generic;

#nullable disable

namespace HouseManagementSystem.Data.Models
{
    public partial class Room
    {
        public Room()
        {
            Contracts = new HashSet<Contract>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string HouseId { get; set; }
        public bool? Status { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual House House { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
