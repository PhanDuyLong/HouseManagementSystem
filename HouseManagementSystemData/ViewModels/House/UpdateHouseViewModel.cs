using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.ViewModels.House
{
    public class UpdateHouseViewModel
    {
        [Required]
        public string Id { get; set; }

        public virtual UpdateHouseInfoViewModel HouseInfo { get; set; }
    }
}
