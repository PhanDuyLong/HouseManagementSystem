using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.HouseViewModels
{
    public class HouseBaseViewModel
    {
        public string Id { get; set; }
        public string OwnerUserId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? Status { get; set; }

        public virtual AccountDetailViewModel OwnerUser { get; set; }
        public virtual HouseInfoViewModel HouseInfo { get; set; }

    }
}
