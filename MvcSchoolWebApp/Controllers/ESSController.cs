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
using System.Collections;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Web.Script.Serialization;

namespace MvcSchoolWebApp.Controllers
{
    public class ESSController : Controller
    {
        public Database.Database da = new Database.Database("Falconlocal");
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
            string rptid = "HR2003";
            ReportsController rc = new ReportsController();
            string[] arr = new string[7];
            arr[0] = ess.empid;
            arr[1] = ess.empid;
            arr[2] = Convert.ToDateTime(ess.begdate).ToString("yyyy-MM-dd");
            arr[3] = Convert.ToDateTime(ess.enddate).ToString("yyyy-MM-dd");
            arr[4] = "";
            arr[5] = cmd.ExecuteScalar().ToString();
            arr[6] = "";

            string[] formula_values = new string[2];
            formula_values[0] = Convert.ToDateTime(ess.begdate).ToString("dd/MM/yyyy");
            formula_values[1] = Convert.ToDateTime(ess.enddate).ToString("dd/MM/yyyy");

            string updatedquery = rc.getupdatedquery(rc.getsquery(rptid), arr);
            //System.IO.Stream stream = rc.getrptstream(rptid, updatedquery);
            System.IO.Stream stream = rc.getrptstream(rptid, updatedquery, formula_values);


            //string updatedquery = rc.getupdatedquery(rc.getsquery(rptid), arr);
            //System.IO.Stream stream = rc.getrptstream(rptid, updatedquery);
            return File(stream, "application/pdf");
        }
        [HttpPost]
        public JsonResult SaveCalendar(string[][] cdata)
        {
           


            din = new DatabaseInsertClass();

            din.insertCalender(cdata);

            return Json(true);


        }

        [HttpPost]
             public JsonResult UpdateApl(string reqno , string act)
        {

            din = new DatabaseInsertClass();

            din.updateApl(reqno, act);

            return Json(true);


        }



        [HttpGet]
        public void CvView(string userid)
        {

            string FilePath = Server.MapPath("resume-samples.pdf");
            WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(FilePath);
            if (FileBuffer != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", FileBuffer.Length.ToString());
                Response.BinaryWrite(FileBuffer);
            }


        }

        

        [HttpPost]
        public string GetAppl(string aplid)
        {
            string query;

            if (aplid == "0") {

                query = "select req.reqno , usr01.userid  as email , apl.aplid,pos.postxt, loc.charvalue as location, req.reqstat as Status, RTRIM(usr01.title)+' ' + RTRIM(usr01.fname) + ' ' + usr01.lname as Name from eposhdr as Pos left join areqmst as req on pos.pos = req.pos  inner join apl0120 as apl on apl.jobkey = req.reqno inner join usr03 on apl.aplid = usr03.paramvalue inner join usr01 on usr03.userid = usr01.userid left join charvalue as loc on loc.classobj = req.classobj left join charistic as Char on loc.charistic = char.charistic where 1=1 and char.charistic = 'LOCATION'";

            }
            else
            {

                query = "select req.reqno , usr01.userid  as email , apl.aplid,pos.postxt, loc.charvalue as location, req.reqstat as Status, RTRIM(usr01.title)+' ' + RTRIM(usr01.fname) + ' ' + usr01.lname as Name from eposhdr as Pos left join areqmst as req on pos.pos = req.pos  inner join apl0120 as apl on apl.jobkey = req.reqno inner join usr03 on apl.aplid = usr03.paramvalue inner join usr01 on usr03.userid = usr01.userid left join charvalue as loc on loc.classobj = req.classobj left join charistic as Char on loc.charistic = char.charistic where 1=1 and char.charistic = 'LOCATION' and paramid = 'APL' and usr01.userid = '" + aplid + "'";


            }

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
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
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);

                    }
                    var jsonSerialiser = new JavaScriptSerializer();
                    string jsonvalue = jsonSerialiser.Serialize(rows);

                    return jsonvalue;
                }
            }
        }


        [HttpPost]
        public JsonResult GetEmpDtl(string empid)
        {


            ArrayList arryList1 = new ArrayList();
            ArrayList arryList = new ArrayList();

            try
            {


                // string tablename;
                string query = "select esub.eesubgrptxt, epos.postxt from empmain as ep inner join emporg as eorg on ep.empid = eorg.empid inner join eposhdr as epos on eorg.pos = epos.pos inner join eesubgrp as esub on ep.eesubgrp = esub.eesubgrp where ep.empid = '" + empid + "' and eorg.delind <> 'X'";

                da.CreateConnection();
                da.OpenConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {

                    Dictionary<string, object> row;

                    while (da.obj_reader.Read())
                    {

                        arryList1.Add(da.obj_reader["eesubgrptxt"].ToString());
                        arryList.Add(da.obj_reader["postxt"].ToString());

                    }
                    da.obj_reader.Close();
                }
                else
                {
                    da.obj_reader.Close();
                    return Json(new { success = false, responseText = "not found" }, JsonRequestBehavior.AllowGet); ;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing" + ex.Message);
            }



            return Json(new { success = true, responseText = arryList1, responseID = arryList }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public JsonResult DeleteCalendar(string id)
        {
            try
            {
                // string tablename;
                string query = "delete from emp0290 where pid = '" + id + "'";
                da.CreateConnection();
                da.OpenConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.obj_sqlcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {

            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult UpdateCalendar(string[][] cdata)
        {
            din = new DatabaseInsertClass();

            string status = din.updateCalender(cdata);


            return Json(status);
        }




        [HttpGet]
        public JsonResult GetCalendar(string pid , string empid = null , string moment = null)
        {
            var user_id = System.Web.HttpContext.Current.Session["User_Id"].ToString();

            string[] arr = new string[20];

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            ArrayList arryList1 = new ArrayList();
            try

            {
                string query;


                


                if (pid == "0")
                {

                    query = "select *  from emp0290  join emppers on emp0290.empid = emppers.empid join visittype on emp0290.attype = visittype.visittype join costorder on emp0290.clientid = costorder.costorder where emp0290.creuser = '" + user_id + "'";
               
                }else if (pid == "1")
                {

                    DateTime date = DateTime.Parse(moment);
                    string month = date.ToString("MM");
                    string year = date.ToString("yyyy");


                    query = "select *  from emp0290  join emppers on emp0290.empid = emppers.empid join visittype on emp0290.attype = visittype.visittype join costorder on emp0290.clientid = costorder.costorder where emp0290.empid = '" + empid + "' and MONTH(emp0290.begdate) = '"+ month + "' and YEAR(emp0290.begdate) = '"+ year + "'";
                }
                else
                {

                    query = "select *  from emp0290  join emppers on emp0290.empid = emppers.empid join visittype on emp0290.attype = visittype.visittype join costorder on emp0290.clientid = costorder.costorder where pid = '" + pid + "'";
                }



                da.CreateConnection();
                da.OpenConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {

                    Dictionary<string, object> row;

                    while (da.obj_reader.Read())
                    {
                        row = new Dictionary<string, object>();

                        row.Add("attype", da.obj_reader["attype"].ToString());
                        row.Add("typetxt", da.obj_reader["typetxt"].ToString());
                        row.Add("starttime", da.obj_reader["starttime"].ToString());
                        row.Add("endtime", da.obj_reader["endtime"].ToString());
                        row.Add("remarks", da.obj_reader["remarks"].ToString());
                        row.Add("location", da.obj_reader["location"].ToString());
                        row.Add("clientid", da.obj_reader["clientid"].ToString());
                        row.Add("clientname", da.obj_reader["ordstxt"].ToString());
                        row.Add("purpose", da.obj_reader["purpose"].ToString());
                        row.Add("pid", da.obj_reader["pid"].ToString());
                        row.Add("vwith", da.obj_reader["vwith"].ToString());
                        row.Add("empid", da.obj_reader["empid"].ToString());
                        row.Add("EmployeeName", da.obj_reader["firstname"].ToString() + " " + da.obj_reader["midname"].ToString() + " " + da.obj_reader["lastname"].ToString());
                        rows.Add(row);
                        //arryList1.Add(da.obj_reader["empid"].ToString());
                        //arryList1.Add(da.obj_reader["empid"].ToString());

                    }
                    da.obj_reader.Close();
                }
                else
                {
                    da.obj_reader.Close();
                    //return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing" + ex.Message);
            }


            //string isoJson = JsonConvert.SerializeObject(rows, new IsoDateTimeConverter());


            //return isoJson;

            return Json(new { success = true, responseText = rows }, JsonRequestBehavior.AllowGet);





        }
        public ActionResult Applicant_List()

        {

            return View();
        
        
        }



            public ActionResult Multi_Calendar()
        
        {

            var list = new List<Report>();

            List<Report> listuser = new List<Report>();
            Report users = new Report();

            string query = "select distinct ep.empid, ep.firstname + ' ' + ep.midname + ' ' + ep.lastname as 'empname' from emppers as ep where ep.delind <> 'X' and ep.empid not in (Select empid from emp0351 as e51 where e51.delind <> 'X' and e51.payblock = 'X') order by empname ASC";
            da.CreateConnection();
            da.OpenConnection();
            da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
            da.obj_reader = da.obj_sqlcommand.ExecuteReader();
            if (da.obj_reader.HasRows)
            {
                while (da.obj_reader.Read())
                {
                    listuser.Add(new Report
                    {
                        empid = da.obj_reader["empid"].ToString().Trim(),
                        empname = da.obj_reader["empname"].ToString()
                    });
                }

            }

            da.obj_reader.Close();


            //var client = new List<Clientmodel>();

            //List<Clientmodel> listclient = new List<Clientmodel>();
            //Clientmodel clients = new Clientmodel();


            query = "select costorder, ordstxt from costorder";

            da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
            da.obj_reader = da.obj_sqlcommand.ExecuteReader();
            if (da.obj_reader.HasRows)
            {
                while (da.obj_reader.Read())
                {
                    listuser.Add(new Report
                    {
                        costorder = da.obj_reader["costorder"].ToString(),
                        cname = da.obj_reader["ordstxt"].ToString()
                    });
                }

            }

            da.obj_reader.Close();

            query = "select visittype, typetxt from visittype";

            da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
            da.obj_reader = da.obj_sqlcommand.ExecuteReader();
            if (da.obj_reader.HasRows)
            {
                while (da.obj_reader.Read())
                {
                    listuser.Add(new Report
                    {
                        visittype = da.obj_reader["visittype"].ToString(),
                        typetxt = da.obj_reader["typetxt"].ToString()
                    });
                }

            }




            return View(listuser);
        }


        public ActionResult View_Calendar()

        {

            var list = new List<Report>();

            List<Report> listuser = new List<Report>();
            Report users = new Report();

            string query = "select distinct ep.empid, ep.firstname + ' ' + ep.midname + ' ' + ep.lastname as 'empname' from emppers as ep where ep.delind <> 'X' and ep.empid not in (Select empid from emp0351 as e51 where e51.delind <> 'X' and e51.payblock = 'X') order by empname ASC";
            da.CreateConnection();
            da.OpenConnection();
            da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
            da.obj_reader = da.obj_sqlcommand.ExecuteReader();
            if (da.obj_reader.HasRows)
            {
                while (da.obj_reader.Read())
                {
                    listuser.Add(new Report
                    {
                        empid = da.obj_reader["empid"].ToString().Trim(),
                        empname = da.obj_reader["empname"].ToString()
                    });
                }

            }

            da.obj_reader.Close();




            query = "select costorder, ordstxt from costorder";

            da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
            da.obj_reader = da.obj_sqlcommand.ExecuteReader();
            if (da.obj_reader.HasRows)
            {
                while (da.obj_reader.Read())
                {
                    listuser.Add(new Report
                    {
                        costorder = da.obj_reader["costorder"].ToString(),
                        cname = da.obj_reader["ordstxt"].ToString()
                    });
                }

            }

            da.obj_reader.Close();

            query = "select visittype, typetxt from visittype";

            da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
            da.obj_reader = da.obj_sqlcommand.ExecuteReader();
            if (da.obj_reader.HasRows)
            {
                while (da.obj_reader.Read())
                {
                    listuser.Add(new Report
                    {
                        visittype = da.obj_reader["visittype"].ToString(),
                        typetxt = da.obj_reader["typetxt"].ToString()
                    });
                }

            }




            return View("Multi_Calendar", listuser);
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