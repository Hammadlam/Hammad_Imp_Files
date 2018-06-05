using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace MvcSchoolWebApp.Controllers
{
    public class AttendanceController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string upd_attd_popup;
        DatabaeseClass db;
        public static List<Users> user_dtl;

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

        public ActionResult MarkAttendance()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[32].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            db = new DatabaeseClass();
            DatabaseModel attendance = new DatabaseModel();

            attendance.campus = db.getcampus();
            attendance.campusid = attendance.campus[0].Value;
            attendance.classes = db.getclass(attendance.campus[0].Value, user_id);
            attendance.teachername = db.FillTeacherName(user_id);

            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            attendance.section = sl;
            attendance.subject = sl;

            ViewBag.date = db.convertservertousertimezone(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            
            return View(attendance);
        }

        public ActionResult ViewAttendance()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel attendance = new DatabaseModel();
            attendance.campus = db.getcampus();
            attendance.campusid = attendance.campus[0].Value;
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            attendance.classes = db.getclass(attendance.campus[0].Value, user_id);
            attendance.classesid = attendance.classes[0].Value;

            attendance.section = db.getsection(attendance.campus[0].Value, attendance.classes[0].Value, user_id);
            attendance.sectionid = attendance.section[0].Value;

            attendance.subject = db.getsubject(attendance.campus[0].Value, attendance.classes[0].Value, attendance.section[0].Value, user_id);

            attendance.stdname = db.getstudentname(attendance.campusid, attendance.classesid,attendance.sectionid);
            
            return View(attendance);
        }


        [HttpPost]
        public JsonResult getClassJson(string campusId, string selectCityId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getclass(campusId,user_id));
        }

       
        [HttpPost]
        public JsonResult getSectionJson(string campusId, string classId, string selectCityId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsection(campusId,classId,user_id));
        }

        [HttpPost]
        public JsonResult getSubjectJson(string campusId, string classId, string sectionId, string teacherId, string selectsubjectId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsubject(campusId, classId, sectionId, user_id));
        }
                        
        public JsonResult getJQGridJson(string campus, string classes, string section, string subject, DateTime date)
        {
            db = new DatabaeseClass();
            return Json(db.FillMarkAttendance(campus, classes, section, subject, date), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult MarkAttendance(string[] studentId, string[] status , string subjectId, DateTime dateId)

        {
            DatabaseInsertClass din = new DatabaseInsertClass();
            din.InsertAttendance(studentId, status, subjectId, dateId);
            return Json(AttendanceController.upd_attd_popup, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getstudentname(string campusId, string classId, string sectionId, string selectstudentnameid = null)
        {
            db = new DatabaeseClass();
            return Json(db.getstudentname(campusId, classId, sectionId));
        }

        public JsonResult getStudentFillJQGrid(string campusid, string classid, string sectionid, string studentid, DateTime dateid, string subjectid)
        {
            if (studentid == null || studentid == "")
                studentid = user_id;
            db = new DatabaeseClass();
            return Json(db.FillStudentAttendance(campusid, classid, sectionid, studentid, dateid, subjectid), JsonRequestBehavior.AllowGet);
        }
    }
} 