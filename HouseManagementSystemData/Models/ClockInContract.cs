using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class ClockInContract
    {
        public int Id { get; set; }
        public string ClockId { get; set; }
        public int? ContractId { get; set; }
        public bool? Status { get; set; }

        public virtual Clock Clock { get; set; }
        public virtual Contract Contract { get; set; }
    }
}
