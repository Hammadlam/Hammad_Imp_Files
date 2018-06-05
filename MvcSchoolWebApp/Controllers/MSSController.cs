using MvcSchoolWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MvcSchoolWebApp.Controllers
{
    public class MSSController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string upd_attd_popup;
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

        
        public ActionResult resignApproval()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[50].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            return View();
        }

        public ActionResult loanApproval()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[49].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            db = new DatabaeseClass();
            ESSModel model = new ESSModel();
            model.recodtype = db.getRecodType();
            model.loantyp = sl;
            model.apprrecodtyp = sl;
            return View(model);
        }

        public string GetLoanAppr(String reqtyp)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("select e78.empid as empid, ep.firstname +''+ ep.lastname as empname, e78.apprdate as dates, " +
                "e77.paymamt as loanamt, apstat.appstattxt as loanstat, apstats.appstattxt as overalstat " +
                "from emp0378 e78 inner join emppers ep on e78.empid = ep.empid " +
                "inner join emp0377 e77 on e78.empid = e77.empid and e78.begdate = e77.begdate and e78.enddate = e77.enddate "+ 
                "inner join hrappstat apstat on e78.apprstat = apstat.appstat " +
                "inner join hrappstat apstats on e77.reqstat = apstats.appstat "+
                "where e78.approver = '" + user_id + "' and e78.reqtype = '10' ", con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            if(col.ColumnName == "dates")
                            {
                                row.Add(col.ColumnName, Convert.ToDateTime(dr[col]).ToString("dd-MMMM-yyyy"));
                            }
                            else
                                row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);

                    }
                    var jsonSerialiser = new JavaScriptSerializer();
                    string jsonvalue = JsonConvert.SerializeObject(rows, Formatting.None);
                    return jsonvalue;
                }
            }
        }

        public JsonResult getloanDetails(string empId)
        {
            db = new DatabaeseClass();
            return Json(db.getLoanDetails(empId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult rejectRecomendloan(string empid, string reqno, string subpagtype,string comment, string loanamt)
        {
            
            din = new DatabaseInsertClass();
            din.rejectRecodLoan(empid,reqno,subpagtype,comment,loanamt,user_id);
            return Json(HomeController.popup_status,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult loanRecommend(ESSModel model)
        {
            din = new DatabaseInsertClass();
            din.loanrecodTo(model, user_id);
            return RedirectToAction("loanApproval", "MSS");
        }

        public JsonResult acceptLoanByRecomder(string empid, string reqno, string subpagtype, string startdate, string enddate, string apprrecodtypid, string comment, string reqtype)
        {
            din = new DatabaseInsertClass();
            if (apprrecodtypid != "")
            {
                din.apprLoanRecomended(empid, reqno, subpagtype, startdate, enddate, apprrecodtypid, comment, reqtype, user_id);
            }
            else
            {
                din.acceptLoanByRecomder(empid, reqno, subpagtype, startdate, enddate, apprrecodtypid, comment, reqtype, user_id);
            }
            return Json(HomeController.popup_status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult rejectApprloan(string empid, string reqno, string subpagtype, string comment, string loanamt)
        {

            din = new DatabaseInsertClass();
            din.rejectApprLoan(empid, reqno, subpagtype, comment, loanamt, user_id);
            return Json(HomeController.popup_status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult loanApprovalAsPerPolicy(ESSModel model)
        {
            din = new DatabaseInsertClass();
            din.loanAcceptedAsPerPolicy(model, user_id);
            return RedirectToAction("loanApproval", "MSS");
        }

        [HttpPost]
        public JsonResult getApprRecodType(string empId, string reqtype )
        {
            db = new DatabaeseClass();
            return Json(db.getApprRecodType(empId, reqtype,user_id));
        }

        /***************************LEAVE APPROVAL**************************************/
        public ActionResult leaveApproval()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[48].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            db = new DatabaeseClass();
            ESSModel model = new ESSModel();
            model.recodtype = db.getRecodType();
            model.loantyp = sl;
            model.apprrecodtyp = sl;
            return View(model);
        }

        public string GetLeaveAppr(String reqtyp)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("select e78.empid as empid, ep.firstname + '' + ep.lastname as empname, e78.apprdate as dates, " +
                "e77.lvdays as lvdays, apstat.appstattxt as leavestat, apstats.appstattxt as overalstat " +
                "from emp0378 e78 inner join emppers ep on e78.empid = ep.empid " +
                "inner join emp0277 e77 on e78.empid = e77.empid and e78.begdate = e77.begdate and e78.enddate = e77.enddate " +
                "inner join hrappstat apstat on e78.apprstat = apstat.appstat " +
                "inner join hrappstat apstats on e77.reqstat = apstats.appstat " +
                "where e78.approver = '"+user_id+"' and e78.reqtype = '"+ reqtyp +"' ", con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName == "dates")
                            {
                                row.Add(col.ColumnName, Convert.ToDateTime(dr[col]).ToString("dd-MMMM-yyyy"));
                            }
                            else
                                row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);

                    }
                    var jsonSerialiser = new JavaScriptSerializer();
                    string jsonvalue = JsonConvert.SerializeObject(rows, Formatting.None);
                    return jsonvalue;
                }
            }
        }

        public JsonResult getleaveDetails(string empId)
        {
            db = new DatabaeseClass();
            return Json(db.getLeaveDetails(empId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult rejectRecomendleave(string empid, string reqno, string subpagtype, string comment, string totdays)
        {

            din = new DatabaseInsertClass();
            din.rejectRecodLeave(empid, reqno, subpagtype, comment, totdays, user_id);
            return Json(HomeController.popup_status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult leaveRecommend(ESSModel model)
        {
            din = new DatabaseInsertClass();
            din.leaverecodTo(model, user_id);
            return RedirectToAction("leaveApproval", "MSS");
        }

        public JsonResult acceptLeaveByRecomder(string empid, string reqno, string subpagtype, string startdate, string enddate, string apprrecodtypid, string comment, string reqtype)
        {
            din = new DatabaseInsertClass();
            if (apprrecodtypid != "")
            {
                din.apprLeaveRecomended(empid, reqno, subpagtype, startdate, enddate, apprrecodtypid, comment, reqtype, user_id);
            }
            else
            {
                din.acceptLeaveByRecomder(empid, reqno, subpagtype, startdate, enddate, apprrecodtypid, comment, reqtype, user_id);
            }
            return Json(HomeController.popup_status, JsonRequestBehavior.AllowGet);
        }

        public JsonResult rejectApprleave(string empid, string reqno, string subpagtype, string comment, string lvdays)
        {

            din = new DatabaseInsertClass();
            din.rejectApprLeave(empid, reqno, subpagtype, comment, lvdays, user_id);
            return Json(HomeController.popup_status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult leaveApprovalAsPerPolicy(ESSModel model)
        {
            din = new DatabaseInsertClass();
            din.leaveAcceptedAsPerPolicy(model, user_id);
            return RedirectToAction("leaveApproval", "MSS");
        }

        /***************************LEAVE APPROVAL**************************************/
    }
}