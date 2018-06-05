using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcSchoolWebApp.Models;

namespace MvcSchoolWebApp.Controllers
{
    public class PreprimaryController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string popup_status;
        DatabaeseClass db;
        DatabaseInsertClass din;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["User_Role"] != null)
            {
                base.OnActionExecuting(filterContext);
                user_role = HttpContext.Session["User_Role"].ToString();
                user_id = HttpContext.Session["User_Id"].ToString();
            }

            else
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
        }

        public ActionResult PPMatric()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[32].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            PPMatricModel ppm = new PPMatricModel();
            DatabaeseClass db = new DatabaeseClass();
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            ppm.campus = db.getcampus();
            ppm.campusid = ppm.campus[0].Value;

            ppm.classes = db.getclass(ppm.campus[0].Value,user_id);
            ppm.classesid = "";

            ppm.teacher = db.FillTeacherName(user_id);

            ppm.section = sl;
            ppm.subject = sl;
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            return View(ppm);
        }

        public ActionResult PPCambridge()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[32].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            PPMatricModel ppc = new PPMatricModel();
            db = new DatabaeseClass();
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            ppc.campus = db.getcampus();
            ppc.campusid = ppc.campus[0].Value;

            ppc.classes = db.getclass(ppc.campus[0].Value, user_id);
            ppc.classesid = "";
            
            ppc.teacher = db.FillTeacherName(user_id);
            ppc.section = sl;
            ppc.subject = sl;

            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            return View(ppc);
        }

        [HttpPost]
        public JsonResult getClassJson(string campusId, string selectCityId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getclass(campusId, user_id));
        }


        [HttpPost]
        public JsonResult getSectionJson(string campusId, string classId, string selectCityId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsection(campusId, classId, user_id));
        }

        [HttpPost]
        public JsonResult getSubjectJson(string campusId, string classId, string sectionId, string teacherId, string selectsubjectId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsubject(campusId, classId, sectionId, user_id));
        }

        public JsonResult getJQGridJson(string campus, string classes, string section, string subject, string teacher, string date, string module)
        {
            db = new DatabaeseClass();
            return Json(db.FillPPMatric(campus, classes, section, subject, teacher, date, module), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getDataJQGridJson(string campus, string classes, string section, string subject, string teacher, string date, string module)
        {
            DatabaeseClass db = new DatabaeseClass();
            return Json(db.FillPPMatricData(campus, classes, section, subject, teacher, date, module), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPPCJQGridJson(string campus, string classes, string section, string subject, string teacher, string date, string module)
        {
            db = new DatabaeseClass();
            return Json(db.FillPPMatric(campus, classes, section, subject, teacher, date, module), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPPCDataJQGridJson(string campus, string classes, string section, string subject, string teacher, string date, string module)
        {
            DatabaeseClass db = new DatabaeseClass();
            return Json(db.FillPPMatricData(campus, classes, section, subject, teacher, date, module), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PPMarksSubmit(string campusId, string classId, string sectionId, string subjectId, string dateId, string[][] griddata, string[] colName)
        {
            din = new DatabaseInsertClass();
            din.PPMarksInsertion(campusId, classId, sectionId, subjectId,dateId, griddata, colName);
            return Json(HomeController.popup_status, JsonRequestBehavior.AllowGet);
        }
    }
}