using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace MvcSchoolWebApp.Controllers
{
    public class CompanyDocController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string pagename;
        public static string popup_status;
        private Database.Database da = new Database.Database("Falconlocal");

        Data.data data = new data();
        private string camp_id;
        private string emp_id;
        public static string[] popupinfo = new string[3];
        public static List<Users> user_dtl;

        DatabaeseClass db;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Session["User_Dtl"] != null)
            {
                user_dtl = (List<Users>)HttpContext.Session["User_Dtl"];
                base.OnActionExecuting(filterContext);
                user_role = HttpContext.Session["User_Role"].ToString();
                user_id = HttpContext.Session["User_Id"].ToString();

            }
            else
            {
                filterContext.Result = new RedirectResult("~/Login");
            }

        }
        // GET: CompanyDoc
        public ActionResult Index()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            user_role = HttpContext.Session["User_Role"].ToString();
            user_id = HttpContext.Session["User_Id"].ToString();
            if (list[68].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");          //user has no right to access this page, return to dashboard
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            db = new DatabaeseClass();
            CompanyDocModel model = new CompanyDocModel();
            model.dept = db.getDept();
            model.deptid = model.dept[0].Value;

            ViewBag.date = db.convertservertousertimezone(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile, CompanyDocModel model)
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[68].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            DatabaseInsertClass din = new DatabaseInsertClass();
            model.user_id = user_id;

            if (model.deptid != null)
            {
                if (postedFile != null)
                {
                    if (postedFile.ContentLength <= 25000000)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx", ".xls" };
                        var checkextqension = Path.GetExtension(postedFile.FileName).ToLower();
                        string path = Server.MapPath("~/Uploads/CorporateDoc/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        string query3 = "select isnull(max(docid)+01, 01) from corpdoc";
                        da.CreateConnection();
                        da.InitializeSQLCommandObject(da.GetCurrentConnection, query3);
                        da.OpenConnection();
                        model.increment = da.obj_sqlcommand.ExecuteScalar().ToString();
                        da.CloseConnection();

                        model.filename = Path.GetFileName(model.increment + " - " + postedFile.FileName);
                        model.imagepath = path + model.filename;
                        postedFile.SaveAs(model.imagepath);
                        din.InsertCompanyDoc(model);
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
            model.dept = db.getDept();
            model.deptid = model.dept[0].Value;
            ViewBag.date = model.date;
            return View(model);
        }

        public ActionResult AccountDoc()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            user_role = HttpContext.Session["User_Role"].ToString();
            user_id = HttpContext.Session["User_Id"].ToString();
            if (list[69].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");          //user has no right to access this page, return to dashboard
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }
        public ActionResult HRDoc()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            user_role = HttpContext.Session["User_Role"].ToString();
            user_id = HttpContext.Session["User_Id"].ToString();
            if (list[70].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");          //user has no right to access this page, return to dashboard
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }

        public JsonResult getJQGridJsonHRDoc()
        {
            db = new DatabaeseClass();
            return Json(db.FillHRDoc(user_role), JsonRequestBehavior.AllowGet);
        }


        public ActionResult ERPDoc()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            user_role = HttpContext.Session["User_Role"].ToString();
            user_id = HttpContext.Session["User_Id"].ToString();
            if (list[71].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");          //user has no right to access this page, return to dashboard
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }
        public ActionResult AOneDoc()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            user_role = HttpContext.Session["User_Role"].ToString();
            user_id = HttpContext.Session["User_Id"].ToString();
            if (list[72].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");          //user has no right to access this page, return to dashboard
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }
        public ActionResult BOneDoc()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            user_role = HttpContext.Session["User_Role"].ToString();
            user_id = HttpContext.Session["User_Id"].ToString();
            if (list[73].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");          //user has no right to access this page, return to dashboard
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }

        public ActionResult DownloadHRDoc(string reportid)
        {
            string file = Server.MapPath("~/Uploads/CorporateDoc/" + reportid);
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

        public JsonResult DeleteHRDoc(string filename, string category)
        {
            if (user_role == "1000")
            {
                DatabaseInsertClass din = new DatabaseInsertClass();
                return Json(din.UpdateCompanyDoc(filename, category, user_id), JsonRequestBehavior.AllowGet);
            }
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}