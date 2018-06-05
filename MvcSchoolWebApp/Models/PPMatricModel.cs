using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Models
{
    public class PPMatricModel
    {
        public List<SelectListItem> campus { get; set; }
        public string campusid { get; set; }

        public List<SelectListItem> classes { get; set; }
        public string classesid { get; set; }
        public List<SelectListItem> section { get; set; }
        public string sectionid { get; set; }
        public string teacher { get; set; }
        public string date { get; set; }
        public List<SelectListItem> module { get; set; }
        public string moduleid { get; set; }
        public List<SelectListItem> subject { get; set; }
        public string subjectid { get; set; }
        public string collabl { get; set; }
        public string field { get; set; }
        public string fieldid { get; set; }
        public string status { get; set; }
        public string studentName { get; set; }
        public string col0 { get; set; }
        public string col1 { get; set; }
        public string col2 { get; set; }
        public string col3 { get; set; }
        public string col4 { get; set; }
        public string col5 { get; set; }
        public string col6 { get; set; }
        public string col7 { get; set; }
        public string col8 { get; set; }
        public string col9 { get; set; }

        public string col10 { get; set; }
        public string col11 { get; set; }
        public string col12 { get; set; }
        public string col13 { get; set; }
        public string col14 { get; set; }
        public string col15 { get; set; }
        public string col16 { get; set; }

        public string[,] array { get; set; }
    }
}