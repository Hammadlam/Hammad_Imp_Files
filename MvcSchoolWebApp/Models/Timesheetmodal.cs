using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Models
{
    public class Timesheetmodal
    {
        public int id { get; set; }
         public string empid { get; set; }
        public string  Name { get; set; }
         public string Client{ get; set; }
        public DateTime Checkindt { get; set; }
        public DateTime Checkoutdt { get; set; }
    }
}