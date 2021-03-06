using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels
{
    public class UpdateClockValueViewModel
    {
        public int Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? IndexValue { get; set; }
        public DateTime? RecordDate { get; set; }
        public bool? Status { get; set; }
    }
}
