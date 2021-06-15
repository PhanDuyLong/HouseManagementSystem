using HMS.Data.ViewModels.Clock;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.ViewModels.ClockInContract
{
    public class ClockInContractDetailViewModel
    {
        public int Id { get; set; }
        public string ClockId { get; set; }
        public int? ContractId { get; set; }
        public bool? Status { get; set; }

        public virtual ClockDetailViewModel Clock { get; set; }
    }
}
