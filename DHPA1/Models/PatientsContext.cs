using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DHPA1.Models
{
    public class PatientsContext:DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Dantist> Dantists { get; set; }
        public PatientsContext():base("name=DefaultConnection")
        { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           base.OnModelCreating(modelBuilder);           
        }
        public DbSet<Picture> Pictures { get; set; }

        public DbSet<Visit> Visits { get; set; }

        public DbSet<Tooth>Teeth { get; set; }

        public DbSet<Manipulation>Manipulations { get; set; }
    }
}