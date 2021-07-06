using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.HouseViewModels
{
    public class CreateHouseViewModel
    {
        public string OwnerUsername { get; set; }

        public virtual CreateHouseInfoViewModel HouseInfo { get; set; }
    }
}
