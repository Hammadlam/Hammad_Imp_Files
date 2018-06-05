using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace MvcSchoolWebApp.Controllers
{
    public class DownloadController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static List<Users> user_dtl;
        // GET: Download
        DatabaeseClass db;
        DatabaseInsertClass din;

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

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            db = new DatabaeseClass();
            DatabaseModel assignment = new DatabaseModel();

            assignment.campus = db.getcampus();

            assignment.classes = db.getclass(assignment.campus[0].Value, user_id);
            assignment.section = sl;
            assignment.subject = sl;

            assignment.campusid = assignment.campus.ElementAt(0).Value;

            if (user_role == "5000" || user_role == "4000")
            {
                assignment.section = db.getsection(assignment.campus[0].Value, assignment.classes[0].Value, user_id);
                assignment.subject = db.getsubject(assignment.campus[0].Value, assignment.classes[0].Value, assignment.section[0].Value, user_id);

                assignment.classesid = assignment.classes[0].Value;
                assignment.sectionid = assignment.section[0].Value;
                assignment.subjectid = assignment.subject[0].Value;
            }  
            return View(assignment);
        }

        public JsonResult fundownload(string datatosave)
        {
            string getvalue = datatosave;
            string url = "";
            try
            {
                Session["download"] = getvalue;
                url = "/download/index";
            }
            catch (Exception ex)
            {
                Session["download"] = "Home Assignment";
            }

            return Json(url, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WeeklyPlan()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[24].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            DatabaseModel dm = new DatabaseModel();
            DatabaeseClass db = new DatabaeseClass();
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            List<SelectListItem> s2 = new List<SelectListItem>();
            string[] obj_time_breaks = new string[] { "3 Minutes", "4 Minutes", "5 Minutes" };
            for (int i = 0; i < obj_time_breaks.Length; i++)
            {
                s2.Add(new SelectListItem
                {
                    Text = obj_time_breaks[i],
                    Value = (i + 3).ToString()
                });
            }

            List<SelectListItem> s3 = new List<SelectListItem>();
            string[] rd_time_breaks = new string[] { "10 Minutes", "11 Minutes", "12 Minutes", "13 Minutes", "14 Minutes", "15 Minutes" };
            for (int i = 0; i < rd_time_breaks.Length; i++)
            {
                s3.Add(new SelectListItem
                {
                    Text = rd_time_breaks[i],
                    Value = (i + 10).ToString()
                });
            }

            List<SelectListItem> s4 = new List<SelectListItem>();
            string[] ww_time_breaks = new string[] { "15 Minutes", "16 Minutes", "17 Minutes", "18 Minutes", "19 Minutes", "20 Minutes" };
            for (int i = 0; i < ww_time_breaks.Length; i++)
            {
                s4.Add(new SelectListItem
                {
                    Text = ww_time_breaks[i],
                    Value = (i + 15).ToString()
                });
            }

            List<SelectListItem> weeklist = new List<SelectListItem>();
            string[] weeks = new string[] { "First", "Second", "Third", "Fourth" };
            for (int i = 0; i < weeks.Length; i++)
            {
                weeklist.Add(new SelectListItem
                {
                    Text = weeks[i],
                    Value = (i + 1).ToString()
                });
            }

            dm.campus = db.getcampus();
            dm.classes = db.getclass(dm.campus[0].Value, user_id);
            dm.section = sl;
            dm.subject = sl;
            dm.tm_time_break = s2;
            dm.rd_time_break = s3;
            dm.ww_time_break = s4;
            dm.wu_time_break = s2;
            dm.weeklist = sl;

            dm.campusid = dm.campus[0].Value;
            dm.classesid = "";
            dm.obj_time_break_id = "";
            dm.eval_time_break_id = "";
            dm.tm_time_break_id = "";
            dm.rd_time_break_id = "";
            dm.ww_time_break_id = "";
            dm.wu_time_break_id = "";
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            if (user_role == "3000")
            {
                dm.student = db.getemployeename(user_id);
            }
            else
            {
                dm.student = sl;
            }

            return View(dm);
        }

        [HttpPost]
        public ActionResult WeeklyPlan(string Methodology, string rd, DatabaseModel dm)
        {
            SqlDataReader sdr;
            DatabaseInsertClass dc = new DatabaseInsertClass();
            string cases = "DLP";
            string id = dm.teachername;
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            string[] time_breaks = new string[] { dm.tm_time_break_id, dm.rd_time_break_id, dm.ww_time_break_id, dm.wu_time_break_id };
            TimeSpan[] db_time = new TimeSpan[time_breaks.Length];
            string current_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt");
            for (int i = 0; i < time_breaks.Length; i++)
            {
                string time = "";
                if (time_breaks[i].Length == 1)
                {
                    time = "00" + ":0" + time_breaks[i] + ":" + "00";
                }
                else
                {
                    time = "00" + ":" + time_breaks[i] + ":" + "00";
                }

                DateTime time2 = DateTime.ParseExact(time, "HH:mm:ss", CultureInfo.InvariantCulture);
                db_time[i] = time2.TimeOfDay;
            }

            string recordno_query = "select isnull(max(recordno),0) from schweeklyplan where " +
                                "begdate = '" + dm.begdate + "' and enddate = '" + dm.begdate + "'  and campusid = '" + dm.campusid + "' and classid = '" + dm.classesid + "' " +
                                "and sectionid = '" + dm.sectionid + "' and subjectid = '" + dm.subjectid + "' and delind <> 'X' ";
            int recordno = 0;

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(recordno_query, con);
                recordno = Convert.ToInt16(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }

            SqlTransaction trans = null;
            SqlCommand cmd2 = con.CreateCommand();
            string[] query = new string[2];

            try
            {
                con.Open();
                trans = con.BeginTransaction();
                cmd2.Connection = con;
                cmd2.Transaction = trans;

                if (user_role == "3000")
                {
                    if (recordno >= 1)
                    {
                        cmd2.CommandText =  "update schweeklyplan set delind = 'X', upduser = '" + user_id + "', upddate = '" + current_date + "', " +
                                            "updtime = '" + current_date + "' where " +
                                            "begdate = '" + dm.begdate + "' and enddate = '" + dm.begdate + "' and campusid = '" + dm.campusid + "' and classid = '" + dm.classesid + "' " +
                                            "and sectionid = '" + dm.sectionid + "' and subjectid = '" + dm.subjectid + "' and recordno = '" + recordno + "'";
                        cmd2.ExecuteNonQuery();
                       // cases = "DLPR";
                    }


                    cmd2.CommandText = "insert into schweeklyplan(empid,campusid,classid,sectionid,subjectid,begdate,enddate,topic," +
                               " objective,resource,evaluation,teach_method,tb_teach_method," +
                               " read_disc,tb_read_disc,writtenwork,tb_writtenwork,wrapup,tb_wrapup,evaluationstd," +
                               " evaluationteach,recordno,delind,upduser,upddate,updtime) values ('" + user_id + "', '" + dm.campusid + "'," +
                               " '" + dm.classesid + "', '" + dm.sectionid + "', '" + dm.subjectid + "', '" + dm.begdate + "', '" + dm.begdate + "', @topic, @objective," +
                               " @resource, @evaluation, '" + Methodology + "', '" + db_time[0] + "', " +
                               " '" + rd + "', '" + db_time[1] + "', @writtenwork, '" + db_time[2] + "', @wrapup, " +
                               " '" + db_time[3] + "',@evalstd, @evalteach, '" + (recordno + 1) + "', ' ', ' ', ' ', ' ')";
                    cmd2.Parameters.AddWithValue("@topic", dm.topic);
                    cmd2.Parameters.AddWithValue("@objective", dm.objective);
                    cmd2.Parameters.AddWithValue("@resource", dm.resource);
                    cmd2.Parameters.AddWithValue("@evaluation", dm.evaluation);
                    cmd2.Parameters.AddWithValue("@writtenwork", dm.writtenwork);
                    cmd2.Parameters.AddWithValue("@wrapup", dm.wrapup);
                    cmd2.Parameters.AddWithValue("@evalstd", dm.evaluationstdid == null ? "" : dm.evaluationstdid);
                    cmd2.Parameters.AddWithValue("@evalteach", dm.evaluationteacherid == null ? "" : dm.evaluationteacherid);
                    cmd2.ExecuteNonQuery();


                    cmd2.CommandText = "select (select count(empid) from schgrpdtl where groupid = '100" + dm.campusid + "' and delind <> 'X') as countemp, empid from schgrpdtl " +
                                    "where groupid = '100" + dm.campusid + "' and delind <> 'X'";
                    int count = 0;
                    string[] recip = new string[count];
                    
                    sdr = cmd2.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        int i = 0;
                        while (sdr.Read())
                        {
                            if (i == 0)
                            {
                                count = Convert.ToInt16(sdr["countemp"].ToString());
                                recip = new string[count];
                            }

                            recip[i] = sdr["empid"].ToString().Trim();
                            i++;
                        }
                        sdr.Close();

                        cmd2.CommandText = "select (select notftitle from znotificationcase where notfcase = '"+cases+"') as title, sc.classtxt, sc.sectionid, ss.subjecttxt from emp0710 e7 "+
                                            "inner join schcampus sch on sch.campusid = e7.campusid "+
                                            "inner join schclass sc on e7.campusid = sc.campusid and sc.classid = e7.classid and e7.sectionid = sc.sectionid "+
                                            "inner join schsubject ss on ss.subjectid = e7.subjectid and e7.classid = ss.classid "+
                                            "where e7.empid = '"+user_id+"'  and e7.delind <> 'X' and sch.campusid = '"+dm.campusid+"' and "+
                                            "sc.classid = '"+dm.classesid+"' and sc.sectionid = '"+dm.sectionid+"' and ss.subjectid = '"+dm.subjectid+"'";
                        sdr = cmd2.ExecuteReader();
                        string text = "";
                        while (sdr.Read())
                        {
                            text = sdr["title"].ToString();
                            text += "<br/> (";
                            text += sdr["classtxt"].ToString() + ", ";
                            text += sdr["sectionid"].ToString() + ", ";
                            text += sdr["subjecttxt"].ToString() + ", ";
                            text += dm.begdate + ")";
                        }
                        sdr.Close();

                        string status = dc.popnotification(recip, user_id, text, cases);
                        if (status != "1")
                        {
                            throw new Exception();
                        }
                    }

                }
                else if (user_role == "1000" || user_role == "2000")
                {
                    cmd2.CommandText = "select isnull(max(recordno),0) from schlessonplancom  " +
                                       "where begdate = '" + dm.begdate + "' and enddate = '" + dm.begdate + "' and campusid = '" + dm.campusid + "' and classid = '" + dm.classesid + "' " +
                                       "and sectionid = '" + dm.sectionid + "' and subjectid = '" + dm.subjectid + "'";
                    // and teacherid = '" + dm.teachername + "'
                    int record = Convert.ToInt16(cmd2.ExecuteScalar());
                    record += 1;
                    cmd2.CommandText = "insert into schlessonplancom (empid,teacherid,begdate,enddate,campusid,classid,sectionid,subjectid,princplcomnt,recordno) " +
                                        "values ('" + user_id + "','" + dm.studentid + "', '" + dm.begdate + "', '" + dm.begdate + "', '" + dm.campusid + "', '" + dm.classesid + "', '" + dm.sectionid + "', " +
                                        "'" + dm.subjectid + "', @comment, '" + record + "')";
                    cmd2.Parameters.AddWithValue("@comment", dm.princ_comments_new);
                    cmd2.ExecuteNonQuery();
                    cases = "DLPR";

                    string[] arr = new string[1];
                    arr[0] = dm.studentid;

                    cmd2.CommandText = "select (select notftitle from znotificationcase where notfcase = '" + cases + "') as title, sc.classtxt, sc.sectionid, ss.subjecttxt from emp0710 e7 " +
                                            "inner join schcampus sch on sch.campusid = e7.campusid " +
                                            "inner join schclass sc on e7.campusid = sc.campusid and sc.classid = e7.classid and e7.sectionid = sc.sectionid " +
                                            "inner join schsubject ss on ss.subjectid = e7.subjectid and e7.classid = ss.classid " +
                                            "where e7.empid = '" + dm.studentid + "'  and e7.delind <> 'X' and sch.campusid = '" + dm.campusid + "' and " +
                                            "sc.classid = '" + dm.classesid + "' and sc.sectionid = '" + dm.sectionid + "' and ss.subjectid = '" + dm.subjectid + "'";
                    sdr = cmd2.ExecuteReader();
                    string text = "";
                    while (sdr.Read())
                    {
                        text = sdr["title"].ToString();
                        text += "<br/> (";
                        text += sdr["classtxt"].ToString() + ", ";
                        text += sdr["sectionid"].ToString() + ", ";
                        text += sdr["subjecttxt"].ToString() + ", ";
                        text += dm.begdate + ")";
                    }
                    sdr.Close();

                    string status = dc.popnotification(arr, user_id, text, cases);
                    if (status != "1")
                    {
                        throw new Exception();
                    }


                    //string status = dc.lessonnotification(arr, user_id, "(select notftitle from znotificationcase where notfcase = '" + cases + "')'("+dm.classes[0].Text+", "+dm.section[0].Text+", "+dm.subject[0].Text+", "+dm.date+")'", cases);
                    //if (status != "1")
                    //{
                    //    throw new Exception();
                    //}
                }

                
                
                
                trans.Commit();
                ViewBag.call_alert = "show_alert()";
                ViewBag.message_popup = "Success: Successfully Uploaded Lesson Plan";
                ViewBag.cssclass = "alert-success";

            }
            catch (Exception ex)
            {
                trans.Rollback();
                ViewBag.call_alert = "show_alert()";
                ViewBag.message_popup = "Error: An Error Occured While Submitting";
                ViewBag.cssclass = "alert-danger";
            }
            finally
            {
                con.Close();
            }

            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            List<SelectListItem> s2 = new List<SelectListItem>();
            string[] obj_time_breaks = new string[] { "3 Minutes", "4 Minutes", "5 Minutes" };
            for (int i = 0; i < obj_time_breaks.Length; i++)
            {
                s2.Add(new SelectListItem
                {
                    Text = obj_time_breaks[i],
                    Value = (i + 3).ToString()
                });
            }
            List<SelectListItem> s3 = new List<SelectListItem>();
            string[] rd_time_breaks = new string[] { "10 Minutes", "11 Minutes", "12 Minutes", "13 Minutes", "14 Minutes", "15 Minutes" };
            for (int i = 0; i < rd_time_breaks.Length; i++)
            {
                s3.Add(new SelectListItem
                {
                    Text = rd_time_breaks[i],
                    Value = (i + 10).ToString()
                });
            }
            List<SelectListItem> s4 = new List<SelectListItem>();
            string[] ww_time_breaks = new string[] { "15 Minutes", "16 Minutes", "17 Minutes", "18 Minutes", "19 Minutes", "20 Minutes" };
            for (int i = 0; i < ww_time_breaks.Length; i++)
            {
                s4.Add(new SelectListItem
                {
                    Text = ww_time_breaks[i],
                    Value = (i + 15).ToString()
                });
            }

            db = new DatabaeseClass();
            ModelState.Clear();
            DatabaseModel datamodel = new DatabaseModel();
            datamodel.campus = db.getcampus();
            datamodel.campusid = datamodel.campusid;

            datamodel.classes = db.getclass(datamodel.campus[0].Value, user_id);
            datamodel.classesid = datamodel.classesid;

            datamodel.section = db.getsection(datamodel.campus[0].Value, datamodel.classes[0].Value, user_id);
            datamodel.sectionid = datamodel.sectionid;

            datamodel.subject = db.getsubject(datamodel.campus[0].Value, datamodel.classes[0].Value, datamodel.section[0].Value, user_id); ;
            datamodel.subjectid = datamodel.subjectid;



            datamodel.weeklist = db.FillWeeks();
            datamodel.week = datamodel.week;

            datamodel.tm_time_break = datamodel.wu_time_break = s2;
            datamodel.rd_time_break = s3;
            datamodel.ww_time_break = s4;

            datamodel.obj_time_break_id = datamodel.eval_time_break_id = datamodel.tm_time_break_id = datamodel.rd_time_break_id = datamodel.ww_time_break_id =
            datamodel.wu_time_break_id = datamodel.teachername = datamodel.week = datamodel.topic = datamodel.objective = datamodel.resource = datamodel.evaluation =
            datamodel.writtenwork = datamodel.wrapup = datamodel.evaluationstdid = datamodel.evaluationteacherid = datamodel.princ_comments = "";

            if (user_role == "3000")
            {
                datamodel.student = db.getemployeename(user_id);
            }
            else
            {
                datamodel.student = sl;
            }

            return View("WeeklyPlan", datamodel);
        }

        public ActionResult DownloadLessons(string reportid)
        {
            string file = Server.MapPath("~/Uploads/Lessons/" + reportid);
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

        public ActionResult Lectures()
        {
            return View();
        }

        public ActionResult Projects()
        {
            return View();
        }



        [HttpPost]
        public JsonResult getClassJson(string campusId, string selectCityId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getclass(campusId, user_id));
        }

        public JsonResult getWeeks()
        {
            db = new DatabaeseClass();
            return Json(db.FillWeeks());
        }

        [HttpPost]
        public JsonResult getSectionJson(string campusId, string classId, string selectCityId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsection(campusId, classId, user_id));
        }

        [HttpPost]
        public JsonResult getWeeklyLessonPlan(string empid, string begdate, string campusid, string classid, string sectionid, string subjectid)
        {
            db = new DatabaeseClass();
            return Json(db.FillWeeklyLessonPlan(empid, begdate, campusid, classid, sectionid, subjectid));
        }


        [HttpPost]
        public JsonResult getSubjectJson(string campusId, string classId, string sectionId, string teacherId, string selectsubjectId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsubject(campusId, classId, sectionId, user_id));
        }


        public JsonResult getFillLessonPlan(string campus, string schclass, string section, string subject)
        {
            //string subject;
            db = new DatabaeseClass();
            string pagename = Session["download"].ToString();
            return Json(db.FillLessonPlan(campus, schclass, section, subject, pagename, user_role), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAssignment(string filename)
        {
            din = new DatabaseInsertClass();

            return Json(din.UpdateLessonPlan(filename, user_id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult WeeklyPlanPP()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[51].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            DatabaseModel dm = new DatabaseModel();
            DatabaeseClass db = new DatabaeseClass();
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            dm.campus = db.getcampus();
            dm.classes = db.getclass(dm.campus[0].Value, user_id);
            dm.section = sl;
            dm.subject = sl;

            dm.campusid = dm.campus[0].Value;
            dm.classesid = "";
            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            if (user_role == "3000")
            {
                dm.student = db.getemployeename(user_id);
            }
            else
            {
                dm.student = sl;
            }

            return View(dm);
        }

        [HttpPost]
        public ActionResult WeeklyPlanPP(DatabaseModel dm)
        {
            DatabaseInsertClass dic = new DatabaseInsertClass();
            dic.insertweeklylessonplan(dm, user_role, user_id);

            if (HomeController.popup_status == "Success")
            {
                ViewBag.call_alert = "show_alert()";
                ViewBag.message_popup = "Success: Successfully Uploaded Lesson Plan";
                ViewBag.cssclass = "alert-success";
            }
            else
            {
                ViewBag.call_alert = "show_alert()";
                ViewBag.message_popup = "Error: An Error Occured While Submitting";
                ViewBag.cssclass = "alert-danger";
            }

            DatabaeseClass db = new DatabaeseClass();
            ModelState.Clear();
            dm = new DatabaseModel();
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            dm.campus = db.getcampus();
            dm.campusid = dm.campusid;

            dm.classes = db.getclass(dm.campusid, user_id);
            dm.classesid = dm.classesid;

            dm.section = db.getsection(dm.campusid, dm.classesid, user_id);
            dm.sectionid = dm.sectionid;

            dm.subject = db.getsubject(dm.campusid, dm.classesid, dm.sectionid, user_id);
            dm.subjectid = dm.subjectid;

            ViewBag.date = db.converteddisplaydate(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            if (user_role == "3000")
            {
                dm.student = db.getemployeename(user_id);
            }
            else
            {
                dm.student = sl;
            }

            return View("WeeklyPlanPP", dm);
        }

        [HttpPost]
        public JsonResult getWeeklyLessonPlanPP(DateTime begdate, DateTime enddate, string campusid, string classid, string sectionid, string subjectid)
        {
            db = new DatabaeseClass();
            return Json(db.FillWeeklyLessonPlanPP(campusid, classid, sectionid, subjectid, begdate, enddate));
        }

    }
}