using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection.Emit;
using System.Web;

namespace MvcSchoolWebApp.Models
{
    public class DataModel : DbContext
    {
        public DbSet<Timesheetmodal> Timesheets
        {
            get;
            set;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Timesheetmodal>().ToTable("emptimesht");
        }

    }
} 