using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace MvcSchoolWebApp.Controllers
{
    public class AdmissionController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static List<Users> user_dtl;
        DatabaeseClass db;


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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CampusStrength()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[0].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel admission = new DatabaseModel();
            admission.campus = db.getcampus();
            admission.campusid = admission.campus[0].Value;

            return View(admission);
        }

        public ActionResult CampusStatus()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[0].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel admission = new DatabaseModel();
            admission.campus = db.getcampus();
            admission.campusid = admission.campus[0].Value;
            
            return View(admission);
        }

        public ActionResult CampusCapacity()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[0].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel admission = new DatabaseModel();
            admission.campus = db.getcampus();
            admission.campusid = admission.campus[0].Value;
            
            return View(admission);
        }

        public JsonResult getJQGridJson(string campus)
        {
            db = new DatabaeseClass();
            return Json(db.FillCampusStrength(campus), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getCapacityJQGridJson(string campus)
        {
           db = new DatabaeseClass();
           return Json(db.FillCampusCapacity(campus), JsonRequestBehavior.AllowGet); 
        }
        
        public JsonResult getStatusJQGridJson(string campus)
        {
            db = new DatabaeseClass();
            return Json(db.FillAdmissionStatus(campus), JsonRequestBehavior.AllowGet);
        }

    }
}