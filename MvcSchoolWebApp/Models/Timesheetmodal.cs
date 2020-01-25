using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Models
{
    public class Timesheetmodal
    {
        public int serial { get; set; }
        public int id { get; set; }
        public string empdesignation { get; set; }
        public string empdepart { get; set; }
        public string empid { get; set; }
        public List<SelectListItem> empname { get; set; }
        public string employeename { get; set; }
        public string Name { get; set; }
        public string clientid { get; set; }
        public string client { get; set; }
        public string date { get; set; }
        public string day { get; set; }
        public string month { get; set; }
        public string time { get; set; }
        public List<SelectListItem> clientname { get; set; }
        public string checkintime { get; set; }
        public string tinlat { get; set; }
        public string toutlat { get; set; }
        public string tinlong { get; set; }
        public string toutlong { get; set; }
        public string checkouttime { get; set; }
        public DateTime Checkindt { get; set; }
        public DateTime Checkoutdt { get; set; }
        public string noofvisit { get; set; }
        public string remarks { get; set; }
        public string remarkstout { get; set; }
        public string rates { get; set; }
        public string total { get; set; }
        public string totalkm { get; set; }
        public string rateperkm { get; set; }
    }
}