using System;
using System.Collections.Generic;

namespace HMS.Data.ViewModels
{
    public class RoomBaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Status { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
