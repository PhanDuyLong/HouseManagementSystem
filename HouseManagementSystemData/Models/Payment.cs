using HouseManagementSystem.Data.Models;
using System;
using System.Collections.Generic;

#nullable disable

namespace HouseManagementSystem.Data.Models
{
    public partial class Payment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public string BillId { get; set; }
        public int? Amount { get; set; }

        public virtual Bill Bill { get; set; }
    }
}
