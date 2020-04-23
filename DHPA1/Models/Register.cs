using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DHPA1.Models
{
    public class Register
    {       
        public string Login { get; set; }
        public string Email { get; set; } 
        public string Password { get; set; }
        public string Repassword { get; set; }
        public bool UserExists { get; set; }  
        public bool Wrongpassword { get; set; } 
        public bool EmailExists { get; set; }
    }
}