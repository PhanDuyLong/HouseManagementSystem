using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class Payment
    {
        public int Id { get; set; }
        public int? BillId { get; set; }
        public DateTime? Date { get; set; }
        public double? Amount { get; set; }
        public string Note { get; set; }
        public bool? Status { get; set; }
        public bool? IsSend { get; set; }

        public virtual Bill Bill { get; set; }
    }
}
