using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DHPA1.Models
{
    public class Visit
    {
        [Key]
        public int VisitId { get; set; }
        public DateTime VisitDate { get; set; }
        public string Description { get; set; }       
        public virtual ICollection<Picture> VisitPictures{ get; set; }       
    }
}