using HouseManagementSystem.Data.Models;
using System;
using System.Collections.Generic;

#nullable disable

namespace HouseManagementSystem.Data.Models
{
    public partial class BillItem
    {
        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string BillId { get; set; }
        public double? TotalPrice { get; set; }

        public virtual Bill Bill { get; set; }
        public virtual Service Service { get; set; }
    }
}
