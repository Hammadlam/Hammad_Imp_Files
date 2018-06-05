using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace MvcSchoolWebApp.Controllers
{
    public class FeeController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static List<Users> user_dtl;
        SqlConnection conn;
        string cs = ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString;

        // GET: Fee
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

        public ActionResult Feechalan()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[36].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel feemodel = new DatabaseModel();

            feemodel.campus = db.getcampus();
            feemodel.classes = db.getclass(feemodel.campus[0].Value, user_id);
            

            feemodel.campusid = feemodel.campus[0].Value;

            if (user_role == "5000" || user_role == "4000")
            {
                feemodel.section = db.getsection(feemodel.campus[0].Value, feemodel.classes[0].Value, user_id);
                feemodel.student = db.getstudentname(feemodel.campus[0].Value, feemodel.classes[0].Value, feemodel.section[0].Value);
                feemodel.subject = db.getsubject(feemodel.campus[0].Value, feemodel.classes[0].Value, feemodel.section[0].Value, user_id);
                feemodel.classesid = feemodel.classes[0].Value;
                feemodel.sectionid = feemodel.section[0].Value;
                feemodel.studentid = feemodel.student[0].Value;
                feemodel.subjectid = feemodel.subject[0].Value;
            }
            else
            {
                feemodel.section = db.EmptyList();
                feemodel.student = db.EmptyList();
                feemodel.subject = db.EmptyList();
                //feemodel.classesid = "";
                //feemodel.sectionid = "";
                //feemodel.studentid = "";
                //feemodel.subjectid = "";
            }
           
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("MMMM yyyy");
            return View(feemodel);
        }

        [HttpPost]
        public ActionResult Feechalan(DatabaseModel dm)
        {
            conn = new SqlConnection(cs);
            ReportsController rc = new ReportsController();
            string rptid = "sf001";
            string begdate = dm.feeMonth.ToString("yyyy/MM/") + "01";
            string enddate = dm.feeMonth.AddMonths(1).ToString("yyyy/MM/dd");
            string[] arr = new string[9];
            arr[0] = arr[1] = dm.classesid;
            arr[2] = dm.campusid;
            arr[3] = arr[4] = dm.studentid;
            arr[5] = begdate;
            arr[6] = enddate;
            arr[7] = dm.sectionid;
            arr[8] = "3000";
            string updatedquery = rc.getupdatedquery(rc.getsquery(rptid), arr);
            System.IO.Stream stream = rc.getrptstream(rptid, updatedquery);
            return File(stream, "application/pdf");
        }

        public ActionResult Feehistory()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[28].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel feemodel = new DatabaseModel();
            feemodel.campus = db.getcampus();

            feemodel.classes = db.getclass(feemodel.campus[0].Value, user_id);

            feemodel.campusid = feemodel.campus[0].Value;

            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            feemodel.section = sl;

            feemodel.student = sl;

            feemodel.classesid = "";
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("yyyy");
            //TempData.Keep("User_Role");
            return View(feemodel);
        }
        public ActionResult Feestatus()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            if (user_role == "3000")
            {
                return RedirectToAction("Index", "dashboard");
            }
            db = new DatabaeseClass();
            DatabaseModel feemodel = new DatabaseModel();
            feemodel.campus = db.getcampus();

            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            feemodel.section = sl;
            feemodel.classes = sl;

            feemodel.student = sl;

            //TempData.Keep("User_Role");
            return View(feemodel);
        }

        public JsonResult getJQGridJson()
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getClassJson(string campusId, string selectCityId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getclass(campusId,user_id));
        }

        [HttpPost]
        public JsonResult getSectionJson(string campusId, string classId, string selectId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsection(campusId, classId,user_id));
        }

        [HttpPost]
        public JsonResult getStudentJson(String campusId, String classId, string sectionId, string selectstudentId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getstudentname(campusId, classId,sectionId));
        }


    }
}