using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels
{
    public class UpdateHouseInfoViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public int? PaidDeadline { get; set; }
        public int? BillDate { get; set; }
        public int? BeforeNotiDate { get; set; }
    }
}
