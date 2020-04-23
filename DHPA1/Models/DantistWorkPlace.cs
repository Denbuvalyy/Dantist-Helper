using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DHPA1.Models
{
    [ComplexType]
    public class DantistWorkPlace
    {        
        public bool Private { get; set; }
        public string PlaceName { get; set; }
        public string City { get; set; }
        public int HouseNumber { get; set; }
        public string StreetName { get; set; }     
    }
}