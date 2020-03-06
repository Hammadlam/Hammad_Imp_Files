using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;

namespace MvcSchoolWebApp.Controllers
{
    public class LoginController : Controller
    {
        private string user_role;
        private string user_id;
        private string user_campus;
        private string user_class;
        private string user_section;
        private List<Users> user_dtl;
        private List<LoginModel> loginModel;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Session["User_Dtl"] != null)
            {
                user_dtl = (List<Users>)HttpContext.Session["User_Dtl"];
                base.OnActionExecuting(filterContext);
                user_role = user_dtl[0].user_earea;
                user_id = user_dtl[0].user_id;

                if (user_dtl[0].user_earea == "4000" || user_dtl[0].user_earea == "5000")
                {
                    filterContext.Result = new RedirectResult("~/dashboard/StudentDashboard");
                }
                else if (user_dtl[0].user_earea == "1000" || user_dtl[0].user_earea == "2000")
                {
                    filterContext.Result = new RedirectResult("~/dashboard");
                }
                else if (user_dtl[0].user_earea == "3000")
                {
                    filterContext.Result = new RedirectResult("~/TM/essTimeMgmt");
                }
            }

            else
            {
                //filterContext.Result = new RedirectResult("~/Login");
            }

        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Exception e = filterContext.Exception;
            filterContext.ExceptionHandled = true;
            filterContext.Result = new JsonResult()
            {
                Data = new
                {
                    View = "",
                    filterContext.Exception.Message,
                    filterContext.Exception.StackTrace
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // GET: Login
        public ActionResult Index()
        {
            if (HttpContext.Session["User_Dtl"] != null)
            {
                if (user_dtl[0].user_earea == "4000" || user_dtl[0].user_earea == "5000")
                {
                    return RedirectToAction("StudentDashboard", "dashboard");
                }
                else if (user_dtl[0].user_earea == "1000" || user_dtl[0].user_earea == "2000")
                {
                    return RedirectToAction("Index", "dashboard");
                }
                else if (user_dtl[0].user_earea == "3000")
                {
                    return RedirectToAction("TeacherDashboard", "dashboard");
                }
            }
            return View();
        }

        [HttpGet]
        public void usertimezone(String timezoneid)
        {
            System.Web.HttpContext.Current.Session["usr_timezone"] = timezoneid;
            DatabaeseClass.usr_timezone = timezoneid;
            MessageController msgc = new MessageController();
            msgc.Reset_msgtime();
        }

        [HandleError]
        [HttpPost]
        public ActionResult Index(Users users)
        {
            if (ModelState.IsValid)
            {
                string username = users.userName;
                string password = users.userPassword;
                DatabaeseClass db = new DatabaeseClass();
                try
                {
                    db.validatelogin(username, password);
                    if (HttpContext.Session["User_Dtl"] != null)
                    {
                        loginModel = (List<LoginModel>)HttpContext.Session["User_Rights"];
                        user_dtl = (List<Users>)HttpContext.Session["User_Dtl"];
                        if (loginModel[0].loginstatus == true)
                        {
                            user_id = HttpContext.Session["User_Id"].ToString();
                            user_role = HttpContext.Session["User_Role"].ToString();
                            db.Fill_usrdtl();
                            if (user_role == "1000" || user_role == "2000" || user_role == "3000")
                            {
                                //return RedirectToAction("index", "dashboard");
                                return RedirectToAction("essTimeMgmt", "TM");
                            }
                            //else if ()
                            //{
                            //    return RedirectToAction("TeacherDashboard", "dashboard");
                            //}
                            else if (user_role == "5000" || user_role == "4000")
                            {
                                return RedirectToAction("StudentDashboard", "dashboard");
                            }
                        }
                        else
                        {
                            ViewBag.call_alert = "show_alert();";
                            ViewBag.message_popup = "Invalid Username or Password";
                            ViewBag.cssclass = "alert-danger";
                        }
                    }
                    else if (HttpContext.Session["User_Dtl"] == null)
                    {
                        ViewBag.call_alert = "show_alert();";
                        ViewBag.message_popup = "Invalid Username or Password";
                        ViewBag.cssclass = "alert-danger";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.call_alert = "show_alert();";
                    ViewBag.message_popup = "Error Occured! (Error Details:  " + ex.Message + ")";
                    ViewBag.cssclass = "alert-danger";
                    return View();
                }
            }
            return View("Index", users);
        }
    }
}