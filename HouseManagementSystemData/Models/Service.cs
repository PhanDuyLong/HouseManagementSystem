using HouseManagementSystem.Data.Models;
using System;
using System.Collections.Generic;

#nullable disable

namespace HouseManagementSystem.Data.Models
{
    public partial class Service
    {
        public Service()
        {
            BillItems = new HashSet<BillItem>();
            ServiceContracts = new HashSet<ServiceContract>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string CalculationUnit { get; set; }
        public bool? Status { get; set; }
        public int? ServiceTypeId { get; set; }

        public virtual ServiceType ServiceType { get; set; }
        public virtual ICollection<BillItem> BillItems { get; set; }
        public virtual ICollection<ServiceContract> ServiceContracts { get; set; }
    }
}
