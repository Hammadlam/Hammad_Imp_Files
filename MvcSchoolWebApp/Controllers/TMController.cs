using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Controllers
{
    public class TMController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string popup_status;
        public static List<Users> user_dtl;

        private Database.Database da = new Database.Database("Falconlocal");
        Data.data data = new data();
        private string idd = "1000000000";

        DatabaeseClass db;
        DatabaseInsertClass din;
        public System.Web.UI.Page Page { get; private set; }

        string getSquery = string.Empty;
        string UpdatedQuery = string.Empty;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Session["User_Dtl"] != null)
            {
                user_dtl = (List<Users>)HttpContext.Session["User_Dtl"];
                base.OnActionExecuting(filterContext);
                user_role = user_dtl[0].user_earea;
                user_id = user_dtl[0].user_id;
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
        }
        // GET: TM
        public ActionResult TimeSheet()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[61].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            return View();
        }
    }
}