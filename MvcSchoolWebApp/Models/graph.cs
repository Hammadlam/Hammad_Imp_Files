using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSchoolWebApp.Models
{
    public class graph
    {

        public string Moduleid { get; set; }
        public string Modulename { get; set; }
        public string Graphname { get; set; }
        public string venderid { get; set; }
        public string Vendername { get; set; }
        public string count { get; set; }
        public string sforid { get; set; }

        public string module_id { get; set; }
        public string filter_id { get; set; }
        public string filter_name { get; set; }
        public string filter_val { get; set; }

        public string[] filters { get; set; }
    }
}