using System.Collections.Generic;

namespace HMS.Data.ViewModels.Clock
{
    public class ClockDetailViewModel
    {
        public int Id { get; set; }
        public int? RoomId { get; set; }
        public string ClockCategoryId { get; set; }
        public bool? Status { get; set; }

        public string ClockCategory { get; set; }
        public virtual ICollection<ClockValueDetailViewModel> ClockValues { get; set; }

    }
}
