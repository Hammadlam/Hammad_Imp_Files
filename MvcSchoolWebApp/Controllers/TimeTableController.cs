using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace MvcSchoolWebApp.Controllers
{
    public class TimeTableController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string popup_status;
        public static List<Users> user_dtl;
        // GET: TimeTable
        DatabaeseClass db;
        DatabaseInsertClass din;

        private Database.Database da = new Database.Database("Falconlocal");
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

        public ActionResult UploadTimetable()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[16].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            db = new DatabaeseClass();
            DatabaseModel timetable = new DatabaseModel();
            timetable.campus = db.getcampus();
            timetable.classes = db.getclass(timetable.campus[0].Value, user_id);

            timetable.campusid = timetable.campus[0].Value;
            timetable.classesid = "";

            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            timetable.section = sl;

            timetable.category = db.FillCategoryTimeTable();
            timetable.categoryid = timetable.category[0].Value;
            ViewBag.date = db.convertservertousertimezone(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            return View(timetable);
        }



        [HttpPost]
        public ActionResult UploadTimetable(HttpPostedFileBase postedFile, DatabaseModel timetable)
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[16].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            din = new DatabaseInsertClass();
            timetable.teacherid = user_id;
            timetable.enddate = timetable.begdate;

            if (timetable.campusid != null && timetable.categoryid != null)
            {
                if (postedFile != null)
                {
                    if (postedFile.ContentLength <= 25000000)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx", ".xls" };
                        var checkextqension = Path.GetExtension(postedFile.FileName).ToLower();
                        string path = Server.MapPath("~/Uploads/Timetables/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        string query3 = "select isnull(max(timetbid)+01, 01) from timetable";
                        da.CreateConnection();
                        da.InitializeSQLCommandObject(da.GetCurrentConnection, query3);
                        da.OpenConnection();
                        timetable.increment = da.obj_sqlcommand.ExecuteScalar().ToString();
                        da.CloseConnection();

                        timetable.filename = Path.GetFileName(timetable.increment + " - " + postedFile.FileName);
                        timetable.imagepath = path + timetable.filename;
                        postedFile.SaveAs(timetable.imagepath);
                        din.InsertTimetable(timetable);
                        if (popup_status == "Success")
                        {
                            ViewBag.Message = "File uploaded successfully.";
                            ViewBag.call_alert = "show_alert()";
                            ViewBag.message_popup = "File Successfully Uploaded";
                            ViewBag.cssclass = "alert-success";
                        }
                        else if (popup_status == "Rights")
                        {
                            ViewBag.Message = "No File Uploaded";
                            ViewBag.call_alert = "show_alert()";
                            ViewBag.message_popup = "You Do Not Have Rights";
                            ViewBag.cssclass = "alert-danger";
                        }
                        else
                        {
                            ViewBag.Message = "No File Uploaded";
                            ViewBag.call_alert = "show_alert()";
                            ViewBag.message_popup = "Found Some Error! Please Try Again";
                            ViewBag.cssclass = "alert-danger";
                        }
                    }
                    else
                    {
                        ViewBag.Message = "File Size must be Less than 25 MB";
                        ViewBag.call_alert = "show_alert()";
                        ViewBag.message_popup = "No File Uploaded";
                        ViewBag.cssclass = "alert-danger";
                    }
                }
                else
                {
                    ViewBag.Message = "No File Uploaded";
                    ViewBag.call_alert = "show_alert()";
                    ViewBag.message_popup = "Please Select a File to Upload";
                    ViewBag.cssclass = "alert-danger";
                }
            }
            else
            {
                ViewBag.Message = "All Fields must be selected";
            }

            db = new DatabaeseClass();

            timetable.campus = db.getcampus();
            timetable.campusid = timetable.campusid;

            timetable.classes = db.getclass(timetable.campusid, user_id);
            timetable.classesid = timetable.classesid;

            timetable.section = db.getsection(timetable.campusid, timetable.classesid, user_id);
            timetable.sectionid = timetable.sectionid;

            timetable.category = db.FillCategoryTimeTable();
            timetable.categoryid = timetable.categoryid;

            ViewBag.date = timetable.begdate;

            return View(timetable);
        }
        public ActionResult MasterTimetable()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[17].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            db = new DatabaeseClass();
            DatabaseModel timetable = new DatabaseModel();
            timetable.campus = db.getcampus();
            timetable.classes = db.getclass(timetable.campus[0].Value, user_id);

            timetable.campusid = timetable.campus[0].Value;
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            timetable.classesid = "";
            timetable.section = sl;
            return View(timetable);
        }
        public ActionResult ClassTimetable()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[19].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel timetable = new DatabaseModel();
            timetable.campus = db.getcampus();
            timetable.classes = db.getclass(timetable.campus[0].Value, user_id);
            timetable.section = db.getsection(timetable.campus[0].Value, timetable.classes[0].Value, user_id);

            if (user_role == "5000" || user_role == "4000")
            {
                timetable.campusid = timetable.campus[0].Value;
                timetable.classesid = timetable.classes[0].Value;
                timetable.sectionid = timetable.section[0].Value;
            }

            return View(timetable);
        }

        public ActionResult TeacherTimetable()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[18].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            db = new DatabaeseClass();
            DatabaseModel timetable = new DatabaseModel();
            timetable.campus = db.getcampus();
            timetable.campusid = timetable.campus[0].Value;
            return View(timetable);
        }
        [HttpPost]
        public JsonResult getClassJson(string campusId, string selectClass = null)
        {
            db = new DatabaeseClass();
            return Json(db.getclass(campusId, user_id));
        }

        [HttpPost]
        public JsonResult getSectionJson(String campusId, String classId, string selectsectionId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsection(campusId, classId, user_id));
        }

        public JsonResult getJQGridJsonMT(string campusId)
        {
            db = new DatabaeseClass();
            return Json(db.FillMasterTimeTable(campusId, user_role), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getJQGridJsonTT(string campusId)
        {
            db = new DatabaeseClass();
            return Json(db.FillTeacherTimeTable(campusId, user_role), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getJQGridJsonCT(string campusId, string classId, string sectionId)
        {
            db = new DatabaeseClass();
            return Json(db.FillClassTimeTable(campusId, classId, sectionId, user_role), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadTimetable(string reportid)
        {
            string file = Server.MapPath("~/Uploads/Timetables/" + reportid);
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            if (System.IO.File.Exists(file))
            {
                return File(file, contentType, Path.GetFileName(file));
            }
            else
            {
                return null;
            }
        }

        public JsonResult DeleteTimetable(string filename)
        {
            din = new DatabaseInsertClass();

            return Json(din.UpdateTimetable(filename, user_id), JsonRequestBehavior.AllowGet);
        }
    }
}