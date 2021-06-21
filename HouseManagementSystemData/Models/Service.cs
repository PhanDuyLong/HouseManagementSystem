using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class Service
    {
        public Service()
        {
            ServiceContracts = new HashSet<ServiceContract>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string CalculationUnit { get; set; }
        public bool? Status { get; set; }
        public int? ServiceTypeId { get; set; }
        public string HouseId { get; set; }
        public double? Price { get; set; }

        public virtual House House { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual ICollection<ServiceContract> ServiceContracts { get; set; }
    }
}
