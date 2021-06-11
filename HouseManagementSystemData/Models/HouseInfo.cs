using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class HouseInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool? Status { get; set; }
        public string HouseId { get; set; }
        public int? NumberOfRoom { get; set; }

        public virtual House House { get; set; }
    }
}
