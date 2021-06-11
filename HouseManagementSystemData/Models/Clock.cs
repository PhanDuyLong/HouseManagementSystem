using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class Clock
    {
        public Clock()
        {
            ClockInContracts = new HashSet<ClockInContract>();
            ClockValues = new HashSet<ClockValue>();
        }

        public string Id { get; set; }
        public int? RoomId { get; set; }
        public string ClockCategoryId { get; set; }
        public bool? Status { get; set; }

        public virtual ClockCategory ClockCategory { get; set; }
        public virtual ICollection<ClockInContract> ClockInContracts { get; set; }
        public virtual ICollection<ClockValue> ClockValues { get; set; }
    }
}
