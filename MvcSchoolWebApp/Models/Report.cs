using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSchoolWebApp.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public int ReportParam { get; set; }
        public string empid { get; set; }

        public string empname { get; set; }

        public string costorder { get; set; }

        public string cname { get; set; }


        public string visittype { get; set; }


        public string typetxt { get; set; }


    }
}