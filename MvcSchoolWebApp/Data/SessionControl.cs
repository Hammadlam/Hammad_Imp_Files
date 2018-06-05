using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Data
{
    public class SessionControl : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["User_Role"] != null)
                base.OnActionExecuting(filterContext);
            else
                filterContext.Result = new RedirectResult("/Login");
        }

        public ActionResult check_session(string Session_Name)
        {
            string actionname = "";
            string controllername = "";

            if (Session_Name == "admin")
            {

            }
            else if (Session_Name == "Parent")
            {

            }
            else if (Session_Name == "Teacher")
            {

            }
            else if (Session_Name == "student")
            {
                actionname = "StudentDashboard";
                controllername = "dashboard";
            }

            return RedirectToAction(actionname,controllername);
            
        }
    }
}