using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Controllers
{
    public class ChatController : Controller
    {
        // GET: chat
        public ActionResult viewchat()
        {
            return View();
        }
    }
}