using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.Parameters
{
    public class ClockValueParameters
    {
        [Required]
        public int ClockId { get; set; }
        public bool? Status { get; set; }
        public bool IsRecordDateAscending { get; set; }
    }
}
