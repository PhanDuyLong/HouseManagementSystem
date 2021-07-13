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
            ServiceContracts = new HashSet<ServiceContract>();
        }

        public int Id { get; set; }
        public string OwnerUserId { get; set; }
        public string TenantUserId { get; set; }
        public int? RoomId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Status { get; set; }

        public virtual Room Room { get; set; }
        public virtual Account TenantUser { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<ServiceContract> ServiceContracts { get; set; }
    }
}
