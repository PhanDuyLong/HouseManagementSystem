using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.ViewModels
{
    public class CreateClockValueViewModel
    {
        public DateTime? CreateDate { get; set; }
        [Required]
        public int ClockId { get; set; }
        public int? IndexValue { get; set; }
        public DateTime? RecordDate { get; set; }
    }
}
