using HMS.Data.Models;
using HMS.Data.ViewModels.Contract.Base;
using HMS.Data.ViewModels.HouseViewModels;
using System;
using System.Collections.Generic;

namespace HMS.Data.ViewModels
{
    public class RoomBaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HouseId { get; set; }
        public bool? Status { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
