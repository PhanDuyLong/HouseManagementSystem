using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.Parameters
{
    public class BillParameters
    {
        public int? ContractId { get; set; }
/*        public DateTime? IssueDate { get; set; }*/
/*        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }*/
        public bool? Status { get; set; }
/*        public bool? IsDeleted { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsPaidInFull { get; set; }*/
        public bool IsIssueDateAscending { get; set; }
    }
}
