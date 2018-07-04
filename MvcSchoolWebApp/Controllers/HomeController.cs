﻿using CrystalDecisions.CrystalReports.Engine;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using MvcSchoolWebApp.Data;
using System.Configuration;

namespace MvcSchoolWebApp.Controllers
{

    public class HomeController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string popup_status;
        public static List<Users> user_dtl;

        private Database.Database da = new Database.Database("Falconlocal");
        Data.data data = new data();
        private string idd = "1000000000";

        DatabaeseClass db;
        DatabaseInsertClass din;
        public Page Page { get; private set; }

        string getSquery = string.Empty;
        string UpdatedQuery = string.Empty;


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

        public JsonResult changepassword(string currpass, string newpass, string conpass)
        {
            string status = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("select passwd from usr01 where userid = '" + user_id + "' and passwd = '" + currpass + "'", con);
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.HasRows)
            {
                if (newpass == conpass)
                {
                    sdr.Close();
                    SqlCommand cmd2 = new SqlCommand("update usr01 set passwd = '" + conpass + "' , createddate = '" + DateTime.Now.ToString("yyyy/MM/dd") + "', " +
                        "createdtime = '" + DateTime.Now.ToString("h:mm:ss tt") + "'  where userid = '" + user_id + "'", con);
                    cmd2.ExecuteNonQuery();
                    status = "Success";
                }
            }
            else
            {
                status = "Error";
                //incorrect previous password 
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MarksUpload()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[32].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            db = new DatabaeseClass();
            DatabaseModel assignment = new DatabaseModel();
            assignment.campus = db.getcampus();
            assignment.campusid = assignment.campus[0].Value;

            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            assignment.classes = db.getclass(assignment.campus[0].Value, user_id);
            assignment.teachername = db.FillTeacherName(user_id);

            assignment.subject = sl;
            assignment.section = sl;
            assignment.category = db.FillCategory();
            assignment.module = db.FillSubModule();
            return View(assignment);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetGroupNames(string word, string type)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString.ToString());
            conn.Open();
            string id = "";
            string txt = "";
            string query;
            if (type == "11")
            {
                query = "select distinct groupid, grouptxt from schgrpdtl where grouptxt like '" + word + "%' order by grouptxt asc";
                id = "groupid";
                txt = "grouptxt";
            }
            else
            {
                query = "select * from vw_searchname where vw_searchname.username like '" + word + "%' and vw_searchname.userid <> '" + user_id+ "' order by username asc";
                id = "userid";
                txt = "username";
            }

            SqlCommand cmd = new SqlCommand(query, conn);
            List<DatabaseModel> lst = new List<DatabaseModel>();
            SqlDataReader sdr;
            sdr = cmd.ExecuteReader();
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    lst.Add(new DatabaseModel
                    {
                        groupid = sdr[id].ToString().Trim(),
                        grouptxt = sdr[txt].ToString().Trim()
                    });
                }
                conn.Close();
            }

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Email()
        {
            DatabaseModel email = new DatabaseModel();
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View(email);
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Email(HttpPostedFileBase postedFile, DatabaseModel dm)
        {
            MessageCls msgobj = new MessageCls();
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            string filepath = "";

            string cs = ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString.ToString();
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();
            try
            {

                string query = "select ISNULL(MAX(chatviewid),0)+1 from chatview";
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = query;
                cmd.Transaction = trans;
                string chatviewId = cmd.ExecuteScalar().ToString();
                db = new DatabaeseClass();
                DateTime currtime = db.convertservertopsttimezone(DateTime.Now.ToString());
                if (dm.sendTo != null && dm.groupid == null)
                {
                    cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                    int recordno = Convert.ToInt16(cmd.ExecuteScalar());
                    string increment = recordno.ToString() + "1";

                    if (postedFile != null)
                    {
                        if(postedFile.ContentLength <= 25000000)
                        {
                            filepath = MsgAttachment(postedFile, increment);
                        }
                    }
                    cmd.CommandText = "insert into inbox(msgid,recordno,subject,message,sender,recip,cc,unread,status,dbtimestmp,chatviewid,filepath,notfcase) values('" + recordno + "','1','" + dm.title + "',@message,'" + user_id + "','" + dm.sendTo + "',' ','X',' ','" + currtime + "','" + chatviewId + "','" + filepath + "','')";
                    cmd.Parameters.AddWithValue("@message", dm.msg);

                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "insert into chatview (chatviewid,msgid,userid,Isview) values('" + chatviewId + "',(select ISNULL(MAX(msgid),0) from inbox),'" + user_id + "',' ')";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "insert into chatview (chatviewid,msgid,userid,Isview) values('" + chatviewId + "',(select ISNULL(MAX(msgid),0) from inbox),'" + dm.sendTo + "',' ')";
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                else
                {
                    cmd.CommandText = "select count(empid) from schgrpdtl where groupid = '" + dm.groupid + "'";
                    int countid = Convert.ToInt16(cmd.ExecuteScalar());
                    if (countid > 0)
                    {
                        string[] totalid = new string[countid];
                        cmd.CommandText = "select empid from schgrpdtl where groupid = '" + dm.groupid + "'";
                        SqlDataReader sdr;
                        sdr = cmd.ExecuteReader();
                        if (sdr.HasRows)
                        {
                            int i = 0;
                            while (sdr.Read())
                            {
                                totalid[i] = Convert.ToString(sdr["empid"]).Trim();
                                i++;
                            }
                            sdr.Close();
                        }

                        cmd.CommandText = "select isnull(max(recordno),0)+01 from schgrpinbox where groupid = '" + dm.groupid + "'";
                        int recordno = Convert.ToInt16(cmd.ExecuteScalar());
                        string increment = dm.groupid + "1" + recordno.ToString();

                        if (postedFile != null)
                        {
                            if(postedFile.ContentLength <= 25000000)
                                filepath = MsgAttachment(postedFile, increment);
                        }
                        for (int i = 0; i < totalid.Length; i++)
                        {
                            cmd.CommandText = "insert into schgrpinbox(groupid, msgid, recordno, sender, recip, status, unread, dbtimestmp) values('" + dm.groupid + "', '1', '" + recordno + "', '" + user_id + "', '" + totalid[i] + "', ' ', 'X', '" + currtime + "')";
                            cmd.ExecuteNonQuery();
                        }

                        cmd.CommandText = "insert into schgrpmsg(groupid, msgid, recordno, subject, msgtxt,filepath) values ('" + dm.groupid + "', '1', '" + recordno + "', '" + dm.title + "', '" + dm.msg + "','" + filepath + "')";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "update schgrpinbox set unread = '' where groupid = '" + dm.groupid + "' and recordno = '" + recordno + "' and recip = '" + user_id + "'";
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                }
                ViewBag.Message = "Message Successfully Sent";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                ViewBag.call_alert = "show_alert()";
                ViewBag.message_popup = "Found Some Error! Please Try Again";
                ViewBag.cssclass = "alert-danger";
            }
            finally
            {
                conn.Close();
                trans.Dispose();
            }
            dm = new DatabaseModel();
            ModelState.Clear();
            return View();
        }

        public string MsgAttachment(HttpPostedFileBase postedFile, string increment)
        {
            
            string path = Server.MapPath("~/Uploads/MessageAttachments/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filename = Path.GetFileName(increment + " - " + postedFile.FileName);
            postedFile.SaveAs(path + filename);
            return path + filename;
        }
        public ActionResult Assignment()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[20].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel assignment = new DatabaseModel();
            assignment.campus = db.getcampus();
            assignment.classes = db.getclass(assignment.campus[0].Value, user_id);
            assignment.campusid = assignment.campus[0].Value;
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            assignment.subject = sl;
            assignment.classesid = "";
            assignment.section = sl;
            assignment.category = db.FillCategory();
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            return View(assignment);
        }

        [HttpPost]
        public ActionResult Assignment(HttpPostedFileBase postedFile, DatabaseModel assignment)
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[20].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            din = new DatabaseInsertClass();
            assignment.teacherid = user_id;
            if (assignment.campusid != null && assignment.sectionid != null && assignment.classesid != null
                && assignment.categoryid != null && assignment.begdate != null && assignment.subjectid != null)
            {
                if (postedFile != null)
                {
                    if(postedFile.ContentLength <= 25000000)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx", ".xls" };
                        var checkextension = Path.GetExtension(postedFile.FileName).ToLower();
                        string path = Server.MapPath("~/Uploads/Lessons/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string query3 = "select isnull(max(lessonid)+01, 01) from lessonplan";
                        da.CreateConnection();
                        da.InitializeSQLCommandObject(da.GetCurrentConnection, query3);
                        da.OpenConnection();
                        assignment.increment = da.obj_sqlcommand.ExecuteScalar().ToString();

                        assignment.filename = Path.GetFileName(assignment.increment + " - " + postedFile.FileName);
                        assignment.imagepath = path + assignment.filename;
                        postedFile.SaveAs(assignment.imagepath);
                        din.InsertAssigment(assignment);
                        da.CloseConnection();
                        if (popup_status == "Success")
                        {
                            ViewBag.Message = "File Uploaded Successfully.";
                            ViewBag.call_alert = "show_alert()";
                            ViewBag.message_popup = "File Uploaded Successfully";
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
                ViewBag.call_alert = "show_alert()";
                ViewBag.message_popup = "All Fields must be selected";
                ViewBag.cssclass = "alert-danger";
            }
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            db = new DatabaeseClass();
            assignment.campus = db.getcampus();
            assignment.campusid = assignment.campusid;

            assignment.classes = db.getclass(assignment.campusid, user_id);
            assignment.classesid = assignment.classesid;

            assignment.section = db.getsection(assignment.campusid, assignment.classesid, user_id);
            assignment.sectionid = assignment.sectionid;

            assignment.category = db.FillCategory();
            assignment.categoryid = assignment.categoryid;

            assignment.subject = db.getsubject(assignment.campusid, assignment.classesid, assignment.sectionid, user_id);
            assignment.subjectid = assignment.subjectid;

            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");

            return View("Assignment", assignment);
        }


        public ActionResult Profiles(Profile pro)
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            #region profile
            if (user_role == "1000" || user_role == "2000" || user_role == "3000")
            {

                da.CreateConnection();
                string query =
                    "select distinct emp.empid, emp.earea , e.eareatxt , eesubarea , esub.esubareat , emp.eegroup, " +
                    "eg.eegrouptxt , emp.begdate ,camp.campustxt, emp.enddate, info.formadd, info.firstname, info.midname, info.lastname, " +
                    "info.secname, info.idnumber, info.birthdate, info.gender, info.birthplace, info.cobirth, info.birthname, info.nationality, img.imagepath, ad.typetxt, " +
                    "empadd.careof, empadd.street1, empadd.street2, empadd.zipcode, empadd.city, empadd.district, empadd.phone from empmain as emp " +
                    "inner join earea as e on emp.earea = e.earea " +
                    "inner join esubarea as esub on emp.eesubarea = esub.esubarea and emp.earea = esub.earea " +
                    "inner join eegroup as eg on emp.eegroup = eg.eegroup inner join emppers as info on emp.empid = info.empid " +
                    "inner join emp0710 as e18 on emp.empid = e18.empid inner join schcampus as camp on e18.campusid = camp.campusid " +
                    "left outer join emp0170 as e17 on emp.empid = e17.empid left outer join imageobj as img on e17.imageid = img.imageid " +
                    "left join empadd on emp.empid = empadd.empid " +
                    "left join addresstype as ad on empadd.subpagtype = ad.addtype " +
                    "where emp.empid = '" + user_id + "' and emp.delind <> 'X' and e18.delind <> 'X' and  info.delind <> 'X' and empadd.delind <> 'X'";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        DateTime date = new DateTime();
                        pro.school = "Schooling System";
                        pro.campus = da.obj_reader["campustxt"].ToString() ?? "-";
                        pro.roleid = Convert.ToInt32(da.obj_reader["empid"]);
                        pro.status = da.obj_reader["eegrouptxt"].ToString() ?? "-";
                        pro.department = da.obj_reader["esubareat"].ToString() ?? "-";
                        pro.birthcountry = da.obj_reader["cobirth"].ToString() ?? "-";
                        pro.careof = da.obj_reader["careof"].ToString() ?? "-";

                        date = Convert.ToDateTime(da.obj_reader["birthdate"]);
                        pro.dob = date.ToString("dd/MMMM/yyyy") ?? "-";

                        pro.city = da.obj_reader["city"].ToString() ?? "-";
                        pro.district = da.obj_reader["district"].ToString() ?? "-";
                        pro.zipcode = da.obj_reader["zipcode"].ToString() ?? "-";
                        pro.paddress = da.obj_reader["street1"].ToString() ?? "-";
                        pro.paddress2 = da.obj_reader["street2"].ToString() ?? "-";
                        pro.firstname = da.obj_reader["firstname"].ToString() ?? "-";
                        pro.fathername = da.obj_reader["birthname"].ToString() ?? "-";
                        pro.lastname = da.obj_reader["lastname"].ToString() ?? "-";
                        pro.secondname = da.obj_reader["secname"].ToString() ?? "-";
                        pro.middlename = da.obj_reader["midname"].ToString() ?? "-";
                        pro.birthplace = da.obj_reader["birthplace"].ToString() ?? "-";
                        date = Convert.ToDateTime(da.obj_reader["begdate"]);
                        pro.fromdate = date.ToString("dd/MMMM/yyyy");
                        date = Convert.ToDateTime(da.obj_reader["enddate"]);
                        pro.todate = date.ToString("dd/MMMM/yyyy") ?? "-";
                        pro.title = da.obj_reader["formadd"].ToString() ?? "-";
                        pro.gender = da.obj_reader["gender"].ToString() ?? "-";
                        pro.phone = da.obj_reader["phone"].ToString() ?? "-";
                        pro.roletxt = da.obj_reader["eareatxt"].ToString() ?? "-";
                        if (da.obj_reader["imagepath"].ToString().Trim() != "")
                            pro.image = da.obj_reader["imagepath"].ToString();
                        else
                            pro.image = "~/Content/Avatar/avatar2.png";
                        pro.addtype = da.obj_reader["typetxt"].ToString() ?? "-";
                        pro.nic = da.obj_reader["idnumber"].ToString() ?? "-";
                        pro.nationality = da.obj_reader["nationality"].ToString() ?? "-";
                    }
                }
            }
            else if (user_role == "5000")
            {

                da.CreateConnection();
                string query =
                    "select distinct info.title, std.stdid, std.stdarea , s.stdareatxt , std.stdsubarea , ssub.stdsubareat , std.stdgroup,  " +
                    "sg.stdgrouptxt , std.begdate ,camp.campustxt, std.enddate, info.firstname, info.midname, info.lastname,  " +
                    "info.birthdate, info.gender, info.birthplace,  info.nationality, img.imagepath, " +
                    "info.street1, info.street2, info.zipcode, info.city, info.district, info.homeph from stdmain as std " +
                    "inner join stdarea as s on std.stdarea = s.stdarea " +
                    "inner join stdsubarea as ssub on std.stdsubarea = ssub.stdsubarea and std.stdarea = ssub.stdarea " +
                    "inner join stdgroup as sg on std.stdgroup = sg.stdgroup inner join stdpers as info on std.stdid = info.stdid " +
                    "inner join std0710 as s18 on std.stdid = s18.stdid inner join schcampus as camp on s18.campusid = camp.campusid " +
                    "left outer join emp0170 as e17 on std.stdid = e17.empid left outer join imageobj as img on e17.imageid = img.imageid " +
                    "where std.stdid = '" + user_id + "' and std.delind <> 'X' and s18.delind <> 'X' and info.delind <> 'X' ";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        DateTime date = new DateTime();
                        pro.school = "Schooling System";
                        pro.campus = da.obj_reader["campustxt"].ToString() ?? "-";
                        pro.roleid = Convert.ToInt32(da.obj_reader["stdid"]);
                        pro.status = da.obj_reader["stdgrouptxt"].ToString() ?? "-";
                        pro.department = da.obj_reader["stdsubareat"].ToString() ?? "-";
                        pro.birthcountry = da.obj_reader["nationality"].ToString() ?? "-";
                        pro.careof = "-";

                        date = Convert.ToDateTime(da.obj_reader["birthdate"]);
                        pro.dob = date.ToString("dd/MMMM/yyyy") ?? "-";

                        pro.city = da.obj_reader["city"].ToString() ?? "-";
                        pro.district = da.obj_reader["district"].ToString() ?? "-";
                        pro.zipcode = da.obj_reader["zipcode"].ToString() ?? "-";
                        pro.paddress = da.obj_reader["street1"].ToString() ?? "-";
                        pro.paddress2 = da.obj_reader["street2"].ToString() ?? "-";
                        pro.firstname = da.obj_reader["firstname"].ToString() ?? "-";
                        pro.fathername = "-";
                        pro.lastname = da.obj_reader["lastname"].ToString() ?? "-";
                        pro.secondname = "-";
                        pro.middlename = da.obj_reader["midname"].ToString() ?? "-";
                        pro.birthplace = da.obj_reader["birthplace"].ToString() ?? "-";
                        date = Convert.ToDateTime(da.obj_reader["begdate"]);
                        pro.fromdate = date.ToString("dd/MMMM/yyyy");
                        date = Convert.ToDateTime(da.obj_reader["enddate"]);
                        pro.todate = date.ToString("dd/MMMM/yyyy") ?? "-";
                        pro.title = da.obj_reader["title"].ToString() ?? "-";
                        pro.gender = da.obj_reader["gender"].ToString() ?? "-";
                        pro.phone = da.obj_reader["homeph"].ToString() ?? "-";
                        pro.roletxt = da.obj_reader["stdareatxt"].ToString() ?? "-";
                        if (da.obj_reader["imagepath"].ToString().Trim() != "")
                            pro.image = da.obj_reader["imagepath"].ToString();
                        else
                            pro.image = "~/Content/Avatar/avatar2.png";
                        pro.addtype = "Mailing Address";
                        pro.nic = "-";
                        pro.nationality = da.obj_reader["nationality"].ToString() ?? "-";
                    }
                }
            }
            else if (user_role == "4000")
            {

                da.CreateConnection();
                string query =
                    "select distinct info.title, std.stdid, std.stdarea , s.stdareatxt , std.stdsubarea , ssub.stdsubareat , std.stdgroup,  " +
                    "sg.stdgrouptxt , std.begdate ,camp.campustxt, std.enddate, info.firstname, info.midname, info.lastname,  " +
                    "info.birthdate, info.gender, info.birthplace,  info.nationality, img.imagepath, " +
                    "info.street1, info.street2, info.zipcode, info.city, info.district, info.homeph from stdmain as std " +
                    "inner join stdarea as s on std.stdarea = s.stdarea " +
                    "inner join stdsubarea as ssub on std.stdsubarea = ssub.stdsubarea and std.stdarea = ssub.stdarea " +
                    "inner join stdgroup as sg on std.stdgroup = sg.stdgroup inner join stdpers as info on std.stdid = info.stdid " +
                    "inner join std0710 as s18 on std.stdid = s18.stdid inner join schcampus as camp on s18.campusid = camp.campusid " +
                    "left outer join emp0170 as e17 on std.stdid = e17.empid left outer join imageobj as img on e17.imageid = img.imageid " +
                    "where std.stdid = '" + user_id + "' and std.delind <> 'X' and s18.delind <> 'X' and info.delind <> 'X' ";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        DateTime date = new DateTime();
                        pro.school = "Schooling System";
                        pro.campus = da.obj_reader["campustxt"].ToString() ?? "-";
                        pro.roleid = Convert.ToInt32(da.obj_reader["stdid"]);
                        pro.status = da.obj_reader["stdgrouptxt"].ToString() ?? "-";
                        pro.department = da.obj_reader["stdsubareat"].ToString() ?? "-";
                        pro.birthcountry = da.obj_reader["nationality"].ToString() ?? "-";
                        pro.careof = "-";

                        date = Convert.ToDateTime(da.obj_reader["birthdate"]);
                        pro.dob = date.ToString("dd/MMMM/yyyy") ?? "-";

                        pro.city = da.obj_reader["city"].ToString() ?? "-";
                        pro.district = da.obj_reader["district"].ToString() ?? "-";
                        pro.zipcode = da.obj_reader["zipcode"].ToString() ?? "-";
                        pro.paddress = da.obj_reader["street1"].ToString() ?? "-";
                        pro.paddress2 = da.obj_reader["street2"].ToString() ?? "-";
                        pro.firstname = da.obj_reader["firstname"].ToString() ?? "-";
                        pro.fathername = "-";
                        pro.lastname = da.obj_reader["lastname"].ToString() ?? "-";
                        pro.secondname = "-";
                        pro.middlename = da.obj_reader["midname"].ToString() ?? "-";
                        pro.birthplace = da.obj_reader["birthplace"].ToString() ?? "-";
                        date = Convert.ToDateTime(da.obj_reader["begdate"]);
                        pro.fromdate = date.ToString("dd/MMMM/yyyy");
                        date = Convert.ToDateTime(da.obj_reader["enddate"]);
                        pro.todate = date.ToString("dd/MMMM/yyyy") ?? "-";
                        pro.title = da.obj_reader["title"].ToString() ?? "-";
                        pro.gender = da.obj_reader["gender"].ToString() ?? "-";
                        pro.phone = da.obj_reader["homeph"].ToString() ?? "-";
                        pro.roletxt = da.obj_reader["stdareatxt"].ToString() ?? "-";
                        if (da.obj_reader["imagepath"].ToString().Trim() != "")
                            pro.image = da.obj_reader["imagepath"].ToString();
                        else
                            pro.image = "~/Content/Avatar/avatar2.png";
                        pro.addtype = "Mailing Address";
                        pro.nic = "-";
                        pro.nationality = da.obj_reader["nationality"].ToString() ?? "-";
                    }
                }
            }
            else
            {
                return null;
            }
            #endregion
            return View(pro);
        }


        public ActionResult Fee()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }


        public ActionResult Teacher_Evalution()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[26].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel attendance = new DatabaseModel();
            attendance.campus = db.getcampus();

            attendance.student = db.FillEmployee(attendance.campus[0].Value);

            attendance.campusid = attendance.campus[0].Value;
            attendance.studentid = "";
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("MMMM yyyy");
            return View(attendance);
        }

        public ActionResult Finance()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[37].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }

        public ActionResult Purchase()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[37].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }
        public ActionResult Sales()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[37].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }

        public ActionResult Human_Resource()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[37].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }

        public ActionResult Admission()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }

        public ActionResult ActivityGrade()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[32].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel activity = new DatabaseModel();
            activity.campus = db.getcampus();
            activity.campusid = activity.campus[0].Value;
            activity.classes = db.getclass(activity.campus[0].Value, user_id);
            activity.section = db.getsection(activity.campus[0].Value, activity.classes[0].Value, user_id);
            return View(activity);
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

        public JsonResult getSubjectJson(string campusId, string classId, string sectionId, string teacherId, string selectsubjectId = null)
        {
            Object ob;
            db = new DatabaeseClass();
            ob = db.getsubject(campusId, classId, sectionId, user_id);
            return Json(ob);
        }
        public JsonResult getMarksJQGridJson(string campus, string classes, string section, string subject, string rsltype)
        {
            db = new DatabaeseClass();
            return Json(db.FillMarksUploadGrid(campus, classes, section, subject, rsltype), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MarksSubmit(string[] studentId, string[] exammarks, string[] projectmarks, string[] testmarks,
            string[] oralmarks, string[] assignmarks, string[] p1marks, string[] p2marks, string[] p3marks, string campusId, string classId, string moduletxt,
            string sectionId, string subjectId, string moduleId, string dateId, string marks, string marksp2, string marksp3)

        {
            if (marksp2 == "" || marksp2 == null)
                marksp2 = "0";
            if (marksp3 == "" || marksp3 == null)
                marksp3 = "0";
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel dm = new DatabaseModel();
            dm.campusid = campusId; dm.classesid = classId; dm.sectionid = sectionId; dm.subjectid = subjectId; dm.marks = marks; dm.marksp2 = marksp2; dm.marksp3 = marksp3;
            dm.date = dateId; dm.moduleid = moduleId; dm.exammarks = exammarks[0]; dm.projectmarks = projectmarks[0]; dm.oralmarks = oralmarks[0];
            dm.teacherid = user_id; dm.studentid = studentId[0]; dm.testmarks = testmarks[0]; dm.assignmarks = assignmarks[0]; dm.moduletxt = moduletxt;

            DatabaseInsertClass din = new DatabaseInsertClass();

            din.InsertMarks(studentId, exammarks, projectmarks, testmarks, oralmarks, assignmarks, p1marks, p2marks, p3marks, dm);
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            dm.campus = db.getcampus();
            dm.campusid = "";
            dm.classes = sl;
            dm.teachername = db.FillTeacherName(user_id);
            dm.subject = sl;
            dm.section = sl;
            dm.category = db.FillCategory();
            dm.module = db.FillSubModule();

            return Json(HomeController.popup_status, JsonRequestBehavior.AllowGet);

        }

        public JsonResult getActivityGrid(string campus, string classes, string section)
        {
            db = new DatabaeseClass();
            return Json(db.FillActivityGrid(campus, classes, section), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getEmployeeAttendanceGrid(string campus, string employeeid, DateTime dateid)
        {
            db = new DatabaeseClass();
            if (employeeid == null || employeeid == "")
                employeeid = user_id;

            return Json(db.FillEmployeeAttendance(campus, employeeid, dateid), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getEmployeeJson(string campusid)
        {
            db = new DatabaeseClass();
            return Json(db.FillEmployee(campusid));
        }

        public ActionResult GradeActivitySubmit(string[] studentId, string[] status, string[] assignppt, string[] gk,
            string[] behave, string[] discp, string[] clean, string[] compliance, string[] task, string[] teachercom, string campusId, string classId, string sectionId)
        {
            din = new DatabaseInsertClass();
            din.InsertGradeActivity(studentId, status, assignppt, gk, behave, discp, clean, compliance, task, teachercom, campusId, classId, sectionId, user_id);
            return Json(HomeController.popup_status, JsonRequestBehavior.AllowGet);

        }
        public ActionResult timesheet()
        {
            return View();
        }
    }
}

