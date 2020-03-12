using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Models
{
    public class ESSModel
    {
        public string empid { get; set; }
        public string begdate { get; set; }
        public string enddate { get; set; }
        public string society { get; set; }
        public string joindate { get; set; }
        public string confrdate { get; set; }
        public string design { get; set; }
        public string dept { get; set; }
        public string loanamt { get; set; }
        public List<SelectListItem> loantyp { get; set; }
        public string loanid { get; set; }
        public string lastdate { get; set; }
        public string repaydate { get; set; }
        public string payperiod { get; set; }
        public string lontintl { get; set; }

        public List<SelectListItem> recodtype { get; set; }
        public string recodid { get; set; }
        public string empname { get; set; }
        public string recordno { get; set; }
        public string coments { get; set; }

        public List<SelectListItem> apprrecodtyp { get; set; }
        public string apprrecodtypid { get; set; }

        public string appvrid { get; set; }
        public string loan_status { get; set; }
        public string comments { get; set; }
        public string fdayleave { get; set; }
        public string ldayleave { get; set; }
        public string totdays { get; set; }
        public string reason { get; set; }

        public List<SelectListItem> ordertyp { get; set; }
        public string ordertypid { get; set; }

        public List<SelectListItem> orderno { get; set; }
        public string ordernoid { get; set; }

        public List<SelectListItem> purdept { get; set; }
        public string purdeptid { get; set; }

        public List<SelectListItem> purgrp { get; set; }
        public string purgrpid { get; set; }

        public List<SelectListItem> costctrtyp { get; set; }
        public string costctrtypid { get; set; }
        public List<SelectListItem> saleordtyp { get; set; }
        public string saleordtypid { get; set; }
        public string hdrtxt { get; set; }
        public List<SelectListItem> cocode { get; set; }
        public string cocodeid { get; set; }
        public string prtxt { get; set; }
        public string entterm { get; set; }
        public string status { get; set; }
        public DateTime month { get; set; }

    }
}