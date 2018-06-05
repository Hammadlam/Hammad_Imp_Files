using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Models
{
    public class Personalinfo
    {

        public int txtid { get; set; }
        public string empid { get; set; }
        public int roleid { get; set; }
        public string roletxt { get; set; }
        public string status { get; set; }
        public string title { get; set; }
        public List<SelectListItem> campus { get; set; }
        public string campusid { get; set; }
        public string txtcampus { get; set; }
        public string department { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string firstname { get; set; }
        public string fathername { get; set; }
        public string secondname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string gender { get; set; }
        public string nic { get; set; }
        public string image { get; set; }
        public string paddress { get; set; }
        public string careof { get; set; }
        public string paddress2 { get; set; }
        public string zipcode { get; set; }
        public string phone { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string nationality { get; set; }
        public string dob { get; set; }
        public string initial { get; set; }
        public string birthplace { get; set; }
        public string birthcountry { get; set; }
        public string country { get; set; }
        [Required(ErrorMessage ="Please enter the ID")]
        public string textbox { get; set; }
        public string txtaddtype { get; set; }
        public List<SelectListItem> employeeid { get; set; }
        public List<SelectListItem> classes { get; set; }
        public List<SelectListItem> section { get; set; }
        public string classId { get; set; }
        public string sectionId { get; set; }
        public string responsibilites { get; set; }

    }
}