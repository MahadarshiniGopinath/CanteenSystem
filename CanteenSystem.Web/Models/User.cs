using System;
using System.Collections.Generic;

#nullable disable

namespace CanteenSystem.Web.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
            UserRoles = new HashSet<UserRole>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public int RollNumber { get; set; }
        public string Department { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
