using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MvcSchoolWebApp.Controllers;

namespace MvcSchoolWebApp.Controllers
{
    public class ESSController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string upd_attd_popup;
        public static string pagetype;
        DatabaeseClass db;
        DatabaseInsertClass din;
        public static List<Users> user_dtl;
        public static string updatedqry;
        string cs = ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString;
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
        public ActionResult payslip()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[44].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            DatabaeseClass dc = new DatabaeseClass();
            ESSModel ess = new ESSModel();
            
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string query = "select Top 1 earea from empmain where empid = '" + user_id + "' and delind <> 'X' order by recordno desc";
            SqlCommand cmd = new SqlCommand(query, conn);
            ess.society = cmd.ExecuteScalar().ToString();

            conn.Close();
            DateTime lastmonth = dc.convertservertousertimezone(DateTime.Now.ToString());
            DateTime firstday = lastmonth.AddDays(1 - DateTime.Now.Day).AddMonths(-1);
            DateTime lastday = new DateTime(firstday.Year, firstday.Month, DateTime.DaysInMonth(firstday.Year, firstday.Month));

            ess.empid = user_id;
            
            ess.begdate = firstday.ToString("dd-MMMM-yyyy");
            ess.enddate = lastday.ToString("dd-MMMM-yyyy");
            return View(ess);
        }

        [HttpPost]
        public ActionResult payslip(ESSModel ess)
        {
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            string earea = "select distinct earea from empmain where empid = '" + user_id + "' and delind <> 'X'";
            SqlCommand cmd = new SqlCommand(earea, conn);
            string rptid = "ZHR001";
            ReportsController rc = new ReportsController();
            string[] arr = new string[7];
            arr[0] = ess.empid;
            arr[1] = ess.empid;
            arr[2] = Convert.ToDateTime(ess.begdate).ToString("yyyy-MM-dd");
            arr[3] = Convert.ToDateTime(ess.enddate).ToString("yyyy-MM-dd");
            arr[4] = "";
            arr[5] = cmd.ExecuteScalar().ToString();
            arr[6] = "";

            string updatedquery = rc.getupdatedquery(rc.getsquery(rptid), arr);
            System.IO.Stream stream = rc.getrptstream(rptid, updatedquery);
            return File(stream, "application/pdf");
        }

        public ActionResult leaveRequest()
        {          
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[45].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            db = new DatabaeseClass();
            List<ESSModel> track_loan = db.track_loan_status(user_id);
            if (track_loan != null)
            {
                ViewBag.trackinglist = track_loan;
            }

            ESSModel model = new ESSModel();
            db = new DatabaeseClass();
            List<ESSModel> dblist = db.getemploydetail(user_id);
            ViewBag.empid = dblist[0].empid;
            ViewBag.design = dblist[0].design;
            pagetype = "0210";
            model.loantyp = db.getLoanType(pagetype);
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");

            DatabaeseClass dc = new DatabaeseClass();
            DateTime currentday = dc.converteddisplaydate(DateTime.Now.ToString());
            ViewBag.date = currentday.ToString("dd-MMMM-yyyy");
            return View(model);
        }
        [HttpPost]
        public ActionResult leaveRequest(ESSModel model)
        {
            din = new DatabaseInsertClass();
            string msg = din.ESSLeaveInsert(model, user_id);

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[45].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            model = new ESSModel();
            db = new DatabaeseClass();
            List<ESSModel> dblist = db.getemploydetail(user_id);
            ViewBag.empid = dblist[0].empid;
            ViewBag.design = dblist[0].design;
            pagetype = "0210";
            model.loantyp = db.getLoanType(pagetype);
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");

            DatabaeseClass dc = new DatabaeseClass();
            DateTime currentday = dc.converteddisplaydate(DateTime.Now.ToString());
            ViewBag.date = currentday.ToString("dd-MMMM-yyyy");
            if (msg == "Success")
            {
                ViewBag.Message = msg;
            }
            else
            {
                ViewBag.Message = msg;
            }
            return View(model);
        }

        public ActionResult loanRequest()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[46].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            db = new DatabaeseClass();
            List<ESSModel> track_loan = db.track_loan_status(user_id);
            if (track_loan != null)
            {
                ViewBag.trackinglist = track_loan;
            }

            ESSModel model = new ESSModel();
            db = new DatabaeseClass();
            List<ESSModel> dblist = db.getemploydetail(user_id);
            DateTime dt = Convert.ToDateTime(dblist[0].joindate);
            DateTime dt2 = Convert.ToDateTime(dblist[0].confrdate);
            ViewBag.empid = dblist[0].empid;
            ViewBag.joindate = dt.ToString("dd-MMMM-yyyy");
            ViewBag.confrdate = dt2.ToString("dd-MMMM-yyyy");
            ViewBag.design = dblist[0].design;
            ViewBag.dept = dblist[0].dept;
            pagetype = "0360";
            model.loantyp = db.getLoanType(pagetype);
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            return View(model);
       }

        [HttpPost]
        public ActionResult loanRequest(ESSModel model)
        {
            din = new DatabaseInsertClass();
            string msg = din.ESSLoanInsert(model,user_id);
            
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[46].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            db = new DatabaeseClass();
            List<ESSModel> dblist = db.getemploydetail(user_id);
            DateTime dt = Convert.ToDateTime(dblist[0].joindate);
            DateTime dt2 = Convert.ToDateTime(dblist[0].confrdate);
            ViewBag.empid = dblist[0].empid;
            ViewBag.joindate = dt.ToString("dd-MMMM-yyyy");
            ViewBag.confrdate = dt2.ToString("dd-MMMM-yyyy");
            ViewBag.design = dblist[0].design;
            ViewBag.dept = dblist[0].dept;
            pagetype = "0360";
            model.loantyp = db.getLoanType(pagetype);
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");

            if (msg == "Success")
            {
                ViewBag.Message = msg;
            }
            else
            {
                ViewBag.Message = msg;
            }
            return View(model);
        }

        public ActionResult resignRequest()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[47].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            db = new DatabaeseClass();
            List<ESSModel> track_loan = db.track_loan_status(user_id);
            if (track_loan != null)
            {
                ViewBag.trackinglist = track_loan;
            }

            ESSModel model = new ESSModel();
            db = new DatabaeseClass();
            List<ESSModel> dblist = db.getemploydetail(user_id);
            DateTime dt = Convert.ToDateTime(dblist[0].joindate);
            DateTime dt2 = Convert.ToDateTime(dblist[0].confrdate);
            ViewBag.empid = dblist[0].empid;
            ViewBag.joindate = dt.ToString("dd-MMMM-yyyy");
            ViewBag.confrdate = dt2.ToString("dd-MMMM-yyyy");
            ViewBag.design = dblist[0].design;
            ViewBag.dept = dblist[0].dept;
            pagetype = "0977";
            model.loantyp = db.getLoanType(pagetype);
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            return View(model);
        }

        [HttpPost]
        public ActionResult resignRequest(ESSModel model)
        {
            din = new DatabaseInsertClass();
            string msg = din.ESSResigInsert(model, user_id);

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[46].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            db = new DatabaeseClass();
            List<ESSModel> dblist = db.getemploydetail(user_id);
            DateTime dt = Convert.ToDateTime(dblist[0].joindate);
            DateTime dt2 = Convert.ToDateTime(dblist[0].confrdate);
            ViewBag.empid = dblist[0].empid;
            ViewBag.joindate = dt.ToString("dd-MMMM-yyyy");
            ViewBag.confrdate = dt2.ToString("dd-MMMM-yyyy");
            ViewBag.design = dblist[0].design;
            ViewBag.dept = dblist[0].dept;
            pagetype = "0977";
            model.loantyp = db.getLoanType(pagetype);
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");

            if (msg == "Success")
            {
                ViewBag.Message = msg;
            }
            else
            {
                ViewBag.Message = msg;
            }
            return View(model);
        }

        public ActionResult purRequisit()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[56].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            ESSModel model = new ESSModel();
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            db = new DatabaeseClass();
            model.ordertyp = sl;
            model.orderno = sl;
            model.purdept = sl;
            model.purgrp = sl;
            model.saleordtyp = sl;
            model.costctrtyp = sl;
            model.cocode = sl;
            ViewBag.begdate = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            return View(model);
        }
    }
}