using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Models
{
    public class CompanyDocModel
    {
        public List<SelectListItem> dept { get; set; }
        public string deptid { get; set; }
        public string date { get; set; }
        public string user_id { get; set; }
        public string filename { get; set; }
        public string imagepath { get; set; }
        public string increment { get; set; }
        public string popup_status { get; set; }
    }
}