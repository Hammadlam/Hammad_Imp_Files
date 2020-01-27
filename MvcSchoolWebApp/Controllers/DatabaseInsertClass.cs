using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace MvcSchoolWebApp.Controllers
{
    public class DatabaseInsertClass : Controller
    {
        /// <summary>
        /// tcode 1  = Create
        /// tcode 2  = Change
        /// tcode 3  = Display
        /// tcode 4  = Create, Change
        /// tcode 5  = Create, Display
        /// tcode 6  = Change, Display
        /// tcode 7  = Create, Change, Display
        /// tcode 8  = Renove, Display
        /// tcode 9  = Remove
        /// </summary>
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string popup_status;
        public readonly List<LoginModel> loginModel = (List<LoginModel>)System.Web.HttpContext.Current.Session["User_Rights"];
        public static List<Users> user_dtl;
        private Database.Database da = new Database.Database("Falconlocal");
        Data.data data = new data();

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

        public Page Page { get; private set; }

        public void InsertAssigment(DatabaseModel db)
        {
            DatabaeseClass dc = new DatabaeseClass();
            string menuid = "63200000";
            int tcode = 0;
            foreach (var item in loginModel)
            {
                if (item.menuid == menuid)
                    tcode = Convert.ToInt32(item.tcode);
            }
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DateTime datetime = Convert.ToDateTime(db.begdate);
            DateTime inserttime = dc.convertedinsertdate(DateTime.Now.ToString());
            string[] stdid = null;
            string notfcase = "";
            string query1 = "select isnull(max(recordno), 0) as recordno from lessonplan where " +
                            "campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "' " +
                            "and delind <> 'X' and lesncategory = '" + db.categoryid + "' and subjectid = '" + db.subjectid + "' ";
            using (SqlTransaction trans = con.BeginTransaction())
            {
                SqlCommand command = con.CreateCommand();
                command.Connection = con;
                command.Transaction = trans;
                try
                {
                    command.CommandText = query1;
                    int recordno = Convert.ToInt16(command.ExecuteScalar());

                    if (recordno >= 1)
                    {
                        recordno += 1;
                    }
                    else
                    {
                        recordno = 1;
                    }
                    if (tcode == 1 || tcode == 4 || tcode == 5 || tcode == 7)
                    {
                        String query3 = "INSERT INTO LESSONPLAN(lessonid, teacherid, lesncategory, campusid, classid, sectionid, subjectid, filepath, [filename], recordno, delind, begdate, enddate, upduser, upddate, updtime, dbtimestamp) " +
                                    "VALUES('" + db.increment + "', '" + db.teacherid + "', '" + db.categoryid + "', '" + db.campusid + "', '" + db.classesid + "', '" + db.sectionid + "', '" + db.subjectid + "' , '" + db.imagepath +
                                    "', '" + db.filename + "', '" + recordno + "', '', '" + datetime.ToString("yyyy-MM-dd") + "', '" + datetime.ToString("yyyy-MM-dd") + "', '', '', '', '" + inserttime.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        command.CommandText = query3;
                        command.ExecuteNonQuery();

                        command.CommandText = "select Count(*) from std0710 where campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "'";
                        int stdidcount = Convert.ToInt16(command.ExecuteScalar());
                        command.CommandText = "select stdid from std0710 where campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "'";

                        SqlDataReader sdr;
                        sdr = command.ExecuteReader();
                        if (sdr.HasRows)
                        {
                            int i = 0;
                            stdid = new string[stdidcount];
                            while (sdr.Read())
                            {
                                if (sdr["stdid"].ToString().Trim() != db.teacherid.Trim())
                                {
                                    stdid[i] = sdr["stdid"].ToString().Trim();
                                    i++;
                                }
                            }
                            sdr.Close();
                            switch (db.categoryid)
                            {
                                case "10":
                                    notfcase = "HA";
                                    break;

                                case "20":
                                    notfcase = "P";
                                    break;

                                case "30":
                                    notfcase = "CA";
                                    break;
                            }
                        }
                        command.CommandText = "select notftitle from znotificationcase where notfcase = '" + notfcase + "'";
                        string subject = command.ExecuteScalar().ToString();
                        string status = popnotification(stdid, db.teacherid, subject, notfcase);
                        if (status == "0")
                        {
                            throw new Exception();
                        }
                        trans.Commit();
                        HomeController.popup_status = "Success";
                    }
                    else
                    {
                        HomeController.popup_status = "Rights";
                    }
                }
                catch (Exception ex)
                {
                    HomeController.popup_status = "Error";
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                    con.Close();
                }
            }
        }

       
        public string updateCalender(string[][] data)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();

            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());

            var status = "success";

            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                if (data != null)
                {
                    int i = 0;

                    foreach (string[] subArray in data)
                    {


                        DateTime startDay = Convert.ToDateTime(subArray[2]);

                        DateTime endDay = Convert.ToDateTime(subArray[3]);



                        TimeSpan ts = endDay - startDay;

                        cmd.CommandText = "update emp0290 set empid='" + subArray[13] + "', begdate='" + startDay + "' , enddate = '" + endDay + "', subpagtype = '', recordno = '" + (i + 1) + "' , " +
                                          "delind = '" + (i + 1) + "'," +
                                          "upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', updtime = '" + insertdate.ToString("HH:mm:ss") + "', starttime = '" + startDay + "', endtime = '" + endDay + "', " +
                                          "tothours = '" + ts.TotalHours + "', remarks =  '" + subArray[12] + "', location = '" + subArray[9] + "', clientid = '" + subArray[8] + "', " +
                                          "purpose='" + subArray[4] + "' , vwith = '" + subArray[11] + "', attype = '" + subArray[10] + "',  dbtimestmp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' where pid = '" + subArray[0] + "'";


                        cmd.ExecuteNonQuery();


                    }




                }


                trans.Commit();




            }
            catch (Exception ex)
            {
                trans.Rollback();
                status = "false";
            }
            finally
            {
                trans.Dispose();
                con.Close();
            }

            return status;

        }

        public void insertCalender(string[][] data)
        {
            var user_id = System.Web.HttpContext.Current.Session["User_Id"].ToString();

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();

            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());

            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                if (data != null)
                {
                    int i = 0;

                    foreach (string[] subArray in data)
                    {
                        // foreach (string i in subArray)
                        // {



                        DateTime startDay = Convert.ToDateTime(subArray[2]);

                        DateTime endDay = Convert.ToDateTime(subArray[3]);



                        TimeSpan ts = endDay - startDay;

                        cmd.CommandText = "insert into emp0290 (empid, begdate, enddate, subpagtype, recordno, " +
                                          "delind, creuser, credate, cretime, " +
                                          "upduser, upddate, updtime, attype, starttime, endtime, " +
                                          "tothours, remarks, location, clientid, tinusr, " +
                                          "tinlat, tinlong, toutusr, toutlat, toutlong, " +
                                          "purpose, dbtimestmp,pid,vwith) " +
                                          "VALUES('" + subArray[8] + "','" + startDay + "','" + endDay + "',' ', '" + (i + 1) + "', " +
                                          "'" + (i + 1) + "','"+ user_id + "','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', " +
                                          "' ','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', '" + subArray[10] + "', '" + startDay + "','" + endDay + "', " +
                                          "'" + ts.TotalHours + "','" + subArray[7] + "', '" + subArray[6] + "','" + subArray[9] + "', ' ', " +
                                          "' ', ' ', ' ',' ', ' ', " +
                                          "'" + subArray[4] + "', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + subArray[0] + "', '" + subArray[5] + "')";


                        cmd.ExecuteNonQuery();




                        //}
                    }


                    // for (int i = 0; i < data.Length; i++)
                    //{

                    //     for (int j = 0; j < data.Length; j++)
                    //     { 


                    //     }
                    //                 if (instname[i] != "")
                    //                 {
                    //                     cmd.CommandText = "insert into apl0520 (aplid, begdate, enddate, asubpagtyp, recordno, " +
                    //                                       "lineid, delind, creuser, credate, cretime, " +
                    //                                       "upduser, upddate, updtime, startdate, finishdate, " +
                    //                                       "apledu, instdept, instname, instcity, inststate, " +
                    //                                       "instctry, aplprof, inudfield1, inudfield2, inudfield3, " +
                    //                                       "inudfield4, dbtimestmp) " +
                    //                                       "VALUES('" + aplid + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "',' ', '" + (i + 1) + "', " +
                    //                                       "'" + (i + 1) + "',' ','" + user_id + "','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', " +
                    //                                       "' ','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', '" + sdateedu[i].ToString("yyyy-MM-dd") + "','" + fdateedu[i].ToString("yyyy-MM-dd") + "', " +
                    //                                       "'" + degreeedu[i] + "',' ', '" + instname[i] + "',' ', ' ', " +
                    //                                       "'PK', ' ', '" + majors[i] + "','" + gpa[i] + "', ' ', " +
                    //                                       "' ', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";


                    //                     cmd.ExecuteNonQuery();
                    //                }
                    //     }

                }


                trans.Commit();


            }
            catch (Exception ex)
            {
                trans.Rollback();

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }


        


        public void updateApl(string userid, string act)
        {
            var user_id = System.Web.HttpContext.Current.Session["User_Id"].ToString();

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();

            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());

            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                
                    int i = 0;

                string action = ""; 

                if (act == "0") {

                    action = "rejected";

                } else if (act == "1") {

                    action = "accepted";

                }
                 

                        cmd.CommandText = "update apl0120 set sstatus = '"+action+"' where jobkey = '"+ userid + "' ";


                        cmd.ExecuteNonQuery();




                trans.Commit();


            }
            catch (Exception ex)
            {
                trans.Rollback();

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }




        public string popnotification(string[] reciparray, string sender, string subject, string notificationcase)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            SqlTransaction trans = null;
            DatabaeseClass dc = new DatabaeseClass();
            string status = "";
            try
            {
                con.Open();
                trans = con.BeginTransaction();
                SqlCommand command = con.CreateCommand();
                command.Connection = con;
                command.Transaction = trans;
                command.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = command.ExecuteScalar().ToString();
                for (int j = 0; j < reciparray.Length; j++)
                {
                    command.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                         "values ('" + msgid + "','1', '" + reciparray[j] + "', '', '" + sender + "', '" + subject + "', '', '','X','', '" + dc.convertservertopsttimezone(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + notificationcase + "')";
                    command.ExecuteNonQuery();
                }
                trans.Commit();
                status = "1";
            }
            catch (Exception ex)
            {
                status = "0";
                trans.Rollback();
            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
            return status;
        }

        public string lessonnotification(string[] reciparray, string sender, string subject, string notificationcase)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            SqlTransaction trans = null;
            DatabaeseClass dc = new DatabaeseClass();
            string status = "";
            try
            {
                con.Open();
                trans = con.BeginTransaction();
                SqlCommand command = con.CreateCommand();
                command.Connection = con;
                command.Transaction = trans;
                command.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = command.ExecuteScalar().ToString();
                for (int j = 0; j < reciparray.Length; j++)
                {
                    command.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                         "values ('" + msgid + "','1', '" + reciparray[j] + "', '', '" + sender + "', " + subject + ", '', '','X','', '" + dc.convertservertopsttimezone(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + notificationcase + "')";
                    command.ExecuteNonQuery();
                }
                trans.Commit();
                status = "1";
            }
            catch (Exception ex)
            {
                status = "0";
                trans.Rollback();
            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
            return status;
        }

        public void uploadimage(string filepath, string filename, string imgtype, string empid, string imgid)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            string path = "~/Uploads/Images/";


            string query2 = "insert into imageobj (imageid, imagetype, imagepath) values ('" + imgid + "', '" + imgtype + "', '" + path + imgid + " - " + filename + "')";
            string query3 = "update emp0170 set imageid = '" + imgid + "' where empid = '" + empid + "'";

            using (SqlTransaction trans = con.BeginTransaction())
            {
                SqlCommand command = con.CreateCommand();
                command.Connection = con;
                command.Transaction = trans;

                try
                {
                    command.CommandText = query2;
                    command.ExecuteNonQuery();
                    command.CommandText = query3;
                    command.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    System.IO.File.Delete(filepath + imgid + " - " + filename);
                }
                finally
                {
                    trans.Dispose();
                    con.Close();
                }

            }
        }

        public bool UpdateLessonPlan(string filename, string user_role)
        {
            DatabaeseClass dc = new DatabaeseClass();
            string menuid = "63300000";
            int tcode = 0;
            DateTime insertdate = dc.convertedinsertdate(DateTime.Now.ToString());
            foreach (var item in loginModel)
            {
                if (item.menuid == menuid)
                    tcode = Convert.ToInt32(item.tcode);
            }
            if (tcode == 8 || tcode == 9)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
                con.Open();
                string query1 = "update lessonplan set delind = 'X', upduser = '" + user_role + "', upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', updtime= '" + insertdate.ToString("HH:mm:ss") + "', dbtimestamp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                    "where filename = '" + filename + "'";
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    SqlCommand command = con.CreateCommand();
                    command.Connection = con;
                    command.Transaction = trans;
                    try
                    {
                        command.CommandText = query1;
                        command.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                        con.Close();
                    }
                }
                return true;
            }
            else
                return false;
        }

        public void upd_personalinfo(string query1)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            personalinfoController pic = new personalinfoController();
            SqlCommand command = new SqlCommand(query1, con);
            try
            {
                command.CommandText = query1;
                command.ExecuteNonQuery();
                pic.getpopupmessage("show_alert();", "Data Successfully Updated", "alert-success");
            }
            catch (Exception ex)
            {
                pic.getpopupmessage("show_alert();", "Error Occured : Rolling Back your Previous Data", "alert-danger");
            }
            finally
            {
                con.Close();
            }
        }

        public void InsertTimetable(DatabaseModel db)
        {
            string menuid = "62800000";
            int tcode = 0;
            foreach (var item in loginModel)
            {
                if (item.menuid == menuid)
                    tcode = Convert.ToInt32(item.tcode);
            }

            DatabaeseClass dc = new DatabaeseClass();
            DateTime insertdate = dc.convertservertopsttimezone(DateTime.Now.ToString());
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();

            string query1 = "select isnull(max(recordno), 0) as recordno from timetable where " +
                            "campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "' and ttcategory = '" + db.categoryid + "'";

            using (SqlTransaction trans = con.BeginTransaction())
            {
                SqlCommand command = con.CreateCommand();
                command.Connection = con;
                command.Transaction = trans;
                string notfcase = "";
                string[] reciparray = null;
                try
                {
                    command.CommandText = query1;
                    int record = 0;
                    record = Convert.ToInt32(command.ExecuteScalar());



                    // string query3 = ;
                    string recip_arr_query = "";
                    string recip_arr_count = "";
                    string colname = "";
                    if (record >= 1 && (tcode == 2 || tcode == 4 || tcode == 6 || tcode == 7))
                    {
                        command.CommandText = "update timetable set delind = 'X', upduser = '" + db.teacherid + "', upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', updtime= '" + insertdate.ToString("HH:mm:ss") + "', dbtimestamp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                            "where recordno = '" + record + "' and campusid='" + db.campusid + "' and classid = '" + db.classesid + "' and ttcategory = '" + db.categoryid + "' and sectionid = '" + db.sectionid + "'";
                        command.ExecuteNonQuery();
                        record += 1;
                        command.CommandText = "INSERT INTO TIMETABLE(timetbid, ttcategory, adminid, campusid, classid, sectionid, filepath, [filename], recordno, delind, begdate, enddate, upduser, upddate, updtime, dbtimestamp) " +
                                        "VALUES('" + db.increment + "', '" + db.categoryid + "', '" + db.teacherid + "', '" + db.campusid + "', '" + db.classesid + "', '" + db.sectionid + "', '" + db.imagepath +
                                        "', '" + db.filename + "', '" + record + "', '', '" + db.begdate + "', '" + db.enddate + "', '', '', '', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        command.ExecuteNonQuery();

                        switch (db.categoryid)
                        {
                            case "101":
                                notfcase = "MT";
                                recip_arr_count = "select Count(distinct userid) from usr01 usr " +
                                                "inner join emp0710 e7 on usr.userid = e7.empid " +
                                                "where usr.menuprof = '50000000' and e7.campusid = '" + db.campusid + "' and e7.delind <> 'X'";
                                recip_arr_query = "select distinct userid from usr01 usr " +
                                                "inner join emp0710 e7 on usr.userid = e7.empid " +
                                                "where usr.menuprof = '50000000' and e7.campusid = '" + db.campusid + "' and e7.delind <> 'X'";
                                colname = "userid";
                                break;

                            case "102":
                                notfcase = "TT";
                                recip_arr_count = "select Count(distinct empid) from emp0710 where campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "' and delind <> 'X'";
                                recip_arr_query = "select distinct empid from emp0710 where campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "' and delind <> 'X'";
                                colname = "empid";
                                break;

                            case "103":
                                notfcase = "CT";
                                recip_arr_count = "select Count(stdid) from std0710 where campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "' and delind <> 'X'";
                                recip_arr_query = "select stdid from std0710 where campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "' and delind <> 'X'";
                                colname = "stdid";
                                break;
                        }
                        command.CommandText = recip_arr_count;
                        int recipcount = Convert.ToInt16(command.ExecuteScalar());

                        SqlDataReader sdr = command.ExecuteReader();
                        if (sdr.HasRows)
                        {
                            reciparray = new string[recipcount];
                            int i = 0;
                            while (sdr.Read())
                            {
                                reciparray[i] = sdr[colname].ToString();
                                i++;
                            }
                        }

                        command.CommandText = "select notftitle from znotificationcase where notfcase = '" + notfcase + "'";
                        string subject = command.ExecuteScalar().ToString();

                        string status = popnotification(reciparray, db.teacherid, subject, notfcase);
                        if (status == "0")
                        {
                            throw new Exception();
                        }

                        trans.Commit();
                        TimeTableController.popup_status = "Success";
                    }
                    else if (record == 0 && (tcode == 1 || tcode == 4 || tcode == 5 || tcode == 7))
                    {
                        record += 1;
                        command.CommandText = "INSERT INTO TIMETABLE(timetbid, ttcategory, adminid, campusid, classid, sectionid, filepath, [filename], recordno, delind, begdate, enddate, upduser, upddate, updtime, dbtimestamp) " +
                                        "VALUES('" + db.increment + "', '" + db.categoryid + "', '" + db.teacherid + "', '" + db.campusid + "', '" + db.classesid + "', '" + db.sectionid + "', '" + db.imagepath +
                                        "', '" + db.filename + "', '" + record + "', '', '" + db.begdate + "', '" + db.enddate + "', '', '', '', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        command.ExecuteNonQuery();

                        switch (db.categoryid)
                        {
                            case "101":
                                notfcase = "MT";
                                recip_arr_count = "select Count(distinct userid) from usr01 usr " +
                                                "inner join emp0710 e7 on usr.userid = e7.empid " +
                                                "where usr.menuprof = '50000000' and e7.campusid = '" + db.campusid + "' and e7.delind <> 'X'";
                                recip_arr_query = "select distinct userid from usr01 usr " +
                                                "inner join emp0710 e7 on usr.userid = e7.empid " +
                                                "where usr.menuprof = '50000000' and e7.campusid = '" + db.campusid + "' and e7.delind <> 'X'";
                                colname = "userid";
                                break;

                            case "102":
                                notfcase = "TT";
                                recip_arr_count = "select Count(distinct empid) from emp0710 where campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "' and delind <> 'X'";
                                recip_arr_query = "select distinct empid from emp0710 where campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "' and delind <> 'X'";
                                colname = "empid";
                                break;

                            case "103":
                                notfcase = "CT";
                                recip_arr_count = "select Count(stdid) from std0710 where campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "' and delind <> 'X'";
                                recip_arr_query = "select stdid from std0710 where campusid = '" + db.campusid + "' and classid = '" + db.classesid + "' and sectionid = '" + db.sectionid + "' and delind <> 'X'";
                                colname = "stdid";
                                break;
                        }

                        command.CommandText = recip_arr_count;
                        int recipcount = Convert.ToInt16(command.ExecuteScalar());

                        SqlDataReader sdr;
                        command.CommandText = recip_arr_query;
                        sdr = command.ExecuteReader();
                        if (sdr.HasRows)
                        {
                            reciparray = new string[recipcount];
                            int i = 0;
                            while (sdr.Read())
                            {
                                if (sdr[colname].ToString().Trim() != user_id)
                                {
                                    reciparray[i] = sdr[colname].ToString();
                                    i++;
                                }
                            }
                            sdr.Close();
                        }

                        command.CommandText = "select notftitle from znotificationcase where notfcase = '" + notfcase + "'";
                        string subject = command.ExecuteScalar().ToString();

                        string status = popnotification(reciparray, db.teacherid, subject, notfcase);
                        if (status == "0")
                        {
                            throw new Exception();
                        }

                        trans.Commit();
                        TimeTableController.popup_status = "Success";
                    }
                    else
                    {
                        TimeTableController.popup_status = "Rights";
                    }
                }
                catch (Exception ex)
                {
                    TimeTableController.popup_status = "Error";
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                    con.Close();
                }
            }
        }

        public void InsertAttendance(string[] studentId, string[] status, string subjectId, DateTime dateId)
        {
            DatabaeseClass dc = new DatabaeseClass();
            string menuid = "62300000";
            int tcode = 0;
            foreach (var item in loginModel)
            {
                if (item.menuid == menuid)
                    tcode = Convert.ToInt32(item.tcode);
            }

            int recordno = 0;
            if (subjectId == "")
                subjectId = "Full day";
            DateTime insertdate = dc.convertedinsertdate(DateTime.Now.ToString());
            string date = dateId.ToString("yyyy-MM-dd");
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            SqlTransaction trans;
            SqlCommand command = con.CreateCommand();
            trans = con.BeginTransaction();
            command.Connection = con;
            command.Transaction = trans;
            string[] absent_students;
            string[] parentids;
            int count_absent = 0;
            for (int i = 0; i < status.Length; i++)
            {
                if (status[i] == "Absent")
                {
                    count_absent += 1;
                }
            }
            absent_students = new string[count_absent];
            int abs = 0;
            for (int i = 0; i < studentId.Length; i++)
            {
                if (status[i] == "Absent")
                {
                    absent_students[abs] = studentId[i];
                    abs++;
                }
            }

            parentids = new string[count_absent];
            for (int i = 0; i < absent_students.Length; i++)
            {
                command.CommandText = "select parentid from stdmain where stdid = '" + absent_students[i] + "' and delind <> 'X'";
                parentids[i] = command.ExecuteScalar().ToString();
            }

            command.CommandText = "select isnull(max(recordno),0) as recordno from std0220 where empid = '" + studentId[0] + "' and begdate between '" + dateId.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + dateId.ToString("yyyy-MM-dd HH:mm:ss") + "' and remarks = '" + subjectId + "' and delind <> 'X' ";
            recordno = Convert.ToInt16(command.ExecuteScalar());

            try
            {
                if (recordno > 0 && (tcode == 2 || tcode == 4 || tcode == 6 || tcode == 7))
                {
                    for (int i = 0; i < studentId.Length; i++)
                    {
                        command.CommandText = "update std0220 set delind = 'X', upduser = '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', updtime = '" + insertdate.ToString("HH:mm:ss") + "', dbtimestmp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' where empid = '" + studentId[i] + "' and recordno = '" + recordno + "' and begdate between '" + dateId.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + dateId.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                        command.ExecuteNonQuery();
                    }

                    for (int i = 0; i < studentId.Length; i++)
                    {
                        if (status[i] == "Present")
                        {
                            command.CommandText = "INSERT INTO std0220 (empid, begdate, enddate, subpagtype, recordno, delind, creuser, credate, cretime, upduser, upddate, updtime, lvtype, halfdayind, lvdays, caldays, remarks,dbtimestmp) " +
                            "Values('" + studentId[i] + "', '" + date + "', '" + date + "', ' ', '" + (recordno + 1) + "', ' ', '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', '" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("HH:mm:ss") + "', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '" + subjectId + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            command.ExecuteNonQuery();
                        }
                        else if (status[i] == "Absent")
                        {
                            command.CommandText = "INSERT INTO std0220 (empid, begdate, enddate, subpagtype, recordno, delind, creuser, credate, cretime, upduser, upddate, updtime, lvtype, halfdayind, lvdays, caldays, remarks,dbtimestmp) " +
                                "Values('" + studentId[i] + "', '" + date + "', '" + date + "', ' ', '" + (recordno + 1) + "', ' ', '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', '" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("HH:mm:ss") + "', ' ', ' ', ' ', ' ', '', '1', ' ', '" + subjectId + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            command.ExecuteNonQuery();
                        }
                        else if (status[i] == "Leave")
                        {
                            command.CommandText = "INSERT INTO std0220 (empid, begdate, enddate, subpagtype, recordno, delind, creuser, credate, cretime, upduser, upddate, updtime, lvtype, halfdayind, lvdays, caldays, remarks,dbtimestmp) " +
                                "Values('" + studentId[i] + "', '" + date + "', '" + date + "', ' ', '" + (recordno + 1) + "', ' ', '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', '" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("HH:mm:ss") + "', ' ', ' ', ' ', ' ', 'X', ' ', ' ', '" + subjectId + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            AttendanceController.upd_attd_popup = "Error";
                            throw new Exception();
                        }
                    }
                    command.CommandText = "select notftitle from znotificationcase where notfcase = 'SA'";
                    string subject = command.ExecuteScalar().ToString();

                    string notf_status = popnotification(parentids, System.Web.HttpContext.Current.Session["User_Id"].ToString(), subject, "SA");
                    if (notf_status == "0")
                    {
                        throw new Exception();
                    }

                    trans.Commit();
                    AttendanceController.upd_attd_popup = "Success";

                }
                else if (recordno == 0 && (tcode == 1 || tcode == 4 || tcode == 5 || tcode == 7))
                {
                    for (int i = 0; i < studentId.Length; i++)
                    {
                        if (status[i] == "Present")
                        {
                            command.CommandText = "INSERT INTO std0220 (empid, begdate, enddate, subpagtype, recordno, delind, creuser, credate, cretime, upduser, upddate, updtime, lvtype, halfdayind, lvdays, caldays, remarks,dbtimestmp) " +
                            "Values('" + studentId[i] + "', '" + date + "', '" + date + "', ' ', '" + (recordno + 1) + "', ' ', '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', '" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("HH:mm:ss") + "', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '" + subjectId + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            command.ExecuteNonQuery();
                        }
                        else if (status[i] == "Absent")
                        {
                            command.CommandText = "INSERT INTO std0220 (empid, begdate, enddate, subpagtype, recordno, delind, creuser, credate, cretime, upduser, upddate, updtime, lvtype, halfdayind, lvdays, caldays, remarks,dbtimestmp) " +
                                "Values('" + studentId[i] + "', '" + date + "', '" + date + "', ' ', '" + (recordno + 1) + "', ' ', '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', '" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("HH:mm:ss") + "', ' ', ' ', ' ', ' ', '', '1', ' ', '" + subjectId + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            command.ExecuteNonQuery();
                        }
                        else if (status[i] == "Leave")
                        {
                            command.CommandText = "INSERT INTO std0220 (empid, begdate, enddate, subpagtype, recordno, delind, creuser, credate, cretime, upduser, upddate, updtime, lvtype, halfdayind, lvdays, caldays, remarks,dbtimestmp) " +
                                "Values('" + studentId[i] + "', '" + date + "', '" + date + "', ' ', '" + (recordno + 1) + "', ' ', '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', '" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("HH:mm:ss") + "', ' ', ' ', ' ', ' ', 'X', ' ', ' ', '" + subjectId + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            AttendanceController.upd_attd_popup = "Error";
                            throw new Exception();
                        }
                    }
                    command.CommandText = "select notftitle from znotificationcase where notfcase = 'SA'";
                    string subject = command.ExecuteScalar().ToString();

                    string notf_status = popnotification(parentids, System.Web.HttpContext.Current.Session["User_Id"].ToString(), subject, "SA");
                    if (notf_status == "0")
                    {
                        throw new Exception();
                    }

                    trans.Commit();
                    AttendanceController.upd_attd_popup = "Success";
                }
                else
                    AttendanceController.upd_attd_popup = "Rights";

            }
            catch (Exception ex)
            {
                AttendanceController.upd_attd_popup = "Error";
                trans.Rollback();
            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        public void InsertEmployeeAttendance(string empid, string clientid, DateTime date, string time, string type, string remarks, string latitude, string longitude)
        {
            DatabaeseClass dc = new DatabaeseClass();
            if (latitude == null && longitude == null)
            {
                latitude = "";
                longitude = "";
            }
            string menuid = "62300000";
            int tcode = 0;
            foreach (var item in loginModel)
            {
                if (item.menuid == menuid)
                    tcode = Convert.ToInt32(item.tcode.Trim());
            }

            string newdate = date.ToString("yyyy-MM-dd") + " " + time;
            DateTime dt = DateTime.Parse(newdate);
            //dt = dc.convertservertousertimezone(dt.ToString());

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            SqlTransaction trans;
            SqlCommand command = con.CreateCommand();
            trans = con.BeginTransaction();
            command.Connection = con;
            command.Transaction = trans;
            int recordno = 0;
            try
            {
                if (type == "01")   // ESS Formtype
                {
                    empid = Convert.ToString(System.Web.HttpContext.Current.Session["User_Id"]);
                    dt = dc.convertservertopsttimezone(DateTime.Now.ToString());
                }

                command.CommandText = "select isnull(max(recordno),0) as recordno from emp0280 where empid = '" + empid + "' and clientid = '" + clientid + "' " +
                                      "and delind <> 'X' and begdate >= '" + dt.ToString("yyyy-MM-dd") + "' and begdate < '" + dt.AddDays(1).ToString("yyyy-MM-dd") + "'";
                recordno = Convert.ToInt16(command.ExecuteScalar());

                command.CommandText = "select isactive from emp0280 where empid = '" + empid + "' and delind <> 'X' and clientid = '" + clientid + "' and upduser = '' and toutusr = '' " +
                                      "and begdate >= '" + dt.ToString("yyyy-MM-dd") + "' and begdate < '" + dt.AddDays(1).ToString("yyyy-MM-dd") + "' " +
                                      "and recordno = (select max(recordno) from emp0280 where empid = '" + empid + "' and delind <> 'X' and clientid = '" + clientid + "' and upduser = '' and toutusr = '' " +
                                      "and begdate >= '" + dt.ToString("yyyy-MM-dd") + "' and begdate < '" + dt.AddDays(1).ToString("yyyy-MM-dd") + "')";
                string isactive = (string)command.ExecuteScalar() ?? "";

                if (recordno == 0)
                {
                    recordno += 1;
                    command.CommandText = "insert into emp0280(empid, begdate, enddate, clientid, subpagtype, " +
                                      "recordno, delind, creuser, credate, cretime, " +
                                      "upduser, upddate, updtime, locat, tinusr, " +
                                      "tinlat, tinlong, toutusr, toutlat, toutlong, " +
                                      "isactive, remarks, remarkstout) values " +
                                      "('" + empid + "', '" + dt + "', '', '" + clientid + "', '', " +
                                      "'" + recordno + "', '', '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', '" + dt.ToString("yyyy/MM/dd") + "', '" + dt.ToString("HH:mm:ss") + "', " +
                                      "'', '', '', '', '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', '" + latitude + "', " +
                                      "'" + longitude + "', '', '', '', 'X', '" + remarks + "', '')";
                    command.ExecuteNonQuery();
                }
                else if (recordno > 0 && isactive != "X")
                {
                    recordno += 1;
                    command.CommandText = "insert into emp0280(empid, begdate, enddate, clientid, subpagtype, " +
                                      "recordno, delind, creuser, credate, cretime, " +
                                      "upduser, upddate, updtime, locat, tinusr, " +
                                      "tinlat, tinlong, toutusr, toutlat, toutlong, " +
                                      "isactive, remarks, remarkstout) values " +
                                      "('" + empid + "', '" + dt + "', '', '" + clientid + "', '', " +
                                      "'" + recordno + "', '', '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', '" + dt.ToString("yyyy/MM/dd") + "', '" + dt.ToString("HH:mm:ss") + "', " +
                                      "'', '', '', '', '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', '" + latitude + "', " +
                                      "'" + longitude + "', '', '', '', 'X', '" + remarks + "', '')";
                    command.ExecuteNonQuery();
                }
                else if (recordno > 0 && isactive == "X")
                {
                    command.CommandText = "update emp0280 set enddate = '" + dt + "', isactive = '', toutusr = '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', " +
                                          "toutlat = '" + latitude + "', toutlong = '" + longitude + "', remarkstout = '" + remarks + "' " +
                                          "where empid = '" + empid + "' and clientid = '" + clientid + "' and upduser = '' and delind <> 'X' " +
                                          "and begdate >= '" + dt.ToString("yyyy-MM-dd") + "' and begdate < '" + dt.AddDays(1).ToString("yyyy-MM-dd") + "'" +
                                          "and recordno = (select max(recordno) from emp0280 where empid = '" + empid + "' and delind <> 'X' and clientid = '" + clientid + "' and upduser = '' and toutusr = '' " +
                                          "and begdate >= '" + dt.ToString("yyyy-MM-dd") + "' and begdate < '" + dt.AddDays(1).ToString("yyyy-MM-dd") + "')";

                    command.ExecuteNonQuery();
                }
                trans.Commit();

            }
            catch (Exception ex)
            {
                trans.Rollback();
            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        public void InsertMarks(string[] studentId, string[] exammarks, string[] projectmarks, string[] testmarks,
            string[] oralmarks, string[] assignmarks, string[] p1marks, string[] p2marks, string[] p3marks, DatabaseModel dm)
        {
            DatabaeseClass dc = new DatabaeseClass();
            string menuid = "62400000";
            int tcode = 0;
            foreach (var item in loginModel)
            {
                if (item.menuid == menuid)
                    tcode = Convert.ToInt32(item.tcode);
            }
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DateTime insertdate = dc.convertedinsertdate(DateTime.Now.ToString());
            string max_record = "select isnull(max(recordno), 0) from schresult where stdid='" + studentId[0] + "' and campusid = '" + dm.campusid + "' and classid = '" + dm.classesid + "' and subjectid = '" + dm.subjectid +
            "' and subresltyp = '" + dm.moduleid + "' and resulttype IN (select reslttyp from schresltype where  subresltptxt = '" + dm.moduletxt + "' )";

            SqlCommand command = new SqlCommand(max_record, con);
            int record = 0;
            record = Convert.ToInt32(command.ExecuteScalar());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;
                if (record > 0 && (tcode == 2 || tcode == 4 || tcode == 6 || tcode == 7))
                {
                    for (int i = 0; i < studentId.Length; i++)
                    {
                        cmd.CommandText = "update schresult set delind = 'X', upduser = '" + dm.teacherid + "', upddate = " + insertdate.ToString("yyyy-MM-dd") + ", updtime= '" + insertdate.ToString("HH:mm:ss") + "', dbtimestamp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                                "where recordno = '" + record + "' and stdid='" + studentId[i] + "' and classid = '" + dm.classesid + "' and subjectid = '" + dm.subjectid +
                                                "' and subresltyp = '" + dm.moduleid + "' and resulttype IN (select reslttyp from schresltype where  subresltptxt = '" + dm.moduletxt + "' )";
                        cmd.ExecuteNonQuery();
                    }

                    for (int i = 0; i < studentId.Length; i++)
                    {
                        double j = (Convert.ToDouble(exammarks[i]) + Convert.ToDouble(projectmarks[i]) + Convert.ToDouble(assignmarks[i]) + Convert.ToDouble(p1marks[i]) + Convert.ToDouble(p2marks[i]) + Convert.ToDouble(p3marks[i])
                        + Convert.ToDouble(oralmarks[i]) + Convert.ToDouble(testmarks[i]));
                        double k = (Convert.ToDouble(dm.marks) + Convert.ToDouble(dm.marksp2) + Convert.ToDouble(dm.marksp3));

                        dm.percentage = (j / k) * 100;
                        dm.percentage = Convert.ToDouble(dm.percentage.ToString("N3"));

                        string query = "INSERT INTO Schresult(resulttype,subresltyp,stdid,campusid,classid,sectionid,teacherid,subjectid,recordno,testmarks,oralmarks,assignmentmarks,examinationmarks,projectmarks,p1,p2,p3," +
                         "totalmarks,totalmarksp2,totalmarksp3, obtainedmarks,percentage,delind,begdate,enddate,upduser,upddate,updtime,dbtimestamp) " +
                        "values ((select reslttyp from schresltype where [subresltptxt]='" + dm.moduletxt + "'), " +
                       "(select subresltyp from schresltype where [subresltptxt] = '" + dm.moduletxt + "'), '" + studentId[i] + "','" + dm.campusid + "','" +
                       dm.classesid + "','" + dm.sectionid + "','" + dm.teacherid + "','" + dm.subjectid + "','" + (record + 1) + "','" + testmarks[i] + "','" + oralmarks[i] + "','" +
                       assignmarks[i] + "','" + exammarks[i] + "','" + projectmarks[i] + "','" + p1marks[i] + "','" + p2marks[i] + "','" + p3marks[i] + "','" + dm.marks + "','" + dm.marksp2 + "','" + dm.marksp3 + "', '" + j + "','" + dm.percentage + "','','" + dm.date + "','" + dm.date + "',' ',' ',' ','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    HomeController.popup_status = "Success";
                }
                else if (record == 0 && (tcode == 1 || tcode == 4 || tcode == 5 || tcode == 7))
                {
                    for (int i = 0; i < studentId.Length; i++)
                    {
                        double j = (Convert.ToDouble(exammarks[i]) + Convert.ToDouble(projectmarks[i]) + Convert.ToDouble(assignmarks[i]) + Convert.ToDouble(p1marks[i]) + Convert.ToDouble(p2marks[i]) + Convert.ToDouble(p3marks[i])
                        + Convert.ToDouble(oralmarks[i]) + Convert.ToDouble(testmarks[i]));
                        double k = Convert.ToDouble(dm.marks);

                        dm.percentage = (j / k) * 100;
                        dm.percentage = Convert.ToDouble(dm.percentage.ToString("N3"));

                        string query = "INSERT INTO Schresult(resulttype,subresltyp,stdid,campusid,classid,sectionid,teacherid,subjectid,recordno,testmarks,oralmarks,assignmentmarks,examinationmarks,projectmarks,p1,p2,p3," +
                         "totalmarks,totalmarksp2,totalmarksp3,obtainedmarks,percentage,delind,begdate,enddate,upduser,upddate,updtime,dbtimestamp) " +
                        "values ((select reslttyp from schresltype where [subresltptxt]='" + dm.moduletxt + "'), " +
                       "(select subresltyp from schresltype where [subresltptxt] = '" + dm.moduletxt + "'), '" + studentId[i] + "','" + dm.campusid + "','" +
                       dm.classesid + "','" + dm.sectionid + "','" + dm.teacherid + "','" + dm.subjectid + "','" + (record + 1) + "','" + testmarks[i] + "','" + oralmarks[i] + "','" +
                       assignmarks[i] + "','" + exammarks[i] + "','" + projectmarks[i] + "','" + p1marks[i] + "','" + p2marks[i] + "','" + p3marks[i] + "','" + dm.marks + "','" + dm.marksp2 + "','" + dm.marksp3 + "','" + j + "','" + dm.percentage + "','','" + dm.date + "','" + dm.date + "',' ',' ',' ', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    HomeController.popup_status = "Success";
                }
                else
                    HomeController.popup_status = "Rights";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";
            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        public void addprofiledata(Personalinfo pro)
        {
            da.CreateConnection();
            da.OpenConnection();
            da.obj_sqlcommand.ExecuteNonQuery();
            da.CloseConnection();
        }

        public bool UpdateTimetable(string filename, string user_id)
        {
            DatabaeseClass dc = new DatabaeseClass();
            string menuid = "62900000";
            int tcode = 0;
            foreach (var item in loginModel)
            {
                if (item.menuid == menuid)
                    tcode = Convert.ToInt32(item.tcode);
            }
            DateTime insertdate = dc.convertedinsertdate(DateTime.Now.ToString());
            if (tcode == 8 || tcode == 9)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
                con.Open();
                string query1 = "update timetable set delind = 'X', upduser = '" + user_id + "', upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', updtime= '" + insertdate.ToString("HH:mm:ss") + "', dbtimestamp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                "where filename = '" + filename + "'";
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    SqlCommand command = con.CreateCommand();
                    command.Connection = con;
                    command.Transaction = trans;

                    try
                    {
                        command.CommandText = query1;
                        command.ExecuteNonQuery();
                        trans.Commit();

                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                        con.Close();
                    }

                }
                return true;
            }
            else
                return false;
        }

        public void InsertGradeActivity(string[] studentId, string[] status, string[] assignppt, string[] gk, string[] behave, string[] discp, string[] clean, string[] compliance, string[] task, string[] teachercom, string campusId, string classId, string sectionId, string user_id)
        {
            DatabaeseClass dc = new DatabaeseClass();
            string menuid = "62500000";
            int tcode = 0;
            foreach (var item in loginModel)
            {
                if (item.menuid == menuid)
                    tcode = Convert.ToInt32(item.tcode);
            }
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DateTime insertdate = dc.convertedinsertdate(DateTime.Now.ToString());
            string max_record = "select isnull(max(recordno), 0) from schactivitygrade where stdid='" + studentId[0] + "' and classid = '" + classId + "' and campusid = '" + campusId + "'";

            SqlCommand command = new SqlCommand(max_record, con);
            int record = 0;
            record = Convert.ToInt32(command.ExecuteScalar());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                if (record > 0 && (tcode == 2 || tcode == 4 || tcode == 6 || tcode == 7))
                {
                    for (int i = 0; i < studentId.Length; i++)
                    {
                        cmd.CommandText = "update schactivitygrade set delind = 'X', upduser = '" + user_id + "', upddate = " + insertdate.ToString("yyyy-MM-dd") + ", updtime= '" + insertdate.ToString("HH:mm:ss") + "', dbtimestamp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                                "where recordno = '" + record + "' and stdid='" + studentId[i] + "' and classid = '" + classId + "' and campusid = '" + campusId + "'";
                        cmd.ExecuteNonQuery();
                    }
                    for (int i = 0; i < studentId.Length; i++)
                    {
                        if (status[i] == "" || assignppt[i] == "" || gk[i] == "" || behave[i] == "" || discp[i] == "" || clean[i] == "" || compliance[i] == "" || task[i] == "")
                        {
                            throw new Exception();
                        }
                        else
                        {
                            string query = "INSERT INTO schactivitygrade(stdid,campusid,classid,sectionid,teacherid,recordno,sports,assemblypresent,gk,behaviour,discipline,cleanliness,compliance,taskdeadline," +
                        "teachercom,col0,col1,delind,begdate,enddate,upduser,upddate,updtime,dbtimestamp) " +
                        "values ('" + studentId[i] + "','" + campusId + "','" + classId + "','" + sectionId + "','" + user_id + "','" + (record + 1) + "','" + status[i] + "','" + assignppt[i] + "','" +
                        gk[i] + "','" + behave[i] + "','" + discp[i] + "','" + clean[i] + "','" + compliance[i] + "','" + task[i] + "','" + teachercom[i] + "', ' ', '"
                        + " ',' ','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("yyyy-MM-dd") + "',' ', ' ', ' ', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                            cmd.CommandText = query;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    HomeController.popup_status = "Success";
                }
                else if (record == 0 && (tcode == 1 || tcode == 4 || tcode == 5 || tcode == 7))
                {
                    for (int i = 0; i < studentId.Length; i++)
                    {
                        if (status[i] == "" || assignppt[i] == "" || gk[i] == "" || behave[i] == "" || discp[i] == "" || clean[i] == "" || compliance[i] == "" || task[i] == "")
                        {
                            throw new Exception();
                        }
                        else
                        {
                            string query = "INSERT INTO schactivitygrade(stdid,campusid,classid,sectionid,teacherid,recordno,sports,assemblypresent,gk,behaviour,discipline,cleanliness,compliance,taskdeadline," +
                        "teachercom,col0,col1,delind,begdate,enddate,upduser,upddate,updtime,dbtimestamp) " +
                        "values ('" + studentId[i] + "','" + campusId + "','" + classId + "','" + sectionId + "','" + user_id + "','" + (record + 1) + "','" + status[i] + "','" + assignppt[i] + "','" +
                        gk[i] + "','" + behave[i] + "','" + discp[i] + "','" + clean[i] + "','" + compliance[i] + "','" + task[i] + "','" + teachercom[i] + "', ' ', '"
                        + " ',' ','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("yyyy-MM-dd") + "',' ', ' ', ' ', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                            cmd.CommandText = query;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    HomeController.popup_status = "Success";
                }
                else
                    HomeController.popup_status = "Rights";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";
            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        public void update_event_db(string day)
        {
            DatabaeseClass dc = new DatabaeseClass();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DateTime insertdate = dc.convertedinsertdate(DateTime.Now.ToString());
            string query1 = "update calndr set remarks = ' ', dbtimestamp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' where caldate6 = '" + day + "' ";
            using (SqlTransaction trans = con.BeginTransaction())
            {
                SqlCommand command = con.CreateCommand();
                command.Connection = con;
                command.Transaction = trans;
                try
                {
                    command.CommandText = query1;
                    command.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                    con.Close();
                }
            }
        }

        [HttpPost]
        public void insertpublishresult(string campusid, string moduleid, string[] pubstat, string[] classid, string[] sectionid)
        {
            DatabaeseClass dc = new DatabaeseClass();
            DateTime insertdate = dc.convertedinsertdate(DateTime.Now.ToString());
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            SqlTransaction trans;
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            trans = conn.BeginTransaction();
            string[] studentid = null;
            try
            {
                cmd.Connection = conn;
                cmd.Transaction = trans;
                for (int i = 0; i < classid.Length; i++)
                {
                    int max_record = 0;
                    cmd.CommandText = "select isnull(max(recordno), 0) from schrsltpb where campusid = '" + campusid + "' and " +
                                      "subresultype = '" + moduleid + "' and classid = '" + classid[i] + "' and sectionid = '" + sectionid[i] + "'";
                    max_record = Convert.ToInt16(cmd.ExecuteScalar());
                    if (max_record >= 1)
                    {
                        cmd.CommandText = "update schrsltpb set delind = 'X', upduser = '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', " +
                                          "upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', updtime = '" + insertdate.ToString("HH:mm:ss") + "', dbtimestmp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                          "where campusid = '" + campusid + "' and classid = '" + classid[i] + "' and sectionid = '" + sectionid[i] + "' " +
                                          "and subresultype = '" + moduleid + "' and recordno = '" + max_record + "'";
                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommandText = "select count(stdid) from std0710 where campusid = '" + campusid + "' and classid = '" + classid[i] + "' and sectionid = '" + sectionid[i] + "' and delind <> 'X'";
                    int count = Convert.ToInt16(cmd.ExecuteScalar());

                    string notfcase = "";
                    switch (moduleid)
                    {
                        case "1":
                        case "2":
                        case "3":
                        case "4":
                            notfcase = "PRWT";
                            break;

                        case "7":
                        case "8":
                            notfcase = "PRPT";
                            break;

                        case "5":
                            notfcase = "PRMT";
                            break;

                        case "6":
                            //if(classid[i] == "N0")
                            notfcase = "PRFT";
                            break;
                    }

                    cmd.CommandText = "select notftitle from znotificationcase where notfcase = '" + notfcase + "'";
                    string subject = cmd.ExecuteScalar().ToString();

                    cmd.CommandText = "select stdid from std0710 where campusid = '" + campusid + "' and classid = '" + classid[i] + "' and sectionid = '" + sectionid[i] + "' and delind <> 'X'";
                    SqlDataReader sdr;
                    sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        int c = 0;
                        studentid = new string[count];
                        while (sdr.Read())
                        {
                            studentid[c] = sdr["stdid"].ToString();
                            c++;
                        }
                        sdr.Close();
                    }

                    max_record += 1;
                    if (pubstat[i] == "publish")
                    {
                        cmd.CommandText = "insert into schrsltpb (campusid, classid, sectionid, resultype, subresultype, " +
                                      "recordno, delind, begdate, enddate, creuser, credate, cretime, upduser, upddate, " +
                                      "updtime, dbtimestmp) values ('" + campusid + "', '" + classid[i] + "', '" + sectionid[i] + "', '1', '" + moduleid + "', '" + max_record + "', " +
                                      "' ', '" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                                      "'" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', " +
                                      "'" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("HH:mm:ss") + "', " +
                                      "'', '', '', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        cmd.ExecuteNonQuery();
                        string status = popnotification(studentid, System.Web.HttpContext.Current.Session["User_Id"].ToString(), subject, notfcase);
                        if (status == "0")
                        {
                            throw new Exception();
                        }
                    }
                    else
                    {
                        for (int k = 0; k < studentid.Length; k++)
                        {
                            cmd.CommandText = "update inbox set status = 'X' where recip = '" + studentid[k] + "' and notfcase = '" + notfcase + "'";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                trans.Commit();
                HomeController.popup_status = "Success";

            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";
            }
            finally
            {
                trans.Dispose();
                conn.Close();
            }
        }

        public void PPMarksInsertion(string campusId, string classId, string sectionId, string subjectId, string dateId, string[][] griddata, string[] colName)
        {
            string query;

            string menuid = "62700000";
            int tcode = 0;
            foreach (var item in loginModel)
            {
                if (item.menuid == menuid)
                    tcode = Convert.ToInt32(item.tcode);
            }
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            List<SelectListItem> stdid = db.getstudentname(campusId, classId, sectionId);
            string max_record = "select isnull(max(recordno), 0) from schpreresult where stdid='" + stdid[0].Value + "' and campusid = '" + campusId + "' and classid = '" + classId + "' and subjectid = '" + subjectId +
            "' and subresltyp = '6' and resulttype = '2'";
            SqlCommand command = new SqlCommand(max_record, con);
            int record = 0;
            record = Convert.ToInt32(command.ExecuteScalar());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;
                if (record > 0 && (tcode == 2 || tcode == 4 || tcode == 6 || tcode == 7))
                {
                    for (int i = 0; i < griddata.Length; i++)
                    {
                        cmd.CommandText = "update schpreresult set delind = 'X', upduser = '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', upddate = " + insertdate.ToString("yyyy-MM-dd") + ", updtime= '" + insertdate.ToString("HH:mm:ss") + "', dbtimestamp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                          "where recordno = '" + record + "' and stdid='" + stdid[i].Value + "' and classid = '" + classId + "' and subjectid = '" + subjectId +
                                          "' and subresltyp = '6' and resulttype ='2'";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "update schsubchar set delind = 'X', upduser = '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', upddate = " + insertdate.ToString("yyyy-MM-dd") + ", updtime= '" + insertdate.ToString("HH:mm:ss") + "', dbtimestamp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                         "where recordno = '" + record + "' and stdid='" + stdid[i].Value + "' and subjectid = '" + subjectId + "'";
                        cmd.ExecuteNonQuery();
                    }

                    record++;
                    for (int i = 0; i < griddata.Length; i++)
                    {
                        query = "insert into Schpreresult(resulttype,subresltyp,stdid,campusid,classid,sectionid,teacherid,subjectid,recordno,delind,begdate,enddate,upduser,upddate,updtime,dbtimestamp) " +
                                "values ('2','6','" + stdid[i].Value + "','" + campusId + "','" + classId + "','" + sectionId + "','" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "','" +
                                subjectId + "','" + record + "',' ','" + dateId + "','" + dateId + "',' ', ' ', ' ','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();

                        for (int j = 1; j < colName.Length; j++)
                        {
                            query = "insert into Schsubchar(stdid,subjectid,charisticid,charistictxt,grade,recordno,delind,begdate,enddate,upduser,upddate,updtime,dbtimestamp) " +
                                    "values('" + stdid[i].Value + "','" + subjectId + "','" + j + "','" + colName[j] + "','" + griddata[i][j] + "','" +
                                    record + "',' ','" + dateId + "','" + dateId + "',' ',' ',' ','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            cmd.CommandText = query;
                            cmd.ExecuteNonQuery();
                        }

                    }
                    //command.CommandText = "select notftitle from znotificationcase where notfcase = 'SA'";
                    //string subject = command.ExecuteScalar().ToString();
                    //string[] reciparr = new string[stdid.Count];
                    //for (int i =0; i<stdid.Count;i++)
                    //{
                    //    reciparr[i] = stdid[i].Value;
                    //}
                    //popnotification(reciparr, System.Web.HttpContext.Current.Session["User_Id"].ToString(), subject, "");
                    trans.Commit();
                    HomeController.popup_status = "Success";
                }
                else if (record == 0 && (tcode == 1 || tcode == 4 || tcode == 5 || tcode == 7))
                {
                    record++;
                    for (int i = 0; i < griddata.Length; i++)
                    {
                        query = "insert into Schpreresult(resulttype,subresltyp,stdid,campusid,classid,sectionid,teacherid,subjectid,recordno,delind,begdate,enddate,upduser,upddate,updtime, dbtimestamp) " +
                                "values ('2','6','" + stdid[i].Value + "','" + campusId + "','" + classId + "','" + sectionId + "','" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "','" +
                                subjectId + "','" + record + "',' ','" + dateId + "','" + dateId + "',' ', ' ', ' ', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                        for (int j = 1; j < colName.Length; j++)
                        {
                            query = "insert into Schsubchar(stdid,subjectid,charisticid,charistictxt,grade,recordno,delind,begdate,enddate,upduser,upddate,updtime, dbtimestamp) " +
                                    "values('" + stdid[i].Value + "','" + subjectId + "','" + j + "','" + colName[j] + "','" + griddata[i][j] + "','" +
                                    record + "',' ','" + dateId + "','" + dateId + "',' ',' ',' ', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            cmd.CommandText = query;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    HomeController.popup_status = "Success";
                }
                else
                    HomeController.popup_status = "Rights";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";
            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        public void insertweeklylessonplan(DatabaseModel dm, string userrole, string userid)
        {
            DatabaeseClass dc = new DatabaeseClass();

            DateTime begdate = DateTime.Parse(dm.begdate);
            DateTime enddate = DateTime.Parse(dm.enddate);
            DateTime currtime = dc.convertservertopsttimezone(DateTime.Now.ToString());
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            string notfcase = "";
            string[] reciparray;
            string status = "";
            SqlDataReader sdr;
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                if (userrole == "3000")
                {
                    cmd.CommandText = "select isnull(max(recordno),0) from schwkplnpp where campusid = '" + dm.campusid + "' and " +
                                          "classid = '" + dm.classesid + "' and sectionid = '" + dm.sectionid + "' and subjectid = '" + dm.subjectid + "' and " +
                                          "begdate = '" + begdate.ToString("yyyy-MM-dd") + "' and enddate = '" + enddate.ToString("yyyy-MM-dd") + "' and delind <> 'X'";
                    int recordno = Convert.ToInt16(cmd.ExecuteScalar());

                    if (recordno > 0)
                    {
                        cmd.CommandText = "update schwkplnpp set delind = 'X', upduser = '" + System.Web.HttpContext.Current.Session["User_Id"].ToString() + "', " +
                                          "upddate = '" + currtime.ToString("yyyy-MM-dd") + "', updtime = '" + currtime.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                          "where campusid = '" + dm.campusid + "' and classid = '" + dm.classesid + "' " +
                                          "and sectionid = '" + dm.sectionid + "' and subjectid = '" + dm.subjectid + "' and " +
                                          "begdate = '" + begdate.ToString("yyyy-MM-dd") + "' and enddate = '" + enddate.ToString("yyyy-MM-dd") + "' and recordno = '" + recordno + "'";
                        cmd.ExecuteNonQuery();
                    }

                    //insert new record
                    recordno++;
                    cmd.CommandText = "insert into schwkplnpp (empid, campusid, classid, sectionid, subjectid, begdate, enddate, " +
                                      "topic, objective, circletime, initact, devproc, assess, homewrk, recordno, delind, " +
                                      "upduser, upddate, updtime, usr01, usr02, usr03) values ('" + dm.studentid + "', '" + dm.campusid + "', '" + dm.classesid + "', " +
                                      "'" + dm.sectionid + "', '" + dm.subjectid + "', '" + begdate.ToString("yyyy-MM-dd") + "', '" + enddate.ToString("yyyy-MM-dd") + "', " +
                                      "'" + dm.topic + "', '" + dm.objective + "', '" + dm.resource + "', '" + dm.evaluation + "', " +
                                      "'" + dm.writtenwork + "', '" + dm.wrapup + "', '" + dm.evaluationstdid + "', '" + recordno + "', '', '', '', '', '', '', '')";
                    cmd.ExecuteNonQuery();

                    notfcase = "WLP";
                    cmd.CommandText = "select (select count(empid) from schgrpdtl where groupid = '100" + dm.campusid + "' and delind <> 'X') as countemp, empid from schgrpdtl " +
                                    "where groupid = '100" + dm.campusid + "' and delind <> 'X'";
                    int count = 0;
                    string[] recip = new string[count];

                    sdr = cmd.ExecuteReader();
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

                        cmd.CommandText = "select (select notftitle from znotificationcase where notfcase = '" + notfcase + "') as title, sc.classtxt, sc.sectionid, ss.subjecttxt from emp0710 e7 " +
                                            "inner join schcampus sch on sch.campusid = e7.campusid " +
                                            "inner join schclass sc on e7.campusid = sc.campusid and sc.classid = e7.classid and e7.sectionid = sc.sectionid " +
                                            "inner join schsubject ss on ss.subjectid = e7.subjectid and e7.classid = ss.classid " +
                                            "where e7.empid = '" + userid + "'  and e7.delind <> 'X' and sch.campusid = '" + dm.campusid + "' and " +
                                            "sc.classid = '" + dm.classesid + "' and sc.sectionid = '" + dm.sectionid + "' and ss.subjectid = '" + dm.subjectid + "'";
                        sdr = cmd.ExecuteReader();
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

                        status = popnotification(recip, userid, text, notfcase);
                        if (status != "1")
                        {
                            throw new Exception();
                        }
                    }
                    trans.Commit();

                }
                else if (userrole == "1000" || userrole == "2000")
                {
                    cmd.CommandText = "select isnull(max(recordno),0) from schlessonplancom where " +
                                       "begdate = '" + begdate.ToString("yyyy-MM-dd") + "' and enddate = '" + enddate.ToString("yyyy-MM-dd") + "' " +
                                       "and campusid = '" + dm.campusid + "' and classid = '" + dm.classesid + "' " +
                                       "and sectionid = '" + dm.sectionid + "' and subjectid = '" + dm.subjectid + "'";
                    // and teacherid = '" + dm.teachername + "'
                    int record = Convert.ToInt16(cmd.ExecuteScalar());
                    record += 1;
                    cmd.CommandText = "insert into schlessonplancom (empid,teacherid,begdate,enddate,campusid,classid,sectionid,subjectid,princplcomnt,recordno) " +
                                        "values ('" + userid + "','" + dm.studentid + "', '" + begdate.ToString("yyyy-MM-dd") + "', '" + enddate.ToString("yyyy-MM-dd") + "', " +
                                        "'" + dm.campusid + "', '" + dm.classesid + "', '" + dm.sectionid + "', " +
                                        "'" + dm.subjectid + "', @comment, '" + record + "')";
                    cmd.Parameters.AddWithValue("@comment", dm.princ_comments_new);
                    cmd.ExecuteNonQuery();

                    notfcase = "WLPR";

                    string[] arr = new string[1];
                    arr[0] = dm.studentid;

                    cmd.CommandText = "select (select notftitle from znotificationcase where notfcase = '" + notfcase + "') as title, sc.classtxt, sc.sectionid, ss.subjecttxt from emp0710 e7 " +
                                            "inner join schcampus sch on sch.campusid = e7.campusid " +
                                            "inner join schclass sc on e7.campusid = sc.campusid and sc.classid = e7.classid and e7.sectionid = sc.sectionid " +
                                            "inner join schsubject ss on ss.subjectid = e7.subjectid and e7.classid = ss.classid " +
                                            "where e7.empid = '" + dm.studentid + "'  and e7.delind <> 'X' and sch.campusid = '" + dm.campusid + "' and " +
                                            "sc.classid = '" + dm.classesid + "' and sc.sectionid = '" + dm.sectionid + "' and ss.subjectid = '" + dm.subjectid + "'";
                    sdr = cmd.ExecuteReader();
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

                    status = popnotification(arr, userid, text, notfcase);
                    if (status != "1")
                    {
                        throw new Exception();
                    }
                    trans.Commit();
                }

                //string status = popnotification(notfcase);
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";
            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        /**************************LOAN REQUEST*****************************************************/
        public string ESSLoanInsert(ESSModel model, string user_id)
        {
            int recordno = 1;
            string cases = "LNR";
            string apprstat = "04";
            string msg = "Success";
            DateTime date = Convert.ToDateTime(model.repaydate);
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<string> recodlist = new List<string>();
            string menuid = "65800000"; //3
            int tcode = 0;

            int recordno78 = 1;
            foreach (var item in loginModel)
            {
                if (item.menuid == menuid)
                    tcode = Convert.ToInt32(item.tcode);
            }
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;
                cmd.CommandText = "select isnull(max(recordno), 0) as recordno, reqstat from emp0377 " +
                "where empid = '" + user_id + "' " +
                "group by reqstat";
                SqlDataReader sdr;
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        recordno = (Convert.ToInt32(sdr["recordno"]) + 1);
                        string reqstat = sdr["reqstat"].ToString().Trim();
                        switch (reqstat)
                        {
                            case "20":
                                msg = "Application is already requested";
                                break;
                            case "40":// Completed loan request applying for new loan, Record number will be +1
                                sdr.Close();

                                cmd.CommandText = "select TOP(1) enddate from emp0377 " +
                                "where empid = '" + user_id + "' and subpagtype = '" + model.loanid + "' and loanreq = '10' and " +
                                "(reqstat = '01' or reqstat = '02') and delind <> 'X' and upduser <> '' " +
                                "order by recordno desc";

                                DateTime enddate = Convert.ToDateTime(cmd.ExecuteScalar());
                                if (enddate < insertdate)
                                {
                                    cmd.CommandText = "select distinct e430.reqtype,e430.recod1,e430.recod2,e430.recod3, e430.apprvl1, e430.apprvl2, e430.apprvl3, e430.apprvl4 " +
                                    ", e430.apprvl5, e430.apprvl6, e430.apprvl7, e430.apprvl8, e430.apprvl9, e430.apprvl10 " +
                                    "from emp0430 e430 where empid = '" + model.empid + "' and reqtype = '10'";
                                    sdr = cmd.ExecuteReader();
                                    if (sdr.HasRows)
                                    {
                                        while (sdr.Read())
                                        {
                                            recodlist.Add(sdr["reqtype"].ToString());

                                            for (int j = 1; j <= 3; j++)
                                            {
                                                string s = "recod" + j;
                                                if (sdr[s].ToString().Trim() != "")
                                                {
                                                    recodlist.Add(sdr[s].ToString());
                                                }
                                            }
                                            // If no recommenders then
                                            if (recodlist.Count == 1)
                                            {
                                                for (int j = 1; j <= 10; j++)
                                                {
                                                    string s = "apprvl" + j;
                                                    if (sdr[s].ToString().Trim() != "")
                                                    {
                                                        recodlist.Add(sdr[s].ToString());
                                                        apprstat = "05";
                                                    }
                                                }
                                            }
                                        }
                                        sdr.Close();
                                        cmd.CommandText = "INSERT INTO emp0377(empid,begdate,enddate,subpagtype,recordno,delind," +
                                        "creuser,credate,cretime,upduser,upddate,updtime," +
                                        "loantype,loanreq,paymamt,paymcurr,payinstal,loanperd,paymdate," +
                                        "collamount,reqstat,apprtype,loancommt) " +
                                        "VALUES('" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', '" + model.loanid + "', '" + recordno + "', ' '," +
                                        "'" + model.empid + "', '" + model.begdate + "', '" + model.begdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "'," +
                                        "'" + model.loanid + "', '" + recodlist[0] + "', '" + model.loanamt + "', 'PKR', '" + model.lontintl + "', '" + model.payperiod + "', '" + lastDayOfMonth + "', ' '," +
                                        "'20', ' ', ' ')";
                                        cmd.ExecuteNonQuery();

                                        for (int i = 1; i < recodlist.Count; i++)
                                        {
                                            cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                                              "creuser,credate,cretime,upduser,upddate,updtime,reqtype,requestno,appractid, " +
                                                              "approver,apprdate,apprstat,apprcoment) " +
                                                              "VALUES('" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', '" + model.loanid + "', '" + recordno78 + "', ' '," +
                                                              "'" + model.empid + "', '" + model.begdate + "', '" + model.lastdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "'," +
                                                              "'" + recodlist[0] + "', '" + recordno + "', '10', '" + recodlist[i] + "', '" + model.begdate + "', '" + apprstat + "', ' ')";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                                            string msgid = cmd.ExecuteScalar().ToString();
                                            cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                                     "values ('" + msgid + "','1', '" + recodlist[i] + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                                            cmd.ExecuteNonQuery();
                                            recordno78++;
                                        }
                                    }
                                }
                                else
                                    msg = "Application Request Failed! Your previous application is not completed yet.";
                                break;

                            case "03":// Rejected loan applying for new Record number will be  +1
                                sdr.Close();
                                cmd.CommandText = "select distinct e430.reqtype,e430.recod1,e430.recod2,e430.recod3, e430.apprvl1, e430.apprvl2, e430.apprvl3, e430.apprvl4 " +
                                    ", e430.apprvl5, e430.apprvl6, e430.apprvl7, e430.apprvl8, e430.apprvl9, e430.apprvl10 " +
                                    "from emp0430 e430 where empid = '" + model.empid + "' and reqtype = '10'";
                                sdr = cmd.ExecuteReader();
                                if (sdr.HasRows)
                                {
                                    while (sdr.Read())
                                    {
                                        recodlist.Add(sdr["reqtype"].ToString());

                                        for (int j = 1; j <= 3; j++)
                                        {
                                            string s = "recod" + j;
                                            if (sdr[s].ToString().Trim() != "")
                                            {
                                                recodlist.Add(sdr[s].ToString());
                                            }
                                        }
                                        // If no recommenders then
                                        if (recodlist.Count == 1)
                                        {
                                            for (int j = 1; j <= 10; j++)
                                            {
                                                string s = "apprvl" + j;
                                                if (sdr[s].ToString().Trim() != "")
                                                {
                                                    recodlist.Add(sdr[s].ToString());
                                                    apprstat = "05";
                                                }
                                            }
                                        }
                                    }
                                    sdr.Close();
                                    cmd.CommandText = "INSERT INTO emp0377(empid,begdate,enddate,subpagtype,recordno,delind," +
                                    "creuser,credate,cretime,upduser,upddate,updtime," +
                                    "loantype,loanreq,paymamt,paymcurr,payinstal,loanperd,paymdate," +
                                    "collamount,reqstat,apprtype,loancommt) " +
                                    "VALUES('" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', '" + model.loanid + "', '" + recordno + "', ' '," +
                                    "'" + model.empid + "', '" + model.begdate + "', '" + model.begdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "'," +
                                    "'" + model.loanid + "', '" + recodlist[0] + "', '" + model.loanamt + "', 'PKR', '" + model.lontintl + "', '" + model.payperiod + "', '" + lastDayOfMonth + "', ' '," +
                                    "'20', ' ', ' ')";
                                    cmd.ExecuteNonQuery();

                                    for (int i = 1; i < recodlist.Count; i++)
                                    {
                                        cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                                          "creuser,credate,cretime,upduser,upddate,updtime,reqtype,requestno,appractid, " +
                                                          "approver,apprdate,apprstat,apprcoment) " +
                                                          "VALUES('" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', '" + model.loanid + "', '" + recordno78 + "', ' '," +
                                                          "'" + model.empid + "', '" + model.begdate + "', '" + model.lastdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "'," +
                                                          "'" + recodlist[0] + "', '" + recordno + "', '10', '" + recodlist[i] + "', '" + model.begdate + "', '" + apprstat + "', ' ')";
                                        cmd.ExecuteNonQuery();
                                        cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                                        string msgid = cmd.ExecuteScalar().ToString();
                                        cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                                 "values ('" + msgid + "','1', '" + recodlist[i] + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                                        cmd.ExecuteNonQuery();
                                        recordno78++;
                                    }
                                }
                                break;
                        }
                        break;
                    }
                }
                else // New Record
                {
                    sdr.Close();
                    cmd.CommandText = "select distinct e430.reqtype,e430.recod1,e430.recod2,e430.recod3, e430.apprvl1, e430.apprvl2, e430.apprvl3, e430.apprvl4 " +
                                    ", e430.apprvl5, e430.apprvl6, e430.apprvl7, e430.apprvl8, e430.apprvl9, e430.apprvl10 " +
                                    "from emp0430 e430 where empid = '" + model.empid + "' and reqtype = '10'";
                    sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        // Has completed loan or didn't pay the previous loan right now
                        while (sdr.Read())
                        {
                            recodlist.Add(sdr["reqtype"].ToString());

                            for (int j = 1; j <= 3; j++)
                            {
                                string s = "recod" + j;
                                if (sdr[s].ToString().Trim() != "")
                                {
                                    recodlist.Add(sdr[s].ToString());
                                }
                            }
                            // If no recommenders then
                            if (recodlist.Count == 1)
                            {
                                for (int j = 1; j <= 10; j++)
                                {
                                    string s = "apprvl" + j;
                                    if (sdr[s].ToString().Trim() != "")
                                    {
                                        recodlist.Add(sdr[s].ToString());
                                        apprstat = "05";
                                    }
                                }
                            }
                        }
                        sdr.Close();
                        cmd.CommandText = "INSERT INTO emp0377(empid,begdate,enddate,subpagtype,recordno,delind," +
                        "creuser,credate,cretime,upduser,upddate,updtime," +
                        "loantype,loanreq,paymamt,paymcurr,payinstal,loanperd,paymdate," +
                        "collamount,reqstat,apprtype,loancommt) " +
                        "VALUES('" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', '" + model.loanid + "', '1', ' '," +
                        "'" + model.empid + "', '" + model.begdate + "', '" + model.begdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "'," +
                        "'" + model.loanid + "', '" + recodlist[0] + "', '" + model.loanamt + "', 'PKR', '" + model.lontintl + "', '" + model.payperiod + "', '" + lastDayOfMonth + "', ' '," +
                        "'20', ' ', ' ')";
                        cmd.ExecuteNonQuery();
                        for (int i = 1; i < recodlist.Count; i++)
                        {
                            cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                              "creuser,credate,cretime,upduser,upddate,updtime,reqtype,requestno,appractid, " +
                                              "approver,apprdate,apprstat,apprcoment) " +
                                              "VALUES('" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', '" + model.loanid + "', '" + recordno78 + "', ' '," +
                                              "'" + model.empid + "', '" + model.begdate + "', '" + model.lastdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "'," +
                                              "'" + recodlist[0] + "', '1', '10', '" + recodlist[i] + "', '" + model.begdate + "', '" + apprstat + "', ' ')";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                            string msgid = cmd.ExecuteScalar().ToString();
                            cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                     "values ('" + msgid + "','1', '" + recodlist[i] + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                            cmd.ExecuteNonQuery();
                            recordno78++;
                        }
                        recordno78 = 1;
                    }
                }
                sdr.Close();
                trans.Commit();

            }
            catch (Exception ex)
            {
                trans.Rollback();
                msg = "Found some error! Please try again.";
            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
            return msg;
        }

        public void rejectRecodLoan(string empid, string reqno, string subpagtype, string comment, string loanamt, string user_id)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                string cases = "LNRJ";
                cmd.Connection = con;
                cmd.Transaction = trans;

                //emp0377
                cmd.CommandText = "update emp0377 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', loancommt = '" + comment + "', reqstat = '03' " +
                "where empid = '" + empid + "' and recordno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and paymamt = '" + loanamt + "' and loanreq = '10' and upduser = ''";
                cmd.ExecuteNonQuery();

                //emp0378
                cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + comment + "', apprstat = '03' " +
                "where empid = '" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and reqtype = '10' and upduser = ''";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                         "values ('" + msgid + "','1', '" + empid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                cmd.ExecuteNonQuery();
                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        public void loanrecodTo(ESSModel model, string user_id)
        {
            string cases = "LNAP";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                //emp0378 Update recomend
                cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + model.coments + "', apprstat = '01' " +
                "where empid = '" + model.empid + "' and requestno = '" + model.recordno + "' and subpagtype = '" + model.loanid + "' and reqtype = '10' and upduser = ''";
                cmd.ExecuteNonQuery();

                //emp0378 new approval
                cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                   "creuser,credate,cretime,upduser,upddate,updtime, " +
                                   "reqtype,requestno,appractid, approver,apprdate,apprstat,apprcoment) " +
                                   "VALUES('" + model.empid + "','" + model.begdate + "','" + model.lastdate + "','" + model.loanid + "','4', ' '," +
                                   "'" + user_id + "','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "',' ','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', " +
                                   "'10','" + model.recordno + "','20','" + model.recodid + "','" + model.begdate + "','05',' ')";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                         "values ('" + msgid + "','1', '" + model.recodid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                cmd.ExecuteNonQuery();
                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        //Recommender Accepting Processing Request
        public void acceptLoanByRecomder(string empid, string reqno, string subpagtype, string startdate, string enddate, string apprrecodtypid, string comment, string reqtype, string user_id)
        {
            string cases = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;
                cmd.CommandText = "select count(apprstat) as apprcount from emp0378 " +
                "where empid = '" + empid + "' and subpagtype = '" + subpagtype + "' " +
                "and appractid = '20' and delind <> 'X' " +
                "and upduser = '' and reqtype = '" + reqtype + "' " +
                "and requestno = '" + reqno + "' and apprstat = '05'";
                cmd.ExecuteNonQuery();
                SqlDataReader sdr;
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        int apprcount = Convert.ToInt32(sdr["apprcount"].ToString());

                        if (apprcount == 1) //Loan accepted and going to recomender (particular person)
                        {
                            sdr.Close();
                            cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                            "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + comment + "', apprstat = '01' " +
                            "where empid = '" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and reqtype = '" + reqtype + "' and upduser = '' " +
                            "and approver = '" + user_id + "' and apprstat = '05' and delind <> 'X'";
                            cmd.ExecuteNonQuery();

                            //Adding Other Approvers

                            List<String> apprlist = new List<string>();
                            cmd.CommandText = "select apprvl1, apprvl2, apprvl3, apprvl4, apprvl5, " +
                            "apprvl6, apprvl7, apprvl8, apprvl9, apprvl10 from emp0430 where empid = '" + empid + "' and reqtype = '" + reqtype + "'";
                            sdr = cmd.ExecuteReader();
                            if (sdr.HasRows)
                            {
                                while (sdr.Read())
                                {
                                    for (int j = 1; j <= 10; j++)
                                    {
                                        string s = "apprvl" + j;
                                        if (sdr[s].ToString().Trim() != "" && sdr[s].ToString().Trim() != user_id.ToString().Trim())
                                        {
                                            apprlist.Add(sdr[s].ToString());
                                        }
                                    }
                                }
                                sdr.Close();
                                for (int i = 0; i < apprlist.Count; i++)
                                {
                                    cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                                      "creuser,credate,cretime,upduser,upddate,updtime,reqtype,requestno,appractid, " +
                                                      "approver,apprdate,apprstat,apprcoment) " +
                                                      "VALUES('" + empid + "', '" + startdate + "', '" + enddate + "', '" + subpagtype + "', (select isnull(max(recordno), 0)+1 as recordno from emp0378 " +
                                                      "where empid ='" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' " +
                                                      " and reqtype = '10' and appractid = '20'), ' '," +
                                                      "'" + user_id + "', '" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("HH:mm:ss") + "', ' ', '" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("HH:mm:ss") + "'," +
                                                      "'10', '" + reqno + "', '20', '" + apprlist[i] + "', '" + insertdate.ToString("yyyy-MM-dd") + "', '05', ' ')";
                                    cmd.ExecuteNonQuery();
                                    cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                                    string msgid = cmd.ExecuteScalar().ToString();
                                    cases = "LNAR";
                                    cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                             "values ('" + msgid + "','1', '" + apprlist[i] + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                                    cmd.ExecuteNonQuery();
                                }
                            }

                        }
                        else                //Individual Loan Acceptance
                        {
                            sdr.Close();
                            cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                            "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + comment + "', apprstat = '01' " +
                            "where empid = '" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and reqtype = '" + reqtype + "' and upduser = '' and delind <> 'X' " +
                            "and approver = '" + user_id + "' and apprstat = '05'";
                            cmd.ExecuteNonQuery();

                            if (apprcount == 0) //Loan Approved By all approvers
                            {
                                cmd.CommandText = "update emp0377 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', loancommt = 'Approved', reqstat = '40' " +
                                "where emp = '" + empid + "' and subpagtype = '" + subpagtype + "' and recordno = '" + reqno + "' and loanreq = '" + reqtype + "' and reqstat = '20' and delind <> 'X'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                                string msgid = cmd.ExecuteScalar().ToString();
                                cases = "LNA";
                                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                         "values ('" + msgid + "','1', '" + empid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        break;
                    }
                }

                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        public void rejectApprLoan(string empid, string reqno, string subpagtype, string comment, string loanamt, string user_id)
        {
            string cases = "LNRJ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                //emp0377
                cmd.CommandText = "update emp0377 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', loancommt = '" + comment + "', reqstat = '03' " +
                "where empid = '" + empid + "' and recordno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and paymamt = '" + loanamt + "' and loanreq = '10' and upduser = '' and delind <> 'X'";
                cmd.ExecuteNonQuery();

                //emp0378
                cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                            "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + comment + "', apprstat = '03' " +
                            "where empid = '" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and reqtype = '10' and upduser = '' " +
                            "and approver = '" + user_id + "' and apprstat = '05' and delind <> 'X'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                         "values ('" + msgid + "','1', '" + empid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                cmd.ExecuteNonQuery();
                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }


        public void loanAcceptedAsPerPolicy(ESSModel model, string user_id)
        {
            string cases = "LNAPP";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                //emp0377
                cmd.CommandText = "update emp0377 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', loancommt = '" + model.coments + "', reqstat = '02' " +
                "where empid = '" + model.empid + "' and recordno = '" + model.recordno + "' and subpagtype = '" + model.loanid + "' and paymamt = '" + model.loanamt + "' and loanreq = '10' and upduser = '' and delind <> 'X'";
                cmd.ExecuteNonQuery();

                //emp0378
                cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                            "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + model.coments + "', apprstat = '02' " +
                            "where empid = '" + model.empid + "' and requestno = '" + model.recordno + "' and subpagtype = '" + model.loanid + "' and reqtype = '10' and upduser = '' " +
                            "and approver = '" + user_id + "' and apprstat = '05' and delind <> 'X'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                         "values ('" + msgid + "','1', '" + model.empid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                cmd.ExecuteNonQuery();
                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }


        public void apprLoanRecomended(string empid, string reqno, string subpagtype, string startdate, string enddate, string apprrecodtypid, string comment, string reqtype, string user_id)
        {
            string cases = "LNRC";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                //emp0378 Update recomend
                cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + comment + "', apprstat = '01' " +
                "where empid = '" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and reqtype = '" + reqtype + "' and upduser = ''";
                cmd.ExecuteNonQuery();

                //emp0378 new approval
                cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                   "creuser,credate,cretime,upduser,upddate,updtime, " +
                                   "reqtype,requestno,appractid, approver,apprdate,apprstat,apprcoment) " +
                                   "VALUES('" + empid + "','" + startdate + "','" + enddate + "','" + subpagtype + "','5', ' '," +
                                   "'" + user_id + "','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "',' ','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', " +
                                   "'10','" + reqno + "','20','" + apprrecodtypid + "','" + startdate + "','05',' ')";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                         "values ('" + msgid + "','1', '" + apprrecodtypid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                cmd.ExecuteNonQuery();
                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }
        /**************************LOAN REQUEST*****************************************************/

        /**************************LEAVE REQUEST*****************************************************/
        public string ESSLeaveInsert(ESSModel model, string user_id)
        {
            string cases = "LVR";
            int recordno = 1;
            string msg = "Success";
            DateTime date = Convert.ToDateTime(model.repaydate);
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<string> recodlist = new List<string>();
            string menuid = "65800000"; //3
            int tcode = 0;

            int recordno78 = 1;
            foreach (var item in loginModel)
            {
                if (item.menuid == menuid)
                    tcode = Convert.ToInt32(item.tcode);
            }
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;
                cmd.CommandText = " select Top 1 recordno, reqstat from emp0277 " +
                "where empid = '" + user_id + "' order by recordno desc ";
                SqlDataReader sdr;
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        recordno = (Convert.ToInt32(sdr["recordno"]) + 1);
                        string reqstat = sdr["reqstat"].ToString().Trim();
                        switch (reqstat)
                        {
                            case "20":
                                msg = "Application is already requested";
                                break;
                            case "40":// Completed loan request applying for new loan, Record number will be +1
                                sdr.Close();

                                cmd.CommandText = "select TOP(1) enddate from emp0277 " +
                                "where empid = '" + user_id + "' and subpagtype = '" + model.loanid + "' and " +
                                "(reqstat = '01' or reqstat = '02') and delind <> 'X' and upduser <> '' " +
                                "order by recordno desc";

                                DateTime enddate = Convert.ToDateTime(cmd.ExecuteScalar());
                                if (enddate < insertdate)
                                {
                                    Boolean recodind = true;
                                    cmd.CommandText = "select distinct e430.reqtype,e430.recod1,e430.recod2,e430.recod3, e430.apprvl1, e430.apprvl2, e430.apprvl3, e430.apprvl4 " +
                                    ", e430.apprvl5, e430.apprvl6, e430.apprvl7, e430.apprvl8, e430.apprvl9, e430.apprvl10 " +
                                    "from emp0430 e430 where empid = '" + model.empid + "' and reqtype = '20'";
                                    sdr = cmd.ExecuteReader();
                                    if (sdr.HasRows)
                                    {
                                        while (sdr.Read())
                                        {
                                            recodlist.Add(sdr["reqtype"].ToString());

                                            for (int j = 1; j <= 3; j++)
                                            {
                                                string s = "recod" + j;
                                                if (sdr[s].ToString().Trim() != "")
                                                {
                                                    recodlist.Add(sdr[s].ToString());
                                                }
                                            }
                                            // If no recommenders then
                                            if (recodlist.Count == 1)
                                            {
                                                for (int j = 1; j <= 10; j++)
                                                {
                                                    string s = "apprvl" + j;
                                                    if (sdr[s].ToString().Trim() != "")
                                                    {
                                                        recodlist.Add(sdr[s].ToString());
                                                    }
                                                }
                                            }
                                        }
                                        sdr.Close();
                                        cmd.CommandText = "INSERT INTO emp0277(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                                              "creuser,credate,cretime,upduser,upddate,updtime, " +
                                                              "lvtype,halfdayind,lvdays,caldays,reqstat,apprtype,comment,remarks) " +
                                                              "VALUES('" + user_id + "','" + model.begdate + "','" + model.lastdate + "','" + model.loanid + "','" + recordno + "', ' ', " +
                                                              "'" + model.empid + "', '" + model.begdate + "', '" + model.lastdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "', " +
                                                              "'" + model.loanid + "', ' ', '" + model.totdays + "','" + model.totdays + "','20',' ','" + model.reason + "',' ')";
                                        cmd.ExecuteNonQuery();

                                        for (int i = 1; i < recodlist.Count; i++)
                                        {
                                            cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                                              "creuser,credate,cretime,upduser,upddate,updtime,reqtype,requestno,appractid, " +
                                                              "approver,apprdate,apprstat,apprcoment) " +
                                                              "VALUES('" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', '" + model.loanid + "', '" + recordno78 + "', ' '," +
                                                              "'" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "'," +
                                                              "'" + recodlist[0] + "', '" + recordno + "', '10', '" + recodlist[i] + "', '" + model.begdate + "', '04', ' ')";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                                            string msgid = cmd.ExecuteScalar().ToString();
                                            cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                                     "values ('" + msgid + "','1', '" + recodlist[i] + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                                            cmd.ExecuteNonQuery();
                                            recordno78++;
                                        }
                                    }
                                }
                                else
                                    msg = "Application Request Failed! Your previous application is not completed yet.";
                                break;

                            case "03":// Rejected loan applying for new Record number will be  +1
                                sdr.Close();
                                cmd.CommandText = "select distinct e430.reqtype,e430.recod1,e430.recod2,e430.recod3, e430.apprvl1, e430.apprvl2, e430.apprvl3, e430.apprvl4 " +
                                    ", e430.apprvl5, e430.apprvl6, e430.apprvl7, e430.apprvl8, e430.apprvl9, e430.apprvl10 " +
                                    "from emp0430 e430 where empid = '" + model.empid + "' and reqtype = '20'";
                                sdr = cmd.ExecuteReader();
                                if (sdr.HasRows)
                                {
                                    while (sdr.Read())
                                    {
                                        recodlist.Add(sdr["reqtype"].ToString());

                                        for (int j = 1; j <= 3; j++)
                                        {
                                            string s = "recod" + j;
                                            if (sdr[s].ToString().Trim() != "")
                                            {
                                                recodlist.Add(sdr[s].ToString());
                                            }
                                        }
                                        // If no recommenders then
                                        if (recodlist.Count == 1)
                                        {
                                            for (int j = 1; j <= 10; j++)
                                            {
                                                string s = "apprvl" + j;
                                                if (sdr[s].ToString().Trim() != "")
                                                {
                                                    recodlist.Add(sdr[s].ToString());
                                                }
                                            }
                                        }
                                    }
                                    sdr.Close();
                                    cmd.CommandText = "INSERT INTO emp0277(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                                              "creuser,credate,cretime,upduser,upddate,updtime, " +
                                                              "lvtype,halfdayind,lvdays,caldays,reqstat,apprtype,comment,remarks) " +
                                                              "VALUES('" + user_id + "','" + model.begdate + "','" + model.lastdate + "','" + model.loanid + "','" + recordno + "', ' ', " +
                                                              "'" + model.empid + "', '" + model.begdate + "', '" + model.lastdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "', " +
                                                              "'" + model.loanid + "', ' ', '" + model.totdays + "','" + model.totdays + "','20',' ','" + model.reason + "',' ')";
                                    cmd.ExecuteNonQuery();

                                    for (int i = 1; i < recodlist.Count; i++)
                                    {
                                        cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                                          "creuser,credate,cretime,upduser,upddate,updtime,reqtype,requestno,appractid, " +
                                                          "approver,apprdate,apprstat,apprcoment) " +
                                                          "VALUES('" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', '" + model.loanid + "', '" + recordno78 + "', ' '," +
                                                          "'" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "'," +
                                                          "'" + recodlist[0] + "', '" + recordno + "', '10', '" + recodlist[i] + "', '" + model.begdate + "', '04', ' ')";
                                        cmd.ExecuteNonQuery();
                                        cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                                        string msgid = cmd.ExecuteScalar().ToString();
                                        cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                                     "values ('" + msgid + "','1', '" + recodlist[i] + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                                        cmd.ExecuteNonQuery();
                                        recordno78++;
                                    }

                                }
                                break;
                        }
                        break;
                    }
                }
                else // New Record
                {
                    sdr.Close();
                    cmd.CommandText = "select distinct e430.reqtype,e430.recod1,e430.recod2,e430.recod3, e430.apprvl1, e430.apprvl2, e430.apprvl3, e430.apprvl4 " +
                                    ", e430.apprvl5, e430.apprvl6, e430.apprvl7, e430.apprvl8, e430.apprvl9, e430.apprvl10 " +
                                    "from emp0430 e430 where empid = '" + model.empid + "' and reqtype = '20'";
                    sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        // Has completed loan or didn't pay the previous loan right now
                        while (sdr.Read())
                        {
                            recodlist.Add(sdr["reqtype"].ToString());

                            for (int j = 1; j <= 3; j++)
                            {
                                string s = "recod" + j;
                                if (sdr[s].ToString().Trim() != "")
                                {
                                    recodlist.Add(sdr[s].ToString());
                                }
                            }
                            // If no recommenders then
                            if (recodlist.Count == 1)
                            {
                                for (int j = 1; j <= 10; j++)
                                {
                                    string s = "apprvl" + j;
                                    if (sdr[s].ToString().Trim() != "")
                                    {
                                        recodlist.Add(sdr[s].ToString());
                                    }
                                }
                            }
                        }
                        sdr.Close();
                        cmd.CommandText = "INSERT INTO emp0277(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                                              "creuser,credate,cretime,upduser,upddate,updtime, " +
                                                              "lvtype,halfdayind,lvdays,caldays,reqstat,apprtype,comment,remarks) " +
                                                              "VALUES('" + user_id + "','" + model.begdate + "','" + model.lastdate + "','" + model.loanid + "','" + recordno + "', ' ', " +
                                                              "'" + model.empid + "', '" + model.begdate + "', '" + model.lastdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "', " +
                                                              "'" + model.loanid + "', ' ', '" + model.totdays + "','" + model.totdays + "','20',' ','" + model.reason + "',' ')";
                        cmd.ExecuteNonQuery();

                        for (int i = 1; i < recodlist.Count; i++)
                        {
                            cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                              "creuser,credate,cretime,upduser,upddate,updtime,reqtype,requestno,appractid, " +
                                              "approver,apprdate,apprstat,apprcoment) " +
                                              "VALUES('" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', '" + model.loanid + "', '" + recordno78 + "', ' '," +
                                              "'" + user_id + "', '" + model.begdate + "', '" + model.lastdate + "', ' ', '" + model.begdate + "', '" + model.begdate + "'," +
                                              "'" + recodlist[0] + "', '1', '10', '" + recodlist[i] + "', '" + model.begdate + "', '04', ' ')";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                            string msgid = cmd.ExecuteScalar().ToString();
                            cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                                     "values ('" + msgid + "','1', '" + recodlist[i] + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                            cmd.ExecuteNonQuery();
                            recordno78++;
                        }
                        recordno78 = 1;
                    }
                }
                sdr.Close();
                trans.Commit();

            }
            catch (Exception ex)
            {
                trans.Rollback();
                msg = "Found some error! Please try again.";
            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
            return msg;
        }

        public void rejectRecodLeave(string empid, string reqno, string subpagtype, string comment, string leavdays, string user_id)
        {
            string cases = "LVRJ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                //emp0277
                cmd.CommandText = "update emp0277 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', remarks = '" + comment + "', reqstat = '03' " +
                "where empid = '" + empid + "' and recordno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and lvdays = '" + leavdays + "' and upduser = ''";
                cmd.ExecuteNonQuery();

                //emp0378
                cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + comment + "', apprstat = '03' " +
                "where empid = '" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and reqtype = '20' and upduser = ''";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                         "values ('" + msgid + "','1', '" + empid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                cmd.ExecuteNonQuery();
                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        public void leaverecodTo(ESSModel model, string user_id)
        {
            string cases = "LVAP";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                //emp0378 Update recomend
                cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + model.coments + "', apprstat = '01' " +
                "where empid = '" + model.empid + "' and requestno = '" + model.recordno + "' and subpagtype = '" + model.loanid + "' and reqtype = '20' and upduser = ''";
                cmd.ExecuteNonQuery();

                //emp0378 new approval
                cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                   "creuser,credate,cretime,upduser,upddate,updtime, " +
                                   "reqtype,requestno,appractid, approver,apprdate,apprstat,apprcoment) " +
                                   "VALUES('" + model.empid + "','" + model.begdate + "','" + model.lastdate + "','" + model.loanid + "','4', ' '," +
                                   "'" + user_id + "','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "',' ','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', " +
                                   "'20','" + model.recordno + "','20','" + model.recodid + "','" + model.begdate + "','05',' ')";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                         "values ('" + msgid + "','1', '" + model.recodid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                cmd.ExecuteNonQuery();
                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        //Recommender Accepting Processing Request
        public void acceptLeaveByRecomder(string empid, string reqno, string subpagtype, string startdate, string enddate, string apprrecodtypid, string comment, string reqtype, string user_id)
        {
            string cases = "";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;
                cmd.CommandText = "select count(apprstat) as apprcount from emp0378 " +
                "where empid = '" + empid + "' and subpagtype = '" + subpagtype + "' " +
                "and appractid = '20' and delind <> 'X' " +
                "and upduser = '' and reqtype = '" + reqtype + "' " +
                "and requestno = '" + reqno + "' and apprstat = '05'";
                cmd.ExecuteNonQuery();
                SqlDataReader sdr;
                sdr = cmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        int apprcount = Convert.ToInt32(sdr["apprcount"].ToString());

                        if (apprcount == 1) //Loan accepted and going to recomender (particular person)
                        {
                            sdr.Close();
                            cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                            "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + comment + "', apprstat = '01' " +
                            "where empid = '" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and reqtype = '" + reqtype + "' and upduser = '' " +
                            "and approver = '" + user_id + "' and apprstat = '05' and delind <> 'X'";
                            cmd.ExecuteNonQuery();

                            //Adding Other Approvers

                            List<String> apprlist = new List<string>();
                            cmd.CommandText = "select apprvl1, apprvl2, apprvl3, apprvl4, apprvl5, " +
                            "apprvl6, apprvl7, apprvl8, apprvl9, apprvl10 from emp0430 where empid = '" + empid + "' and reqtype = '" + reqtype + "'";
                            sdr = cmd.ExecuteReader();
                            if (sdr.HasRows)
                            {
                                while (sdr.Read())
                                {
                                    for (int j = 1; j <= 10; j++)
                                    {
                                        string s = "apprvl" + j;
                                        if (sdr[s].ToString().Trim() != "" && sdr[s].ToString().Trim() != user_id.ToString().Trim())
                                        {
                                            apprlist.Add(sdr[s].ToString());
                                        }
                                    }
                                }
                                sdr.Close();
                                for (int i = 0; i < apprlist.Count; i++)
                                {
                                    cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                                      "creuser,credate,cretime,upduser,upddate,updtime,reqtype,requestno,appractid, " +
                                                      "approver,apprdate,apprstat,apprcoment) " +
                                                      "VALUES('" + empid + "', '" + startdate + "', '" + enddate + "', '" + subpagtype + "', (select isnull(max(recordno), 0)+1 as recordno from emp0378 " +
                                                      "where empid ='" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' " +
                                                      " and reqtype = '" + reqtype + "' and appractid = '20'), ' '," +
                                                      "'" + user_id + "', '" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("HH:mm:ss") + "', ' ', '" + insertdate.ToString("yyyy-MM-dd") + "', '" + insertdate.ToString("HH:mm:ss") + "'," +
                                                      "'" + reqtype + "', '" + reqno + "', '20', '" + apprlist[i] + "', '" + insertdate.ToString("yyyy-MM-dd") + "', '05', ' ')";
                                    cmd.ExecuteNonQuery();
                                    cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                                    string msgid = cmd.ExecuteScalar().ToString();
                                    cases = "LVAR";
                                    cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                             "values ('" + msgid + "','1', '" + apprlist[i] + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                                    cmd.ExecuteNonQuery();
                                }
                            }

                        }
                        else                //Individual Loan Acceptance
                        {
                            sdr.Close();
                            cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                            "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + comment + "', apprstat = '01' " +
                            "where empid = '" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and reqtype = '" + reqtype + "' and upduser = '' and delind <> 'X' " +
                            "and approver = '" + user_id + "' and apprstat = '05'";
                            cmd.ExecuteNonQuery();

                            if (apprcount == 0) //Loan Approved By all approvers
                            {
                                cmd.CommandText = "update emp0277 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', remarks = 'Approved', reqstat = '40' " +
                                "where emp = '" + empid + "' and subpagtype = '" + subpagtype + "' and recordno = '" + reqno + "' and reqstat = '20' and delind <> 'X'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                                string msgid = cmd.ExecuteScalar().ToString();
                                cases = "LVA";
                                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                                         "values ('" + msgid + "','1', '" + empid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        break;
                    }
                }

                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        public void rejectApprLeave(string empid, string reqno, string subpagtype, string comment, string lvdays, string user_id)
        {
            string cases = "LVRJ";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                //emp0377
                cmd.CommandText = "update emp0277 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', remarks = '" + comment + "', reqstat = '03' " +
                "where empid = '" + empid + "' and recordno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and lvdays = '" + lvdays + "'  and upduser = '' and delind <> 'X'";
                cmd.ExecuteNonQuery();

                //emp0378
                cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                            "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + comment + "', apprstat = '03' " +
                            "where empid = '" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and reqtype = '10' and upduser = '' " +
                            "and approver = '" + user_id + "' and apprstat = '05' and delind <> 'X'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                         "values ('" + msgid + "','1', '" + empid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                cmd.ExecuteNonQuery();
                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        public void leaveAcceptedAsPerPolicy(ESSModel model, string user_id)
        {
            string cases = "LVAPP";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                //emp0377
                cmd.CommandText = "update emp0277 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', remarks = '" + model.coments + "', reqstat = '02' " +
                "where empid = '" + model.empid + "' and recordno = '" + model.recordno + "' and subpagtype = '" + model.loanid + "' and lvdays = '" + model.totdays + "' and upduser = '' and delind <> 'X'";
                cmd.ExecuteNonQuery();

                //emp0378
                cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                            "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + model.coments + "', apprstat = '02' " +
                            "where empid = '" + model.empid + "' and requestno = '" + model.recordno + "' and subpagtype = '" + model.loanid + "' and reqtype = '20' and upduser = '' " +
                            "and approver = '" + user_id + "' and apprstat = '05' and delind <> 'X'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                         "values ('" + msgid + "','1', '" + model.empid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                cmd.ExecuteNonQuery();
                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }

        public void apprLeaveRecomended(string empid, string reqno, string subpagtype, string startdate, string enddate, string apprrecodtypid, string comment, string reqtype, string user_id)
        {
            string cases = "LVRC";
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DatabaeseClass db = new DatabaeseClass();
            DateTime insertdate = db.convertedinsertdate(DateTime.Now.ToString());
            SqlTransaction trans;
            SqlCommand cmd = con.CreateCommand();
            trans = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.Transaction = trans;

                //emp0378 Update recomend
                cmd.CommandText = "update emp0378 set upduser = '" + user_id + "' , upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', " +
                "updtime = '" + insertdate.ToString("HH:mm:ss") + "', apprcoment = '" + comment + "', apprstat = '01' " +
                "where empid = '" + empid + "' and requestno = '" + reqno + "' and subpagtype = '" + subpagtype + "' and reqtype = '" + reqtype + "' and upduser = ''";
                cmd.ExecuteNonQuery();

                //emp0378 new approval
                cmd.CommandText = "INSERT INTO emp0378(empid,begdate,enddate,subpagtype,recordno,delind, " +
                                   "creuser,credate,cretime,upduser,upddate,updtime, " +
                                   "reqtype,requestno,appractid, approver,apprdate,apprstat,apprcoment) " +
                                   "VALUES('" + empid + "','" + startdate + "','" + enddate + "','" + subpagtype + "','5', ' '," +
                                   "'" + user_id + "','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "',' ','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', " +
                                   "'" + reqtype + "','" + reqno + "','20','" + apprrecodtypid + "','" + startdate + "','05',' ')";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "select ISNULL(MAX(msgid),0)+1 from inbox";
                string msgid = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "insert into inbox (msgid, recordno, recip, cc, sender, subject, message, status, unread, filepath, dbtimestmp, chatviewid, notfcase) " +
                         "values ('" + msgid + "','1', '" + apprrecodtypid + "', '', '" + user_id + "', (select notftitle from ZNotificationcase where notfcase = '" + cases + "'), '', '','X','', '" + db.convertedinsertdate(DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', '', '" + cases + "')";
                cmd.ExecuteNonQuery();
                trans.Commit();
                HomeController.popup_status = "Success";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                HomeController.popup_status = "Error";

            }
            finally
            {
                trans.Dispose();
                con.Close();
            }
        }



        /**************************LEAVE REQUEST*****************************************************/


        /**************************RESIG REQUEST*****************************************************/
        public string ESSResigInsert(ESSModel model, string user_id)
        {
            string msg = "Success";
            return msg;
        }


        public bool UpdateTimeSheet(DateTime date, string clientid, string empid)
        {
            DatabaeseClass dc = new DatabaeseClass();
            DateTime insertdate = dc.convertedinsertdate(DateTime.Now.ToString());
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            string query1 = "update emp0280 set delind = 'X', upduser = '" + user_role + "', upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', updtime= '" + insertdate.ToString("HH:mm:ss") + "', dbtimestmp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                "where clientid = '" + clientid + "' and empid = '" + empid + "' and begdate > '" + date.ToString("yyyy-MM-dd") + "' " +
                "and begdate < '" + date.AddDays(1).ToString("yyyy-MM-dd") + "'";
            using (SqlTransaction trans = con.BeginTransaction())
            {
                SqlCommand command = con.CreateCommand();
                command.Connection = con;
                command.Transaction = trans;
                try
                {
                    command.CommandText = query1;
                    command.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                    con.Close();

                }
                return true;
            }
        }

        public void InsertCompanyDoc(CompanyDocModel db)
        {
            DatabaeseClass dc = new DatabaeseClass();
            DateTime insertdate = dc.convertservertopsttimezone(DateTime.Now.ToString());
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();

            string query1 = "select isnull(max(recordno), 0) as recordno from corpdoc where " +
                            "doccategory = '" + db.deptid + "'";

            using (SqlTransaction trans = con.BeginTransaction())
            {
                SqlCommand command = con.CreateCommand();
                command.Connection = con;
                command.Transaction = trans;
                try
                {
                    command.CommandText = query1;
                    int record = 0;
                    record = Convert.ToInt32(command.ExecuteScalar());
                    record += 1;
                    command.CommandText = "INSERT INTO corpdoc(docid, doccategory, adminid, filepath, [filename], recordno, delind, begdate, enddate, upduser, upddate, updtime, dbtimestamp) " +
                                    "VALUES('" + db.increment + "', '" + db.deptid + "', '" + db.user_id + "', '" + db.imagepath +
                                    "', '" + db.filename + "', '" + record + "', '', '" + db.date + "', '" + db.date + "', '', '', '', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    command.ExecuteNonQuery();
                    trans.Commit();
                    CompanyDocController.popup_status = "Success";
                }
                catch (Exception ex)
                {
                    CompanyDocController.popup_status = "Error";
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                    con.Close();
                }
            }
        }

        public bool UpdateCompanyDoc(string filename, string category, string user_role)
        {
            DatabaeseClass dc = new DatabaeseClass();
            DateTime insertdate = dc.convertedinsertdate(DateTime.Now.ToString());

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            string query1 = "update corpdoc set delind = 'X', upduser = '" + user_role + "', upddate = '" + insertdate.ToString("yyyy-MM-dd") + "', updtime= '" + insertdate.ToString("HH:mm:ss") + "', dbtimestamp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                "where filename = '" + filename + "' and doccategory = '" + category + "'";
            using (SqlTransaction trans = con.BeginTransaction())
            {
                SqlCommand command = con.CreateCommand();
                command.Connection = con;
                command.Transaction = trans;
                try
                {
                    command.CommandText = query1;
                    command.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                    con.Close();
                }
            }
            return true;
        }

        public void insertAPPForm(string user_id, string[] basicinfo, string[] contactinfo, string[] skillinfo, string[] qualevel, string[] instname,
            DateTime[] sdateedu, DateTime[] fdateedu, string[] degreeedu, string[] majors, string[] gpa)
        {
            DatabaeseClass dc = new DatabaeseClass();

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
            con.Open();
            DateTime insertdate = dc.convertedinsertdate(DateTime.Now.ToString());
            string norng = "select [status], rngfrom, rngto from norange where norngid = '01' and norngobj = 'aplid' ";

            int aplidold = 0, rngfrom = 0, rngto = 0;

            da.CreateConnection();
            da.InitializeSQLCommandObject(da.GetCurrentConnection, norng);
            da.OpenConnection();
            da.obj_reader = da.obj_sqlcommand.ExecuteReader();
            if (da.obj_reader.HasRows)
            {
                while (da.obj_reader.Read())
                {
                    aplidold = Convert.ToInt32(da.obj_reader["status"].ToString());
                    rngfrom = Convert.ToInt32(da.obj_reader["rngfrom"].ToString());
                    rngto = Convert.ToInt32(da.obj_reader["rngto"].ToString());
                }
            }
            else
            {
                HomeController.error_code = 2;
                return;
            }
            if( aplidold > rngfrom && aplidold < rngto)
            {
                int aplidnew = aplidold + 1;
                SqlTransaction trans;
                SqlCommand cmd = con.CreateCommand();
                trans = con.BeginTransaction();
                try
                {
                    cmd.Connection = con;
                    cmd.Transaction = trans;

                    /* Number range table update*/
                    cmd.CommandText = "update norange set [status] = '" + aplidnew + "' , dbtimestmp = '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                      "where norngid = '01' and norngobj = 'aplid' ";
                    cmd.ExecuteNonQuery();

                    /* Apl0010 Table Insert ~ aarea, subarea */
                    cmd.CommandText = "insert into apl0010 ( aplid, begdate, enddate, asubpagtyp, recordno, " +
                                      "delind, creuser, credate, cretime, upduser, " +
                                      "upddate, updtime, aarea, asubarea, agroup, asubgrp, dbtimestmp) " +
                                      "VALUES('" + aplidnew + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "',' ', '1', " +
                                      "' ','" + user_id + "','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "',' ', " +
                                      "'" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "','3000','10','1','04','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    cmd.ExecuteNonQuery();

                    /* Apl0100 Table Insert ~ basic info*/
                    cmd.CommandText = "insert apl0100 (aplid, begdate, enddate, asubpagtyp, recordno, " +
                                      "delind, creuser, credate, cretime, upduser, " +
                                      "upddate, updtime, formadd, lastname, midname, " +
                                      "firstname, birthname, initials, secname, idnumber, " +
                                      "gender, nationality, commlang, birthdate, birthplace, " +
                                      "cobirth, [state], maritalstat, dbtimestmp) " +
                                      "VALUES('" + aplidnew + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "',' ', '1', " +
                                      "' ','" + user_id + "','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "',' ', " +
                                      "'" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', ' ', ' ', ' ', " +
                                      "'" + basicinfo[0] + "','" + basicinfo[0] + "',' ',' ','" + basicinfo[1] + "', " +
                                      "'" + basicinfo[2] + "','PK','UR','" + basicinfo[4] + "','" + basicinfo[5] + "', " +
                                      "'PK', ' ', '" + basicinfo[3] + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    cmd.ExecuteNonQuery();

                    /* Apl0110 Table Insert ~ contact info*/
                    cmd.CommandText = "insert into apl0110 (aplid, begdate, enddate, asubpagtyp, recordno, " +
                                      "delind, creuser, credate, cretime, upduser, " +
                                      "upddate, updtime, careof, street1, street2, " +
                                      "zipcode, district, city, ctry, phone, " +
                                      "phone2, phone3, phone4, phone5, linkedin, " +
                                      "skype, email, col0, col1, dbtimestmp) " +
                                      "VALUES('" + aplidnew + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "',' ', '1', " +
                                      "' ','" + user_id + "','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "',' ', " +
                                      "'" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', ' ', '" + contactinfo[0] + "','" + contactinfo[1] + "', " +
                                      "' ', ' ', '" + contactinfo[2] + "','" + contactinfo[3] + "', '" + contactinfo[5] + "' , " +
                                      "'" + contactinfo[6] + "','" + contactinfo[7] + "',' ', ' ', '" + contactinfo[8] + "', " +
                                      "'" + contactinfo[9] + "','" + contactinfo[4] + "',' ',' ','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    cmd.ExecuteNonQuery();
                    if (skillinfo != null)
                    {
                        /*Apl0531 Table Insert ~ skill info */
                        for (int i = 0; i < skillinfo.Length; i++)
                        {
                            if (skillinfo[i] != "")
                            {
                                cmd.CommandText = "insert into apl0531 (aplid, begdate, enddate, asubpagtyp, recordno, " +
                                                  "lineid, delind, creuser, credate, cretime, " +
                                                  "upduser, upddate, updtime, skillno, aplprof, " +
                                                  "acquisdate, remarks, dbtimestmp) " +
                                                  "VALUES('" + aplidnew + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + qualevel[i] + "', '" + (i + 1) + "', " +
                                                  "'" + (i + 1) + "',' ','" + user_id + "','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', " +
                                                  "' ','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', ' ', ' ', " +
                                                  "'" + insertdate.ToString("yyyy-MM-dd") + "','" + skillinfo[i] + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    /*Apl0520 Table Insert ~ education info */
                    for (int i = 0; i < qualevel.Length; i++)
                    {
                        if (qualevel[i] != "")
                        {
                            cmd.CommandText = "insert into apl0520 (aplid, begdate, enddate, asubpagtyp, recordno, " +
                                              "lineid, delind, creuser, credate, cretime, " +
                                              "upduser, upddate, updtime, startdate, finishdate, " +
                                              "apledu, instdept, instname, instcity, inststate, " +
                                              "instctry, aplprof, inudfield1, inudfield2, inudfield3, " +
                                              "inudfield4, dbtimestmp) " +
                                              "VALUES('" + aplidnew + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + qualevel[i] + "', '" + (i + 1) + "', " +
                                              "'" + (i+1) + "',' ','" + user_id + "','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', " +
                                              "' ','" + insertdate.ToString("yyyy-MM-dd") + "','" + insertdate.ToString("HH:mm:ss") + "', '" + sdateedu[i].ToString("yyyy-MM-dd") + "','" + fdateedu[i].ToString("yyyy-MM-dd") + "', " +
                                              "'" + degreeedu[i] + "',' ', '" + instname[i] + "',' ', ' ', " +
                                              "'PK', ' ', '" + majors[i] + "','" + gpa[i] + "', ' ', " +
                                              "' ', '" + insertdate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    HomeController.error_code = 1;

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    HomeController.error_code = 0;
                }
                finally
                {
                    trans.Dispose();
                    con.Close();
                }
            }
            else
            {
                HomeController.error_code = 3;
                return;
            }
        }
    }
}