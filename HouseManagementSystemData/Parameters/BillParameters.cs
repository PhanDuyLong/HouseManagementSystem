using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.Parameters
{
    public class BillParameters
    {
        public int? RoomId { get; set; }
        public int? ContractId { get; set; }
        public bool? Status { get; set; }
        public bool? IsPaidInFull { get; set; }
        public bool IsIssueDateAscending { get; set; }
    }
}
