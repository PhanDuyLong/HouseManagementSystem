using System;
using System.Collections.Generic;

#nullable disable

namespace HMSAPI.Models
{
    public partial class ServiceContract
    {
        public int Id { get; set; }
        public int? ContractId { get; set; }
        public string ServiceId { get; set; }
        public double? UnitPrice { get; set; }
        public bool? Status { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual Service Service { get; set; }
    }
}
