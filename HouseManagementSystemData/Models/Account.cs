using System;
using System.Collections.Generic;

#nullable disable

namespace HMS.Data.Models
{
    public partial class Account
    {
        public Account()
        {
            Contracts = new HashSet<Contract>();
            Houses = new HashSet<House>();
        }

        public string Name { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool? Status { get; set; }
        public string Image { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<House> Houses { get; set; }
    }
}
