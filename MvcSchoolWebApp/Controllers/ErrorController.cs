using System;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}