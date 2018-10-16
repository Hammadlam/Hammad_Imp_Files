using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Controllers
{
    public class HRController : Controller
    {
        // GET: HR
        public ActionResult Applicantform()
        {
            return View();
        }
    }
}