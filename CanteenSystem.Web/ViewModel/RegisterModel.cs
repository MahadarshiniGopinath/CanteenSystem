using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenSystem.Web.ViewModel
{
    public class RegisterModel
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; } 
        public int Rollnumber { get; set; } 
        public string Department { get; set; } 
        public string StudentEmail { get; set; }
        public bool IsParent { get; set; }
        public bool IsAdmin { get; set; }

        public RegisterModel()
        {

        }
        public RegisterModel(bool isAdmin =false)
        {
            IsAdmin = isAdmin;
        }
    }
}
