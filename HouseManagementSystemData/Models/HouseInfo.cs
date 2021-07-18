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
        public int? PaidDeadline { get; set; }
        public int? BillDate { get; set; }
        public int? BeforeNotiDate { get; set; }
        public string HouseId { get; set; }
        public string Image { get; set; }

        public virtual House House { get; set; }
    }
}
