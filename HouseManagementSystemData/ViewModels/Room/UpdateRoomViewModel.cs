using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.ViewModels.Room
{
    public class UpdateRoomViewModel
    {
        [Required]
        public int Id { get; set; }
        public double? RoomSquare { get; set; }
        public double? DefaultPrice { get; set; }
        public string Name { get; set; }
    }
}
