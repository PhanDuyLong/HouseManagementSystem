using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels
{
    public class BillItemDetailViewModel
    {
        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string BillId { get; set; }
        public double? TotalPrice { get; set; }

        public virtual ServiceViewModel Service { get; set; }
    }
}
