using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.Room
{
    public class CreateRoomViewModel
    {
        public double? RoomSquare { get; set; }
        public double? DefaultPrice { get; set; }
        public string HouseId { get; set; }
        public string Name { get; set; }
    }
}
