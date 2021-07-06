using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class Clock
    {
        public Clock()
        {
            ClockValues = new HashSet<ClockValue>();
            ServiceContracts = new HashSet<ServiceContract>();
        }

        public string Id { get; set; }
        public int? RoomId { get; set; }
        public string ClockCategoryId { get; set; }
        public bool? Status { get; set; }

        public virtual ClockCategory ClockCategory { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<ClockValue> ClockValues { get; set; }
        public virtual ICollection<ServiceContract> ServiceContracts { get; set; }
    }
}
