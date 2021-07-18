using HMS.Data.Models;
using HMS.Data.ViewModels.ServiceContract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HMS.Data.ViewModels.Contract.Base
{
    public class UpdateContractViewModel
    {
        [Required]
        public int Id { get; set; }
        public string TenantUserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ICollection<UpdateServiceContractViewModel> UpdateServiceContracts { get; set; }
    }
}
