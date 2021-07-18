using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.ViewModels.Clock
{
    public class UpdateClockViewModel
    {
        [Required]
        public int Id { get; set; }
        public bool? Status { get; set; }

        public string ClockCategory { get; set; }
    }
}
