using HMS.Data.Models;
using System;
using System.Collections.Generic;

namespace HMS.Data.ViewModels.Contract.Base
{
    public class ContractBaseViewModel
    {
        public int Id { get; set; }
        public string OwnerUserId { get; set; }
        public string TenantUserId { get; set; }
        public int RoomId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Status { get; set; }

        public virtual AccountDetailViewModel TenantUser { get; set; }


    }
}
