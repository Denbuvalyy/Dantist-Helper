using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DHPA1.Models
{
    public class Dantist
    {
        [Key]
        public int DantistId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string DantistAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } 
        public DantistWorkPlace WorkPlace { get; set; }
        public DateTime DoB { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }     
        public byte[] Salt { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
    }
}