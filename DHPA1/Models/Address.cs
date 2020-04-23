using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DHPA1.Models
{
    [ComplexType]
    public class Address
    {        
        public string StreetName { get; set; }
        public string CityOrTown { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public int PostIndex { get; set; }     
    }
}