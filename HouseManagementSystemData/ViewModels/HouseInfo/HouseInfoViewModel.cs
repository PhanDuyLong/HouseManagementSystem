using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels
{
    public class HouseInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool? Status { get; set; }
        public string HouseId { get; set; }
        public int? NumberOfRoom { get; set; }
    }
}
