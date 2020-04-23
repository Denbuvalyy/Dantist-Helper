using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DHPA1.Models
{
    public class Manipulation
    {
        [Key]
        public int ManipulationId { get; set; }        
        public string Description { get; set; }
        public DateTime ManipulationDate { get; set; }
        public virtual ICollection<Picture> ManipulationPictures { get; set; }
    }
}