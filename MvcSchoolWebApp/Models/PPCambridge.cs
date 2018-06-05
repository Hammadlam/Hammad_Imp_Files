using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Models
{
    public class PPCambridge
    {
        public List<SelectListItem> campus { get; set; }
        public string campusid { get; set; }
        public List<SelectListItem> classes { get; set; }
        public string classesid { get; set; }
        public List<SelectListItem> section { get; set; }
        public string sectionid { get; set; }
        public List<SelectListItem> subject { get; set; }
        public string teachername { get; set; }
        public string date { get; set; }
        public List<SelectListItem> module { get; set; }
        public string moduleid { get; set; }



    }
}