using System;
using System.Collections.Generic;

#nullable disable

namespace HouseManagementSystem.Data.Models
{
    public partial class ClockValue
    {
        public int Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public string ClockId { get; set; }
        public int? IndexValue { get; set; }
        public DateTime? RecordDate { get; set; }

        public virtual Clock Clock { get; set; }
    }
}
