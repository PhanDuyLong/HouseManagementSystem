using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.HouseViewModels
{
    public class HouseHomeViewModel
    {
        public string Id { get; set; }
        public string OwnerUsername { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? Status { get; set; }

        public virtual HouseInfoViewModel HouseInfo { get; set; }
    }
}
