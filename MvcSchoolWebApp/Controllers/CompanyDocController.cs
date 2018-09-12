using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Controllers
{
    public class CompanyDocController : Controller
    {
        // GET: CompanyDoc
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult displaydoc()
        {
            return View();
        }
    }
}