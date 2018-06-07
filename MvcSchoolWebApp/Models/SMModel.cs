using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Models
{
    public class SMModel
    {
        public List<SelectListItem> sotype { get; set; }
        public string sotypeid { get; set; }
        public List<SelectListItem> saleorder { get; set; }
        public string saleorderid { get; set; }
        public List<SelectListItem> custno { get; set; }
        public string custnoid { get; set; }
        public string po { get; set; }
        public string purdate { get; set; }
        public string orddate { get; set; }
        public string delvdate { get; set; }
        public List<SelectListItem> currtyp { get; set; }
        public string currtypid { get; set; }
        public List<SelectListItem> exrate { get; set; }
        public string exrateid { get; set; }
        public string exrateval { get; set; }
        public string interref { get; set; }
        public string val { get; set; }
        public List<SelectListItem> sorg { get; set; }
        public string sorgid { get; set; }
        public string jobno { get; set; }
        public List<SelectListItem> soledis { get; set; }
        public string soledisid { get; set; }
        public string cheqno { get; set; }
        public List<SelectListItem> prodgrp { get; set; }
        public string prodgrpid { get; set; }
        public List<SelectListItem> paytrm { get; set; }
        public string paytrmid { get; set; }
        public List<SelectListItem> salecity { get; set; }
        public string salecityid { get; set; }
        public List<SelectListItem> saleman { get; set; }
        public string salemanid { get; set; }
        public string refno { get; set; }
        public string refna { get; set; }
        public string perforno { get; set; }
        public string delvblk { get; set; }
        public string delvblkna { get; set; }
        public string billblk { get; set; }
        public string cominvc { get; set; }
        public string marksz { get; set; }
        public List<SelectListItem> incom { get; set; }
        public string incomid { get; set; }
        public string incomval { get; set; }
        public string creaby { get; set; }
        public string chgby { get; set; }
        public string creadate { get; set; }
        public string chgdate { get; set; }
        public string creatim { get; set; }
        public string chgtime { get; set; }
        public string stat { get; set; }
        public string currstat { get; set; }
    }
}