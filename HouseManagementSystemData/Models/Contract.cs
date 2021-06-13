using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class Contract
    {
        public Contract()
        {
            Bills = new HashSet<Bill>();
            ClockInContracts = new HashSet<ClockInContract>();
            ServiceContracts = new HashSet<ServiceContract>();
        }

        public int Id { get; set; }
        public string OwnerUsername { get; set; }
        public string TenantUsername { get; set; }
        public int? RoomId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Status { get; set; }
        public string Image { get; set; }

        public virtual Room Room { get; set; }
        public virtual Account TenantUsernameNavigation { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<ClockInContract> ClockInContracts { get; set; }
        public virtual ICollection<ServiceContract> ServiceContracts { get; set; }
    }
}
