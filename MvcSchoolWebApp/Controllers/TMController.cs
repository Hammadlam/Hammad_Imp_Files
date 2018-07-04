using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Controllers
{
    public class TMController : Controller
    {
        // GET: TM
        public ActionResult TimeSheet()
        {
            return View();
        }
    }
}