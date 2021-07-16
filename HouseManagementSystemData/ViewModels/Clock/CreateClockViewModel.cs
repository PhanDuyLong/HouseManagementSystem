using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.ViewModels.Clock
{
    public class CreateClockViewModel
    {
        [Required]
        public int RoomId { get; set; }

        public string ClockCategoryName { get; set; }
    }
}
