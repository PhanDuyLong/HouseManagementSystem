using HouseManagementSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HouseManagementSystem.Data.ViewModels
{
    public class HouseViewModel
    {
        public string Id { get; set; }
        public string OwnerUsername { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<HouseInfoViewModel> HouseInfoViewModels { get; set; }

    }
}
