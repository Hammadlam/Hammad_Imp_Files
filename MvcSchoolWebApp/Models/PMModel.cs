using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Models
{
    public class PMModel
    {
        public List<SelectListItem> matno { get; set; }
        public string matnoid { get; set; }
        public List<SelectListItem> plant { get; set; }
        public string plantid { get; set; }
        public List<SelectListItem> refmat { get; set; }
        public string refmatid { get; set; }
        public List<SelectListItem> refplant { get; set; }
        public string refplantid { get; set; }
        public string valdate { get; set; }
        public List<SelectListItem> use { get; set; }
        public string useid { get; set; }
        public string baseqty { get; set; }
        public string baseqtyval { get; set; }
        public string flotsze { get; set; }
        public string tlotsze { get; set; }
        public string desc { get; set; }
        public string longtxt { get; set; }
        public string creausr { get; set; }
        public string chgusr { get; set; }
        public string creadate { get; set; }
        public string chgdate { get; set; }
    }
}