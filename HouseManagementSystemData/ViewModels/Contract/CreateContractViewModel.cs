using HMS.Data.ViewModels.ServiceContract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.Data.ViewModels.Contract
{
    public class CreateContractViewModel
    {
        public string TenantUserId { get; set; }

        [Required]
        public int RoomId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? RoomPrice { get; set; }
        public string Note { get; set; }

        public virtual ICollection<CreateServiceContractViewModel> CreateServiceContracts { get; set; }
    }
}
