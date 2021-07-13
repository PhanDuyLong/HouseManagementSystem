using HMS.Data.Models;
using System;
using System.Collections.Generic;

namespace HMS.Data.ViewModels.Contract.Base
{
    public class UpdateContractViewModel
    {
        public int? Id { get; set; }
        public string TenantUserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Image { get; set; }
    }
}
