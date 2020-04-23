using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DHPA1.Models
{
    public class CreateVisit
    {
        public DateTime VisitDate { get; set; }
        public string Description { get; set; }
        public int PatientsId { get; set; } 
    }
}