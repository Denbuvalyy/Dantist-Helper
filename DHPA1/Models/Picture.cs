using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DHPA1.Models
{
    public class Picture
    {
        [Key]
        public int PictureId { get; set; }
        public string Caption { get; set; }
        public string PicturePath { get; set; }        
        public int? VisitId { get; set; }
        public Visit Visit { get; set; }        
        public int? ManipulationId { get; set; }
        public Manipulation Manipulation { get; set; } 
    }
}