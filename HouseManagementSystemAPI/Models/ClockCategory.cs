using System;
using System.Collections.Generic;

#nullable disable

namespace HMSAPI.Models
{
    public partial class ClockCategory
    {
        public ClockCategory()
        {
            Clocks = new HashSet<Clock>();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Clock> Clocks { get; set; }
    }
}
