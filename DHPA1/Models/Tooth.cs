using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DHPA1.Models
{
    public class Tooth
    {
        [Key]
        public int ToothId { get; set; }
        public int Position { get; set; } 
        public string Description { get; set; }        
        public bool Manipulated { get; set; }
        public virtual ICollection<Manipulation> Manipulations { get; set; }        
    }
}