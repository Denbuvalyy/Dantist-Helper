using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DHPA1.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string WorkPlace { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } 
        public Address Address { get; set; }
        public string Warnings { get; set; }
        public DateTime LastVisitDate { get; set; } 
        public virtual ICollection<Visit> Visits { get; set; }
        public virtual ICollection<Tooth> Teeth { get; set; }
        public virtual ICollection<Dantist>Dantists { get; set; }
        public string FullName()
        {
            return Surname +" "+ Name;
        }      
    }
}