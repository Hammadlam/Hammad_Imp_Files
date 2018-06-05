using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;

namespace MvcSchoolWebApp.Controllers
{
    public class ResultController : Controller
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

        // GET: Result
        public ActionResult Index()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            db = new DatabaeseClass();
            DatabaseModel rusl = new DatabaseModel();

            rusl.campus = db.getcampus();
            rusl.classes = db.getclass(rusl.campus[0].Value, user_id);
            rusl.campusid = rusl.campus[0].Value;
            
            if (user_role == "5000" || user_role == "4000")
            {
                rusl.section = db.getsection(rusl.campus[0].Value, rusl.classes[0].Value, user_id);
                rusl.stdname = db.getstudentname(rusl.campus[0].Value, rusl.classes[0].Value, rusl.section[0].Value);
                rusl.subject = db.getsubject(rusl.campus[0].Value, rusl.classes[0].Value, rusl.section[0].Value, user_id);
                rusl.classesid = rusl.classes[0].Value;
                rusl.sectionid = rusl.section[0].Value;
                rusl.studentid = rusl.stdname[0].Value;
                rusl.subjectid = rusl.subject[0].Value;
            }
            else if (user_role == "1000" || user_role == "2000" || user_role == "3000")
            {
                rusl.classesid = "";
                //rusl.sectionid = "";
                //rusl.studentid = "";
                //rusl.subjectid = "";
                rusl.section = sl;
                rusl.stdname = sl;
                rusl.subject = sl;
            }

            

            return View(rusl);
        }
        
        public JsonResult fncresulttype(string dataToSave)
        {
            string getvalue = dataToSave;
            string url = "";
            try
            {
                Session["resulttype"] = getvalue;
                url = "/result/index";
            }
            catch (Exception)
            {
                Session["resulttype"] = "Weekly Test";
            }
            return Json(url, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Quiz()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }

        public ActionResult MidTerm()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }

        public ActionResult PublishResult()
        {
            MessageCls msgobj = new MessageCls();
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            DatabaseModel dm = new DatabaseModel();
            DatabaeseClass dc = new DatabaeseClass();
            dm.campus = dc.getcampus();
            dm.campusid = dm.campus[0].Value;

            //dm.classes = dc.getclass(dm.campus[0].Value, user_id);
            //dm.classesid = "";

            dm.module = dc.FillSubModule();
            dm.moduleid = "";

            return View(dm);
        }

        [HttpPost]
        public ActionResult PublishResult(DatabaseModel dm)
        {
            MessageCls msgobj = new MessageCls();
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            DatabaeseClass dc = new DatabaeseClass();
            dm.campus = dc.getcampus();
            dm.campusid = dm.campusid;

            //dm.classes = dc.getclass(dm.campus[0].Value, user_id);
            //dm.classesid = dm.classesid;

            dm.module = dc.FillSubModule();
            dm.moduleid = dm.moduleid;

            ViewBag.resultlist = dc.getpublishresult(dm.campusid, dm.classesid, dm.moduleid);
            return View(dm);
        }

        [HttpPost]
        public JsonResult submitpublishresult(string campusid, string moduleid, string[] pubstat, string[] classid, string[] sectionid)
        {
            DatabaseInsertClass dc = new DatabaseInsertClass();
            dc.insertpublishresult(campusid, moduleid, pubstat, classid, sectionid);
            return Json(HomeController.popup_status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getstudentname(string campusId, string classId, string sectionId, string selectstudentnameid = null)
        {
            db = new DatabaeseClass();
            return Json(db.getstudentname(campusId, classId, sectionId));
        }

        [HttpPost]
        public JsonResult getClassJson(string campusId, string selectCityId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getclass(campusId,user_id));
        }
       
        public JsonResult getSectionJson(String campusId, String classId, string selectsectionId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsection(campusId, classId,user_id));
        }

        [HttpPost]
        public JsonResult getsubjectJSON(String campusId, String classId, String sectionId, string nameId, string selectsubject = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsubject(campusId, classId, sectionId, user_id));
        }
        public JsonResult getsessionJSON(String campusId, String classId, String sectionId, string nameId, string selectsubject = null)
        {
            db = new DatabaeseClass();
            return Json(db.FillSubject(campusId, classId, sectionId, user_id));
        }
        public JsonResult getfinalreports(string campusId, string classId, string sectionId, string moduleId)
        {
            db = new DatabaeseClass();
            return Json(db.FillFinalReports(campusId, classId, sectionId, moduleId));
        }


        public JsonResult getGridJson(string nameId, string subjectid, string campus, string classes, string section)
        {
            db = new DatabaeseClass();
            string resulttype = Session["resulttype"].ToString();
            return Json(db.FillWeeklyTest(nameId, subjectid,campus, classes, section,resulttype), JsonRequestBehavior.AllowGet);
        }

    }
}