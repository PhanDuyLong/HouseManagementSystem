using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.Contract
{
    public class CreateContractViewModel
    {
        
        public string TenantUserId { get; set; }
        public int? RoomId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
      

    }
}
