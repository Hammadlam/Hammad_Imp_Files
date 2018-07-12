using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.SessionState;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Globalization;
using TimeZoneNames;

namespace MvcSchoolWebApp.Controllers
{
    public class DatabaeseClass : Controller
    {
        public Database.Database da = new Database.Database("Falconlocal");
        private string user_role = Convert.ToString(System.Web.HttpContext.Current.Session["User_Role"]) ?? " ";
        private string user_id = Convert.ToString(System.Web.HttpContext.Current.Session["User_Id"]) ?? " ";
        List<campus> user = new List<campus>();
        List<class_d> usr_class = new List<class_d>();
        List<class_d> get_clist = new List<class_d>();
        List<section> usr_section = new List<section>();
        List<subject> usr_subject = new List<subject>();
        List<studentnames> usrstdnames = new List<studentnames>();

        public void validatelogin(string username, string password)
        {
            List<LoginModel> item = new List<LoginModel>();
            List<Users> user_dtl = new List<Users>();
            string query = "select usr.userid, usr.passwd, usr.fname, usr.lname, usr.acdtitle, usr.menuprof, img.imagepath, mpd.menustat, " +
                           "mpd.menulabel, mpd.menuid, mpd.tcode, std.eareatxt from usr01 usr " +
                           "inner join menuprofdtl mpd on usr.menuprof = mpd.menuprof " +
                           "left join emp0170 e17 on e17.empid = usr.userid " +
                           "left join imageobj img on img.imageid = e17.imageid " +
                           "inner join earea std on usr.acdtitle = std.earea where userid = '" + username + "' order by mpd.menuid";
            da.CreateConnection();
            da.OpenConnection();
            da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
            da.obj_reader = da.obj_sqlcommand.ExecuteReader();
            if (da.obj_reader.HasRows)
            {
                while (da.obj_reader.Read())
                {
                    string pass = da.obj_reader["passwd"].ToString().Trim();
                    if (pass == password)
                    {
                        user_dtl.Add(new Users
                        {
                            user_id = da.obj_reader["userid"].ToString().Trim(),
                            user_earea = da.obj_reader["acdtitle"].ToString().Trim(),
                            user_fullname = da.obj_reader["fname"].ToString() + " " + da.obj_reader["lname"].ToString(),
                            user_image = da.obj_reader["imagepath"].ToString(),
                            user_role = da.obj_reader["eareatxt"].ToString()
                        });

                        item.Add(new LoginModel
                        {
                            menuprof = da.obj_reader["menuprof"].ToString(),
                            menuid = da.obj_reader["menuid"].ToString(),
                            menustat = da.obj_reader["menustat"].ToString(),
                            tcode = da.obj_reader["tcode"].ToString(),
                            earea = da.obj_reader["acdtitle"].ToString(),
                            loginstatus = true
                        });
                    }
                    else
                    {
                        item.Add(new LoginModel
                        {
                            loginstatus = false
                        });
                    }
                }
                da.CloseConnection();
                if (user_dtl.Count > 0)
                {
                    System.Web.HttpContext.Current.Session["User_Id"] = user_dtl[0].user_id;
                    System.Web.HttpContext.Current.Session["User_Role"] = user_dtl[0].user_earea;
                    System.Web.HttpContext.Current.Session["User_Dtl"] = user_dtl;
                    System.Web.HttpContext.Current.Session["User_Rights"] = item;
                }
            }
        }

        public DateTime converteddisplaydate(string date)
        {
            String dbTime = date;
            DateTime time = DateTime.Parse(dbTime);
            string user_timezone = System.Web.HttpContext.Current.Session["usr_timezone"].ToString();
            string lang = CultureInfo.CurrentCulture.Name;
            var abbreviations = TZNames.GetNamesForTimeZone(user_timezone, lang);

            DateTime servertime = time;
            string serverregion = TimeZone.CurrentTimeZone.StandardName;
            string usrregion = abbreviations.Standard;

            TimeZoneInfo server = TimeZoneInfo.FindSystemTimeZoneById(serverregion);
            TimeZoneInfo pst = TimeZoneInfo.FindSystemTimeZoneById(usrregion);

            DateTime date2 = DateTime.SpecifyKind(servertime, DateTimeKind.Unspecified);
            DateTime dt = TimeZoneInfo.ConvertTime(date2, server, pst);
            
            return dt;
        }

        public DateTime convertusertoservertimezone(string date)
        {
            String dbTime = date;
            DateTime time = DateTime.Parse(dbTime);
            string user_timezone = System.Web.HttpContext.Current.Session["usr_timezone"].ToString();
            string lang = CultureInfo.CurrentCulture.Name;
            var abbreviations = TZNames.GetNamesForTimeZone(user_timezone, lang);

            DateTime servertime = time;
            string serverregion = TimeZone.CurrentTimeZone.StandardName;
            string usrregion = abbreviations.Standard;

            TimeZoneInfo server = TimeZoneInfo.FindSystemTimeZoneById(serverregion);
            TimeZoneInfo user = TimeZoneInfo.FindSystemTimeZoneById(usrregion);

            DateTime date2 = DateTime.SpecifyKind(servertime, DateTimeKind.Unspecified);
            DateTime dt = TimeZoneInfo.ConvertTime(date2, user, server);

            return dt;
        }

        public DateTime convertservertousertimezone(string date)
        {
            String dbTime = date;
            DateTime time = DateTime.Parse(dbTime);
            string user_timezone = System.Web.HttpContext.Current.Session["usr_timezone"].ToString();
            string lang = CultureInfo.CurrentCulture.Name;
            var abbreviations = TZNames.GetNamesForTimeZone(user_timezone, lang);

            DateTime servertime = time;
            string serverregion = TimeZone.CurrentTimeZone.StandardName;
            string usrregion = abbreviations.Standard;

            TimeZoneInfo server = TimeZoneInfo.FindSystemTimeZoneById(serverregion);
            TimeZoneInfo user = TimeZoneInfo.FindSystemTimeZoneById(usrregion);

            DateTime date2 = DateTime.SpecifyKind(servertime, DateTimeKind.Unspecified);
            DateTime dt = TimeZoneInfo.ConvertTime(date2, server, user);

            return dt;
        }

        public DateTime convertpsttousertimezone(string date)
        {
            String dbTime = date;
            DateTime time = DateTime.Parse(dbTime);
            string user_timezone = System.Web.HttpContext.Current.Session["usr_timezone"].ToString();
            string lang = CultureInfo.CurrentCulture.Name;
            var abbreviations = TZNames.GetNamesForTimeZone(user_timezone, lang);

            DateTime servertime = time;
            string serverregion = "Pakistan Standard Time";
            string usrregion = abbreviations.Standard;

            TimeZoneInfo server = TimeZoneInfo.FindSystemTimeZoneById(serverregion);
            TimeZoneInfo user = TimeZoneInfo.FindSystemTimeZoneById(usrregion);

            DateTime date2 = DateTime.SpecifyKind(servertime, DateTimeKind.Unspecified);
            DateTime dt = TimeZoneInfo.ConvertTime(date2, server, user);

            return dt;
        }

        public DateTime convertservertopsttimezone(string date)
        {
            String dbTime = date;
            DateTime time = DateTime.Parse(dbTime);
            string user_timezone = System.Web.HttpContext.Current.Session["usr_timezone"].ToString();
            string lang = CultureInfo.CurrentCulture.Name;
            var abbreviations = TZNames.GetNamesForTimeZone(user_timezone, lang);

            DateTime servertime = time;
            string serverregion = "Pakistan Standard Time";
            string usrregion = TimeZone.CurrentTimeZone.StandardName;

            TimeZoneInfo server = TimeZoneInfo.FindSystemTimeZoneById(serverregion);
            TimeZoneInfo user = TimeZoneInfo.FindSystemTimeZoneById(usrregion);

            DateTime date2 = DateTime.SpecifyKind(servertime, DateTimeKind.Unspecified);
            DateTime dt = TimeZoneInfo.ConvertTime(date2, user, server);

            return dt;
        }

        //public string getstandardtimezonename(string timezoneid)
        //{
        //    Dictionary<string, string> keys = new Dictionary<string, string>();
        //    keys.Add("Asia/Karachi", "Pakistan Standard Time");
        //    keys.Add("Asia/Dubai", "Gulf Standard Time");
        //    string standardname = keys[timezoneid];
        //    return standardname;
        //}

        public DateTime convertedinsertdate(string date)
        {
            String dbTime = date;
            DateTime time = DateTime.Parse(dbTime);
            string user_timezone = System.Web.HttpContext.Current.Session["usr_timezone"].ToString();
            string lang = CultureInfo.CurrentCulture.Name;
            var abbreviations = TZNames.GetNamesForTimeZone(user_timezone, lang);

            DateTime servertime = time;
            string serverregion = TimeZone.CurrentTimeZone.StandardName;
            string usrregion = abbreviations.Standard;

            TimeZoneInfo server = TimeZoneInfo.FindSystemTimeZoneById(serverregion);
            TimeZoneInfo pst = TimeZoneInfo.FindSystemTimeZoneById(usrregion);

            DateTime date2 = DateTime.SpecifyKind(servertime, DateTimeKind.Unspecified);
            DateTime dt = TimeZoneInfo.ConvertTime(date2, server, pst);
            return dt;
        }

        public void Fill_usrdtl()
        {
            da.CreateConnection();
            da.OpenConnection();
            user_id = System.Web.HttpContext.Current.Session["User_Id"].ToString();
            user_role = System.Web.HttpContext.Current.Session["User_Role"].ToString();
            List<SelectListItem> campuses = FillCamp(user_id);
            for (int i = 0; i < campuses.Count; i++)
            {
                campus cam = new campus(campuses[i].Value, campuses[i].Text);
                user.Add(cam);

            }
            System.Web.HttpContext.Current.Session["ssn_usr_campus"] = user;

            for (int i = 0; i < user.Count; i++)
            {
                List<SelectListItem> classes = FillClass(user[i].campusid, user_id);
                for (int j = 0; j < classes.Count; j++)
                {
                    class_d cls = new class_d(user[i].campusid, classes[j].Value, classes[j].Text);
                    usr_class.Add(cls);
                }
            }
            System.Web.HttpContext.Current.Session["ssn_usr_class"] = usr_class;

            for (int i = 0; i < usr_class.Count; i++)
            {
                List<SelectListItem> sections = FillSection(usr_class[i].campusid, usr_class[i].classid, user_id);
                for (int j = 0; j < sections.Count; j++)
                {
                    section sec = new section(usr_class[i].campusid, usr_class[i].classtxt, usr_class[i].classid, sections[j].Value, sections[j].Text);
                    usr_section.Add(sec);
                }
            }
            System.Web.HttpContext.Current.Session["ssn_usr_section"] = usr_section;

            for (int i = 0; i < usr_section.Count; i++)
            {
                List<SelectListItem> subj = FillSubject(usr_section[i].campusid, usr_section[i].classid, usr_section[i].sectionid, user_id);
                for (int j = 0; j < subj.Count; j++)
                {
                    subject sub = new subject(usr_section[i].campusid, usr_section[i].classid, usr_section[i].classtxt, usr_section[i].sectionid, usr_section[i].sectiontxt, subj[j].Value, subj[j].Text);
                    usr_subject.Add(sub);
                }
            }
            System.Web.HttpContext.Current.Session["ssn_usr_subject"] = usr_subject;

            for (int i = 0; i < usr_section.Count; i++)
            {
                List<SelectListItem> stdnames = studentname(usr_section[i].campusid, usr_section[i].classid, usr_section[i].sectionid);
                for (int j = 0; j < stdnames.Count; j++)
                {
                    studentnames std = new studentnames(usr_section[i].campusid, usr_section[i].classid, usr_section[i].classtxt, usr_section[i].sectionid, usr_section[i].sectiontxt, stdnames[j].Value, stdnames[j].Text);
                    usrstdnames.Add(std);
                }
            }
            System.Web.HttpContext.Current.Session["ssn_usr_student"] = usrstdnames;

            da.CloseConnection();
        }

        public List<SelectListItem> EmptyList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            return list;
        }

        [HandleError(ExceptionType = typeof(HttpNotFoundResult), View = "Error")]
        public List<SelectListItem> FillCamp(String id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                string query;
                string tablename = "";
                string colname = "";
                if (user_role == "1000" || user_role == "2000" || user_role == "3000")
                {
                    query = "select distinct Schcampus.campusid , Schcampus.campustxt from Schcampus " +
                    "inner join emp0710 on Schcampus.campusid = emp0710.campusid " +
                    "where emp0710.empid ='" + id + "' and emp0710.delind <> 'X' ";
                }
                else if (user_role == "5000")
                {
                    query = "select distinct Schcampus.campusid , Schcampus.campustxt from Schcampus " +
                    "inner join std0710 on Schcampus.campusid = std0710.campusid " +
                    "where std0710.stdid ='" + id + "' and std0710.delind <> 'X' ";
                }
                else
                {
                    query = "select distinct Schcampus.campusid , Schcampus.campustxt from Schcampus " +
                    "inner join std0710 on Schcampus.campusid = std0710.campusid " +
                    "inner join stdmain on stdmain.stdid = std0710.stdid " +
                    "where stdmain.parentid ='" + id + "' and std0710.delind <> 'X' and stdmain <> 'X' ";
                }


                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                // da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["campustxt"].ToString(),
                            Value = da.obj_reader["campusid"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                }
                else
                {
                    da.obj_reader.Close();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCmp. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<SelectListItem> FillClass(String campusId, String id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            DatabaseModel dm = new DatabaseModel();
            try
            {
                string tablename;
                string query = "select distinct classid, classtxt, Cast(Substring(classid,2,LEN(classid)) as Integer) from Schclass " +
                                "where campusid = '" + campusId + "'";
                string new_query = "";

                if (user_role == "1000" || user_role == "2000")
                {
                    new_query = query;
                }
                else if (user_role == "3000")
                {
                    new_query = query + "and classid IN (select distinct classid from emp0710 where empid = '" + id + "')";
                }
                else if (user_role == "5000")
                {
                    new_query = query + "and classid IN (select distinct classid from std0710 where stdid = '" + id + "')";
                }
                else
                {
                    new_query = query + "and classid IN (select distinct classid from std0710 s71 inner join stdmain sm on s71.stdid = sm.stdid where parentid = '" + id + "')";
                }
                new_query += " order by Cast(Substring(classid,2,LEN(classid)) as Integer) asc";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, new_query);
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["classtxt"].ToString(),
                            Value = da.obj_reader["classid"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                }
                else
                {
                    da.obj_reader.Close();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCls. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<SelectListItem> FillSection(String campusId, String classId, String id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                string query = "";
                if (user_role == "1000" || user_role == "2000")
                    query = "select distinct ss.sectionid, ss.sectiontxt from schsection ss inner join schclass sc on sc.sectionid = ss.sectionid " +
                            "where sc.campusid = '" + campusId + "' and sc.classid = '" + classId + "'";
                else if (user_role == "3000")
                {
                    query = "select distinct ss.sectionid, ss.sectiontxt from Schsection as ss inner join emp0710 as e71 on ss.sectionid = e71.sectionid " +
                              "where e71.empid = '" + id + "' and e71.campusid = '" + campusId + "' and e71.classid = '" + classId + "'";
                }
                else if (user_role == "5000")
                {
                    query = "select distinct ss.sectionid, ss.sectiontxt from Schsection as ss inner join std0710 as s71 on ss.sectionid = s71.sectionid " +
                              "where s71.stdid = '" + id + "' and s71.campusid = '" + campusId + "' and s71.classid = '" + classId + "'";
                }
                else
                {
                    query = "select distinct ss.sectionid, ss.sectiontxt from Schsection as ss inner join std0710 as s71 on ss.sectionid = s71.sectionid " +
                              " inner join stdmain sm on s71.stdid = sm.stdid where sm.parentid = '" + id + "' and s71.campusid = '" + campusId + "' and s71.classid = '" + classId + "'";
                }
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["sectiontxt"].ToString(),
                            Value = da.obj_reader["sectionid"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                }
                else
                {
                    da.obj_reader.Close();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FSec. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<SelectListItem> FillSubject(String campusId, String classId, String sectionId, String id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                string tablename = "";
                string query = "select distinct ss.subjectid, ss.subjecttxt from schsubject ss " +
                               "inner join " + tablename + " sc on sc.classid = ss.classid " +
                               "where sc.campusid = '" + campusId + "' ";

                if (user_role == "1000" || user_role == "2000")
                {
                    query = "select distinct ss.subjectid, ss.subjecttxt from schsubject ss " +
                            "inner join schclass sc on sc.classid = ss.classid " +
                            "where sc.campusid = '" + campusId + "' and ss.classid = '" + classId + "'";
                }
                else if (classId == "" && sectionId == "")
                    query = "select distinct ssub.subjectId, ssub.subjecttxt from schsubject as ssub " +
                            "inner join  emp0710 as e71 on e71.classid = ssub.classid " +
                            "where e71.empid = '" + id + "' and e71.campusid = '" + campusId + "' ";
                else if (user_role == "3000")
                {
                    query = "select distinct ssub.subjectId, ssub.subjecttxt from schsubject as ssub " +
                            "inner join  emp0710 as e71 on e71.subjectid = ssub.subjectid " +
                            "where e71.empid = '" + id + "' and e71.campusid = '" + campusId + "' and e71.classid = '" + classId + "' and e71.sectionid = '" + sectionId + "'";
                }
                else if (user_role == "5000")
                {
                    query = "select distinct ssub.subjectId, ssub.subjecttxt from schsubject as ssub " +
                            "inner join  std0710 as s71 on s71.classid = ssub.classid " +
                            "where s71.stdid = '" + id + "' and s71.campusid = '" + campusId + "' and s71.classid = '" + classId + "' and s71.sectionid = '" + sectionId + "'";
                }
                else
                {
                    query = "select distinct ssub.subjectId, ssub.subjecttxt from schsubject as ssub " +
                            "inner join  std0710 as s71 on s71.classid = ssub.classid " +
                            "inner join stdmain as sm on s71.stdid = sm.stdid " +
                            "where sm.parentid = '" + id + "' and s71.campusid = '" + campusId + "' and s71.classid = '" + classId + "' and s71.sectionid = '" + sectionId + "'";
                }
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["subjecttxt"].ToString(),
                            Value = da.obj_reader["subjectid"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                }
                else
                {
                    da.obj_reader.Close();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FSub. Error Details: " + ex.Message);
            }
            return items;
        }

        public List<SelectListItem> getcampus()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            DatabaseModel dm = new DatabaseModel();
            if (System.Web.HttpContext.Current.Session["ssn_usr_campus"] != null)
            {
                List<campus> lst = (List<campus>)System.Web.HttpContext.Current.Session["ssn_usr_campus"];
                for (int i = 0; i < lst.Count; i++)
                {
                    items.Add(new SelectListItem
                    {
                        Value = lst[i].campusid,
                        Text = lst[i].campusname
                    });
                }
            }
            else
            {
                Response.Redirect("~/Login/Index");
            }
            return items;
        }

        public List<SelectListItem> getclass(String campusId, String id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            DatabaseModel dm = new DatabaseModel();
            if (System.Web.HttpContext.Current.Session["ssn_usr_class"] != null)
            {
                List<class_d> lst = (List<class_d>)System.Web.HttpContext.Current.Session["ssn_usr_class"];
                for (int i = 0; i < lst.Count; i++)
                {
                    if (lst.ElementAt(i).campusid.Equals(campusId))
                    {

                        items.Add(new SelectListItem
                        {
                            Value = lst[i].classid,
                            Text = lst[i].classtxt
                        });
                    }
                }
            }
            return items;
        }

        public List<SelectListItem> getsection(String campusId, string classid, String id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            DatabaseModel dm = new DatabaseModel();
            if (System.Web.HttpContext.Current.Session["ssn_usr_section"] != null)
            {
                List<section> lst = (List<section>)System.Web.HttpContext.Current.Session["ssn_usr_section"];
                for (int i = 0; i < lst.Count; i++)
                {
                    if (lst.ElementAt(i).campusid.Equals(campusId) && lst.ElementAt(i).classid.Equals(classid))
                    {

                        items.Add(new SelectListItem
                        {
                            Value = lst[i].sectionid,
                            Text = lst[i].sectiontxt
                        });
                    }
                }
            }
            return items;
        }

        public List<SelectListItem> getsubject(String campusId, String classId, String sectionId, String id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            DatabaseModel dm = new DatabaseModel();
            if (System.Web.HttpContext.Current.Session["ssn_usr_subject"] != null)
            {
                List<subject> lst = (List<subject>)System.Web.HttpContext.Current.Session["ssn_usr_subject"];
                for (int i = 0; i < lst.Count; i++)
                {
                    if (lst.ElementAt(i).campus_id.Equals(campusId) && lst.ElementAt(i).class_id.Equals(classId) && lst.ElementAt(i).section_id.Equals(sectionId))
                    {

                        items.Add(new SelectListItem
                        {
                            Value = lst[i].subject_id,
                            Text = lst[i].subject_txt
                        });
                    }
                }
            }
            return items;
        }

        public List<SelectListItem> getsubject(string campusid)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            DatabaseModel dm = new DatabaseModel();
            if (System.Web.HttpContext.Current.Session["ssn_usr_subject"] != null)
            {
                List<subject> lst = (List<subject>)System.Web.HttpContext.Current.Session["ssn_usr_subject"];
                for (int i = 0; i < lst.Count; i++)
                {
                    if (lst.ElementAt(i).campus_id.Equals(campusid))
                    {

                        items.Add(new SelectListItem
                        {
                            Value = lst[i].subject_id,
                            Text = lst[i].subject_txt
                        });
                    }
                }
            }
            return items;
        }

        public List<SelectListItem> getstudentname(String campusId, String classId, String sectionId)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            Personalinfo pro = new Personalinfo();
            if (System.Web.HttpContext.Current.Session["ssn_usr_student"] != null)
            {
                List<studentnames> lst = (List<studentnames>)System.Web.HttpContext.Current.Session["ssn_usr_student"];
                for (int i = 0; i < lst.Count; i++)
                {
                    if (lst.ElementAt(i).campus_id.Equals(campusId) && lst.ElementAt(i).class_id.Equals(classId) && lst.ElementAt(i).section_id.Equals(sectionId))
                    {
                        items.Add(new SelectListItem
                        {
                            Value = lst[i].student_id,
                            Text = lst[i].student_txt
                        });
                    }
                }
            }
            return items;
        }

        [HandleError]
        public List<SelectListItem> FillEmployee(String campusId)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                da.CreateConnection();
                string query = "select distinct e71.empid, ep.firstname + ' ' + ep.midname + ' ' + ep.lastname as 'empname' from emp0710 as e71 "+
                                "inner join empmain on e71.empid = empmain.empid "+
                                "inner join emppers as ep on e71.empid = ep.empid "+
                                "inner join usr01 usr on usr.userid = empmain.empid "+
                                "where e71.delind <> 'X' and empmain.delind <> 'X' and ep.delind <> 'X' and empmain.earea not in ('4000', '5000') "+
                                "and usr.menuprof <> '50000000' and e71.campusid = '"+campusId+"' order by empname ASC";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["empname"].ToString(),
                            Value = da.obj_reader["empid"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FEmp. Error Details: " + ex.Message);
            }
            return items;
        }


        [HandleError]
        public List<SelectListItem> FillSession(String campusId, String classId, String sectionId, String teacherId)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                da.CreateConnection();
                string query = "select distinct schsubject.subjectid, schsubject.subjecttxt from schsubject " +
                "where classid =  '" + classId + "'";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["subjecttxt"].ToString(),
                            Value = da.obj_reader["subjectid"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FSess. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<SelectListItem> FillCategory()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                da.CreateConnection();
                string query = "select lesncategory, categoryname from lessonplncategory";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {

                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["categoryname"].ToString(),
                            Value = da.obj_reader["lesncategory"].ToString().Trim()
                        });

                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCat. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<SelectListItem> FillCategoryTimeTable()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                da.CreateConnection();
                string query = "select ttcategory, categorytxt from timetablecategory";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["categorytxt"].ToString(),
                            Value = da.obj_reader["ttcategory"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCTT. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public string FillTeacherName(string id)
        {
            string items = "";
            try
            {
                da.CreateConnection();
                string query = "select ep.firstname + ' ' + ep.midname + ' ' + ep.lastname as empname from emppers ep where ep.empid = '" + id + "' order by empname";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items = da.obj_reader["empname"].ToString();
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FTN. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<SelectListItem> FillStudent(String campusId, String classId)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                da.CreateConnection();
                string query = "select stdmain.stdid from stdmain inner join stdarea on stdmain.stdarea = stdarea.stdarea inner join " +
                "std0710 on stdmain.stdid = std0710.stdid inner join Schcampus on std0710.campusid = Schcampus.campusid " +
                "where stdmain.stdarea = '5000' and  std0710.campusid = '" + campusId + "' and std0710.classid = '" + classId + "'";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["stdid"].ToString(),
                            Value = da.obj_reader["stdid"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.CloseConnection();
                    da.obj_reader.Close();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FStd. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<SelectListItem> studentname(String campusId, String classId, String sectionId)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                string query = "select stdpers.stdid, stdpers.firstname + ' ' + stdpers.midname + ' ' + stdpers.lastname as stdname from std0710 inner join stdpers on std0710.stdid = stdpers.stdid " +
                    "inner join stdmain as sm on std0710.stdid = sm.stdid where sm.stdarea = '5000' and std0710.campusid = '" + campusId
                    + "' and std0710.classid = '" + classId + "' and std0710.sectionid = '" + sectionId + "' and stdpers.delind<> 'X' and std0710.delind <> 'X' and sm.delind <> 'X'";

                if (user_role == "5000")
                {
                    query += " and std0710.stdid = '" + user_id + "' order by stdname";
                }
                else if (user_role == "4000")
                {
                    query += " and sm.parentid = '" + user_id + "' order by stdname";
                }
                else
                {
                    query += " order by stdname";
                }
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["stdname"].ToString(),
                            Value = da.obj_reader["stdid"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                }
                else
                {
                    da.obj_reader.Close();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-StdNm. Error Details: " + ex.Message);
            }
            return items;
        }


        [HandleError]
        public List<SelectListItem> FillSubModule()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                da.CreateConnection();
                string query = "select subresltyp, subresltptxt from schresltype order by reslttyp";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["subresltptxt"].ToString(),
                            Value = da.obj_reader["subresltyp"].ToString().Trim()
                        });
                    }
                    da.CloseConnection();
                    da.obj_reader.Close();
                }
                else
                {
                    da.CloseConnection();
                    da.obj_reader.Close();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FSM. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<SelectListItem> FillModule()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                da.CreateConnection();
                string query = "select reslttyp, resltyptxt from schresltype order by reslttyp";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["resltyptxt"].ToString(),
                            Value = da.obj_reader["reslttyp"].ToString().Trim()
                        });
                    }
                    da.CloseConnection();
                    da.obj_reader.Close();
                }
                else
                {
                    da.CloseConnection();
                    da.obj_reader.Close();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FM. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillMarkAttendance(string campusid, string classid, string sectionid, string subjectid, DateTime date)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                int count = 0;
                da.CreateConnection();
                string query = "select distinct std.stdid, sp.firstname + ' ' + sp.midname + ' ' + sp.lastname as stdname, s22.halfdayind, s22.lvdays, " +
                                "(select count(*) from std0710 s17 where s17.campusid = '" + campusid + "' and s17.classid = '" + classid + "' and s17.sectionid = '" + sectionid + "') as counts " +
                                " from stdmain as std" +
                                " inner join stdpers as sp on std.stdid = sp.stdid" +
                                " inner join std0710 as s18 on std.stdid = s18.stdid" +
                                " inner join std0220 as s22 on std.stdid = s22.empid" +
                                " inner join Schclass on s18.classid = Schclass.classid" +
                                " inner join schsubject on Schclass.classid = schsubject.classid" +
                                " where std.stdarea = '5000' and s18.campusid = '" + campusid + "' and " +
                                " s18.classid = '" + classid + "' and s18.sectionid = '" + sectionid + "' " +
                                " and s22.delind <> 'X' and std.delind <> 'X' and s18.delind <> 'X' " +
                                " and s22.begdate between '" + date.ToString("yyyy-MM-dd") + "' and '" + date.ToString("yyyy-MM-dd") + "' ";

                if (subjectid != "")
                {
                    query += " and remarks = '" + subjectid + "' order by stdname";
                }
                else
                {
                    query += " and remarks = 'Full day' order by stdname";
                }
                string attd = "";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        if (da.obj_reader["halfdayind"].ToString().Trim() == "X")
                        {
                            attd = "Leave";
                        }
                        else if (da.obj_reader["lvdays"].ToString().Trim() == "1")
                        {
                            attd = "Absent";
                        }
                        else
                        {
                            attd = "Present";
                        }
                        items.Add(new JQGridModel
                        {
                            studentId = da.obj_reader["stdid"].ToString(),
                            studentName = da.obj_reader["stdname"].ToString(),
                            status = attd,
                            count = Convert.ToInt32(da.obj_reader["counts"].ToString())
                        });
                    }
                    da.CloseConnection();
                    da.obj_reader.Close();
                }
                else
                {
                    string query2 = "select distinct std.stdid, sp.firstname + ' ' + sp.midname + ' ' + sp.lastname as stdname, " +
                                "(select count(*) from std0710 s17 where s17.campusid = '" + campusid + "' and s17.classid = '" + classid + "' and s17.sectionid = '" + sectionid + "') as counts " +
                                " from stdmain as std" +
                                " inner join stdpers as sp on std.stdid = sp.stdid" +
                                " inner join std0710 as s18 on std.stdid = s18.stdid" +
                                " inner join Schclass on s18.classid = Schclass.classid" +
                                " inner join schsubject on Schclass.classid = schsubject.classid" +
                                " where std.stdarea = '5000' and s18.campusid = '" + campusid + "' and s18.classid = '" + classid + "'" +
                                " and s18.sectionid = '" + sectionid + "' order by stdname";

                    da.InitializeSQLCommandObject(da.GetCurrentConnection, query2);
                    da.obj_reader.Close();
                    da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                    if (da.obj_reader.HasRows)
                    {
                        while (da.obj_reader.Read())
                        {
                            attd = "Present";

                            items.Add(new JQGridModel
                            {
                                studentId = da.obj_reader["stdid"].ToString(),
                                studentName = da.obj_reader["stdname"].ToString(),
                                status = attd,
                                count = Convert.ToInt32(da.obj_reader["counts"].ToString())
                            });
                        }
                        da.CloseConnection();
                        da.obj_reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FMA. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillWeeklyTest(string studentid, string subjectid, string campusid, string classid, string sectionid, string resulttype)
        {
            String s = resulttype + " ";
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                da.CreateConnection();
                string resultypquery = "select distinct reslttyp from schresltype where resltyptxt = '" + resulttype + "'";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, resultypquery);
                da.OpenConnection();
                string subresultid = da.obj_sqlcommand.ExecuteScalar().ToString().Trim();
                da.CloseConnection();

                string query = "";
                string user_role = System.Web.HttpContext.Current.Session["User_Role"].ToString().Trim();
                string user_id = System.Web.HttpContext.Current.Session["User_Id"].ToString().Trim();
                if (user_role == "1000" || user_role == "2000" || user_role == "3000")
                {
                    query = "select distinct  schsubject.subjecttxt, totalmarks, examinationmarks, projectmarks, testmarks, oralmarks, assignmentmarks, " +
                            "obtainedmarks, rt.subresltptxt, percentage, p1,p2,p3, r.begdate from Schresult as r " +
                               "inner join schsubject on r.subjectid = schsubject.subjectid " +
                               "inner join schresltype as rt on r.resulttype = rt.reslttyp and r.subresltyp = rt.subresltyp " +
                               "inner join schrsltpb pb on r.campusid = pb.campusid and r.classid = pb.classid and r.sectionid = pb.sectionid and r.subresltyp = pb.subresultype " +
                               "where stdid = '" + studentid + "' and schsubject.subjectid = '" + subjectid + "' and r.campusid = '" + campusid + "' and r.classid = '" + classid + "' " +
                               "and r.delind <> 'X' and r.sectionid = '" + sectionid + "' and rt.reslttyp='" + subresultid + "' and pb.delind <> 'X'";
                }
                else if (user_role == "4000" || user_role == "5000")
                {
                    query = "select distinct ss.subjecttxt, sr.subresltyp, rt.subresltptxt, sr.totalmarks, sr.examinationmarks, sr.projectmarks, " +
                            "sr.testmarks, sr.oralmarks, sr.assignmentmarks, sr.obtainedmarks, sr.percentage, sr.p1, sr.p2, sr.p3 " +
                            "from schresult sr inner join Schrsltpb sp on sp.campusid = sr.campusid and sr.classid = sp.classid and " +
                            "sr.sectionid = sp.sectionid and sr.subresltyp = sp.subresultype " +
                            "inner join Schcampus sc on sc.campusid = sr.campusid " +
                            "inner join schclass scl on scl.classid = sr.classid " +
                            "inner join schsubject ss on ss.subjectid = sr.subjectid " +
                            "inner join schresltype rt on rt.subresltyp = sr.subresltyp " +
                            "inner join schrsltpb pb on sr.campusid = pb.campusid and sr.classid = pb.classid and sr.sectionid = pb.sectionid and sr.subresltyp = pb.subresultype " +
                            "where sr.campusid = '" + campusid + "' and sr.classid = '" + classid + "' and sr.sectionid = '" + sectionid + "' and " +
                            "sr.subjectid = '" + subjectid + "' and sr.stdid = '" + user_id + "' and sr.delind <> 'X' " +
                            "and rt.reslttyp = '" + subresultid + "' and sp.delind <> 'X'";
                }

                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new JQGridModel
                        {
                            resulttype = da.obj_reader["subresltptxt"].ToString(),
                            totalmarks = da.obj_reader["totalmarks"].ToString(),
                            examinationmarks = da.obj_reader["examinationmarks"].ToString(),
                            projectmarks = da.obj_reader["projectmarks"].ToString(),
                            testmarks = da.obj_reader["testmarks"].ToString(),
                            oralmarks = da.obj_reader["oralmarks"].ToString(),
                            assignmentmarks = da.obj_reader["assignmentmarks"].ToString(),
                            obtainedmarks = da.obj_reader["obtainedmarks"].ToString(),
                            percentage = Convert.ToDouble(da.obj_reader["percentage"].ToString()),
                            p1marks = da.obj_reader["p1"].ToString(),
                            p2marks = da.obj_reader["p2"].ToString(),
                            p3marks = da.obj_reader["p3"].ToString()

                        });
                    }
                    da.CloseConnection();
                    da.obj_reader.Close();
                }
                else
                {
                    da.CloseConnection();
                    da.obj_reader.Close();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FWT. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillMarksUploadGrid(string campusid, string classid, string sectionid, string subjectid, string rsltype)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                da.CreateConnection();
                string query = "select distinct std.stdid, sp.firstname + ' ' + sp.midname + ' ' + sp.lastname as stdname, sch.totalmarks, sch.examinationmarks, sch.projectmarks, sch.assignmentmarks, sch.testmarks, sch.oralmarks, sch.p1 , sch.p2 , sch.p3, " +
                    "(select count(*) from std0710 s17 where s17.campusid = '" + campusid + "' and s17.classid = '" + classid + "' and s17.sectionid = '" + sectionid + "') as counts " +
                    "from stdmain std" +
                    " inner join stdpers sp on std.stdid = sp.stdid " +
                    " inner join std0710 as s18 on std.stdid = s18.stdid " +
                    " inner join Schclass on s18.classid = Schclass.classid " +
                    " inner join schsubject on Schclass.classid = schsubject.classid " +
                    " inner join schresult sch on std.stdid = sch.stdid " +
                    " where std.stdarea = '5000' and sch.campusid = '" + campusid + "' and sch.classid = '" + classid + "' and sch.sectionid = '" + sectionid + "' and" +
                    " sch.subjectid = '" + subjectid + "' and  " +
                    " sch.subresltyp = '" + rsltype + "' and sch.delind <> 'X' and s18.delind <> 'X' and sp.delind <> 'X' order by stdname";

                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new JQGridModel
                        {
                            studentId = da.obj_reader["stdid"].ToString(),
                            studentName = da.obj_reader["stdname"].ToString(),
                            exammarks = da.obj_reader["examinationmarks"].ToString(),
                            projectmarks = da.obj_reader["projectmarks"].ToString(),
                            assignmarks = da.obj_reader["assignmentmarks"].ToString(),
                            testmarks = da.obj_reader["testmarks"].ToString(),
                            oralmarks = da.obj_reader["oralmarks"].ToString(),
                            p1marks = da.obj_reader["p1"].ToString(),
                            p2marks = da.obj_reader["p2"].ToString(),
                            p3marks = da.obj_reader["p3"].ToString(),
                            totalmarkstxt = da.obj_reader["totalmarks"].ToString(),
                            classes = classid,
                            count = Convert.ToInt32(da.obj_reader["counts"].ToString())
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }

                else
                {
                    da.obj_reader.Close();
                    query = "select distinct std.stdid , sp.firstname + ' ' + sp.midname + ' ' + sp.lastname as stdname, " +
                                       "(select count(*) from std0710 s17 where s17.campusid = '" + campusid + "' and s17.classid = '" + classid + "' and s17.sectionid = '" + sectionid + "') as counts " +
                                       "from stdmain as std " +
                                       "inner join stdpers as sp on std.stdid = sp.stdid " +
                                       "inner join std0710 as s18 on std.stdid = s18.stdid " +
                                       "inner join Schclass on s18.classid = Schclass.classid " +
                                       "inner join schsubject on Schclass.classid = schsubject.classid " +
                                       "where std.stdarea = '5000' and s18.campusid = '" + campusid + "' and s18.classid = '" + classid + "' and s18.sectionid = '" + sectionid + "' order by stdname";
                    da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                    da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                    while (da.obj_reader.Read())
                    {
                        items.Add(new JQGridModel
                        {
                            studentId = da.obj_reader["stdid"].ToString(),
                            studentName = da.obj_reader["stdname"].ToString(),
                            exammarks = "0",
                            projectmarks = "0",
                            assignmarks = "0",
                            testmarks = "0",
                            oralmarks = "0",
                            p1marks = "0",
                            p2marks = "0",
                            p3marks = "0",
                            classes = classid,
                            count = Convert.ToInt32(da.obj_reader["counts"].ToString())
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FMUG. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillClassWiseGrid(string campusid, string classid, string sectionid)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                da.CreateConnection();
                string query = "select distinct std.stdid , sp.firstname + ' ' + sp.midname as stdname, sp.lastname from stdmain as std " +
                               "inner join stdpers as sp on std.stdid = sp.stdid " +
                               "inner join std0710 as s18 on std.stdid = s18.stdid " +
                               "inner join Schclass on s18.classid = Schclass.classid " +
                               "inner join schsubject on Schclass.classid = schsubject.classid " +
                               "where std.stdarea = '5000' and s18.campusid = '" + campusid + "' and s18.classid = '" + classid + "' " +
                               "and s18.sectionid = '" + sectionid + "' order by stdname asc, sp.lastname asc";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {


                        items.Add(new JQGridModel
                        {
                            studentId = da.obj_reader["stdid"].ToString(),
                            studentName = da.obj_reader["stdname"].ToString(),
                            fatherName = da.obj_reader["lastname"].ToString()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCWG. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillMasterTimeTable(string campusid, string user_roles)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                da.CreateConnection();
                string query = "select t.timetbid, t.filename, t.filepath, t.begdate from timetable as t " +
                               "where t.delind <> 'X' AND t.ttcategory = 101 AND t.campusid = '" + campusid + "'";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    int i = 1;
                    while (da.obj_reader.Read())
                    {
                        items.Add(new JQGridModel
                        {
                            serialNo = i,// Convert.ToInt32(da.obj_reader["timetbid"].ToString()),
                            fileName = da.obj_reader["filename"].ToString(),
                            date = Convert.ToDateTime(da.obj_reader["begdate"]).ToString("dd-MMMM-yyyy"),
                            viewButton = da.obj_reader["filepath"].ToString(),
                            user_role = user_roles
                        });
                        i++;
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FMTT. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillTeacherTimeTable(string campusid, string user_roles)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                da.CreateConnection();
                string query = "select t.timetbid, t.filename, t.filepath, t.begdate from timetable as t " +
                               "where t.delind <> 'X' and t.ttcategory = 102 AND t.campusid = '" + campusid + "' " +
                               "order by t.begdate desc";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    int i = 1;
                    while (da.obj_reader.Read())
                    {
                        items.Add(new JQGridModel
                        {
                            serialNo = i,
                            fileName = da.obj_reader["filename"].ToString(),
                            date = Convert.ToDateTime(da.obj_reader["begdate"]).ToString("dd-MMMM-yyyy"),
                            viewButton = da.obj_reader["filepath"].ToString(),
                            user_role = user_roles
                        });
                        i++;
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FTTT. Error Details: " + ex.Message);
            }

            return items;
        }

        [HandleError]
        public List<JQGridModel> FillClassTimeTable(string campusid, string classid, string sectionid, string user_roles)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                da.CreateConnection();
                string query = "select t.timetbid, t.filename, t.filepath, t.begdate from timetable as t " +
                               "where t.delind <> 'X' and t.ttcategory = 103 AND t.campusid = '" + campusid + "' AND t.classid = '" + classid + "' AND t.sectionid = '" + sectionid + "'";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    int i = 1;
                    while (da.obj_reader.Read())
                    {
                        items.Add(new JQGridModel
                        {
                            serialNo = i,//Convert.ToInt32(da.obj_reader["timetbid"].ToString()),
                            fileName = da.obj_reader["filename"].ToString(),
                            date = Convert.ToDateTime(da.obj_reader["begdate"]).ToString("dd-MMMM-yyyy"),
                            viewButton = da.obj_reader["filepath"].ToString(),
                            user_role = user_roles
                        });
                        i++;
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCTT. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<DatabaseModel> FillWeeklyLessonPlan(string empid, string begdate, string campusid, string classid, string sectionid, string subjectid)
        {
            empid = System.Web.HttpContext.Current.Session["User_Id"].ToString();
            DateTime date1 = DateTime.Parse(begdate);
            List<DatabaseModel> items = new List<DatabaseModel>();
            try
            {
                string query = "select wp.empid, wp.begdate, wp.enddate, wp.campusid, wp.classid, wp.sectionid, wp.subjectid, wp.topic, wp.objective, "+
                                "wp.resource, wp.evaluation, wp.teach_method, wp.tb_teach_method, wp.read_disc, wp.tb_read_disc, wp.writtenwork, "+
                                "wp.tb_writtenwork, wp.wrapup, wp.tb_wrapup, wp.evaluationstd, wp.evaluationteach, wpc.princplcomnt from schweeklyplan wp "+
                                "left join schlessonplancom wpc on wp.campusid = wpc.campusid and wp.classid = wpc.classid and "+
                                "wp.sectionid = wpc.sectionid and wp.subjectid = wpc.subjectid and wp.begdate = wpc.begdate and wp.enddate = wpc.enddate "+
                                "where wp.begdate = '"+date1.ToString("yyyy-MM-dd")+ "' and wp.enddate = '" + date1.ToString("yyyy-MM-dd") + "' and " +
                                "wp.campusid = '"+campusid+"' and wp.classid = '"+classid+"' and wp.sectionid = '"+sectionid+"' and wp.subjectid = '"+subjectid+"' and wp.delind <> 'X'";
                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    string teacherid = "";
                    while (da.obj_reader.Read())
                    {
                        teacherid = da.obj_reader["empid"].ToString();
                        string[] dates = new string[] { "tb_teach_method", "tb_read_disc", "tb_writtenwork", "tb_wrapup" };
                        string[] timebreak = new string[dates.Length];
                        for (int i = 0; i < dates.Length; i++)
                        {
                            DateTime date = Convert.ToDateTime(da.obj_reader[dates[i]]);
                            string tb_obj = date.ToString("mm");
                            int min = Convert.ToInt16(tb_obj);
                            timebreak[i] = min.ToString();
                        }
                        items.Add(new DatabaseModel
                        {
                            campusid = da.obj_reader["campusid"].ToString(),
                            classesid = da.obj_reader["classid"].ToString(),
                            sectionid = da.obj_reader["sectionid"].ToString(),
                            subjectid = da.obj_reader["subjectid"].ToString(),
                            topic = da.obj_reader["topic"].ToString(),
                            objective = da.obj_reader["objective"].ToString(),
                            resource = da.obj_reader["resource"].ToString(),
                            evaluation = da.obj_reader["evaluation"].ToString(),
                            teach_method = da.obj_reader["teach_method"].ToString(),
                            tm_time_break_id = timebreak[0],
                            read_disc = da.obj_reader["read_disc"].ToString(),
                            rd_time_break_id = timebreak[1],
                            writtenwork = da.obj_reader["writtenwork"].ToString(),
                            ww_time_break_id = timebreak[2],
                            wrapup = da.obj_reader["wrapup"].ToString(),
                            wu_time_break_id = timebreak[3],
                            evaluationstdid = da.obj_reader["evaluationstd"].ToString(),
                            evaluationteacherid = da.obj_reader["evaluationteach"].ToString(),
                            princ_comments = da.obj_reader["princplcomnt"].ToString(),
                        });
                    }
                    for (int i = 0; i < items.Count; i++)
                    {
                        items[i].teachername = FillTeacherName(teacherid);
                        items[i].teacherid = teacherid;
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FWLP. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<DatabaseModel> FillWeeklyLessonPlanPP(string campusid, string classid, string sectionid, string subjectid, DateTime begdate, DateTime enddate)
        {
            //empid = System.Web.HttpContext.Current.Session["User_Id"].ToString();
            //DateTime m = DateTime.Parse(month);

            //DateTime begdate1 = DateTime.Parse(begdate);

            List<DatabaseModel> items = new List<DatabaseModel>();
            try
            {
                string query = "select wp.empid, wp.begdate, wp.enddate, wp.campusid, wp.classid, wp.sectionid, wp.subjectid, wp.topic, wp.objective, " +
                               "wp.circletime, wp.initact, wp.devproc, wp.assess, wp.homewrk, wpc.princplcomnt " +
                               "from schwkplnpp wp left join schlessonplancom wpc on wp.campusid = wpc.campusid and wp.classid = wpc.classid and " +
                               "wp.sectionid = wpc.sectionid and wp.subjectid = wpc.subjectid and wp.begdate = wpc.begdate and wp.enddate = wpc.enddate " +
                               "where wp.begdate = '"+begdate+"' and wp.enddate = '"+enddate+"' and wp.campusid = '"+ campusid +"' "+
                               "and wp.classid = '"+ classid +"' and wp.sectionid = '"+ sectionid +"' and wp.subjectid = '"+ subjectid +"' and wp.delind <> 'X'";
                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    string teacherid = "";
                    while (da.obj_reader.Read())
                    {
                        teacherid = da.obj_reader["empid"].ToString();
                        
                        items.Add(new DatabaseModel
                        {
                            campusid = da.obj_reader["campusid"].ToString(),
                            classesid = da.obj_reader["classid"].ToString(),
                            sectionid = da.obj_reader["sectionid"].ToString(),
                            subjectid = da.obj_reader["subjectid"].ToString(),
                            topic = da.obj_reader["topic"].ToString(),
                            objective = da.obj_reader["objective"].ToString(),
                            resource = da.obj_reader["circletime"].ToString(),
                            evaluation = da.obj_reader["initact"].ToString(),
                            writtenwork = da.obj_reader["devproc"].ToString(),
                            wrapup = da.obj_reader["assess"].ToString(),
                            evaluationstdid = da.obj_reader["homewrk"].ToString(),
                            princ_comments = da.obj_reader["princplcomnt"].ToString()
                        });
                    }
                    for (int i = 0; i < items.Count; i++)
                    {
                        items[i].teachername = FillTeacherName(teacherid);
                        items[i].studentid = teacherid;
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FWLP. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillLessonPlan(string campus, string schclass, string section, string subject, string pagename, string user_roles)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                da.CreateConnection();
                string query = "select l.lessonid, l.filename, l.filepath, l.begdate from lessonplan as l " +
                               "inner join lessonplncategory as lp on l.lesncategory = lp.lesncategory " +
                               "where l.delind <> 'X' and lp.categoryname = '" + pagename + "' AND l.campusid = '" + campus + "' " +
                               "AND l.classid = '" + schclass + "' AND l.sectionid = '" + section + "' " +
                               "AND l.subjectid = '" + subject + "' order by l.begdate desc";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    int i = 1;
                    while (da.obj_reader.Read())
                    {
                        items.Add(new JQGridModel
                        {
                            serialNo = i, //Convert.ToInt32(da.obj_reader["lessonid"].ToString()),
                            fileName = da.obj_reader["filename"].ToString(),
                            date = Convert.ToDateTime(da.obj_reader["begdate"]).ToString("dd-MMMM-yyyy"),
                            Caption = pagename,
                            user_role = user_roles
                        });
                        i++;
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FLP. Error Details: " + ex.Message);
            }
            return items;
        }



        [HandleError]
        public List<JQGridModel> FillCampusStrength(string campusid)
        {
            int totalstrengt = 0;
            int totalboy = 0;
            int totalgirl = 0;
            JQGridModel jq = new JQGridModel();
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                da.CreateConnection();
                string query = "select sc.classtxt + ' ' + '-' + ' ' + sp71.sectionid as 'class/level', " +
                    "count(sp71.classid) as 'class total' , count(case when sp.gender = 'M' then 1 end) as 'Boys', " +
                    "count(case when sp.gender = 'F' then 1 end) as 'Girls' from std0710 as sp71 " +
                    "inner join schclass as sc on sp71.campusid = sc.campusid and sp71.classid = sc.classid and sp71.sectionid = sc.sectionid " +
                    "inner join stdpers as sp on sp71.stdid = sp.stdid where sp71.campusid = '" + campusid + "' " +
                    "group by sp71.campusid, sc.classtxt, sp71.sectionid";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        totalstrengt += Convert.ToInt32(da.obj_reader["class total"]);
                        totalboy += Convert.ToInt32(da.obj_reader["Boys"]);
                        totalgirl += Convert.ToInt32(da.obj_reader["Girls"]);

                        items.Add(new JQGridModel
                        {
                            classes = da.obj_reader["class/level"].ToString(),
                            total = da.obj_reader["class total"].ToString(),
                            boys = da.obj_reader["Boys"].ToString(),
                            girls = da.obj_reader["Girls"].ToString()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
                items.Add(new JQGridModel
                {
                    totalstrength = totalstrengt,
                    totalboys = totalboy,
                    totalgirls = totalgirl
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCS. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillCampusCapacity(string campusid)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                da.CreateConnection();
                int totseats = 0, totavail = 0, totoccup = 0;
                string query = "select distinct sq.campusid, sc.classtxt, sq.classid, sq.Totalseats, sq.seatsoccupied, sq.seatsremn, " +
                    "Cast(Substring(sq.classid,2,LEN(sq.classid)) as Integer) from schclassquota as sq " +
                    "inner join schclass as sc on sq.campusid = sc.campusid and sq.classid = sc.classid " +
                    "where sq.campusid = '" + campusid + "' and sq.delind <> 'X' order by Cast(Substring(sq.classid,2,LEN(sq.classid)) as Integer)";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        totseats += Convert.ToInt32(da.obj_reader["Totalseats"]);
                        totoccup += Convert.ToInt32(da.obj_reader["seatsoccupied"]);
                        totavail += Convert.ToInt32(da.obj_reader["seatsremn"]);

                        items.Add(new JQGridModel
                        {
                            classes = da.obj_reader["classtxt"].ToString(),
                            total = da.obj_reader["Totalseats"].ToString(),
                            occupied = da.obj_reader["seatsoccupied"].ToString(),
                            available = da.obj_reader["seatsremn"].ToString(),
                            totseats = totseats,
                            totoccupied = totoccup,
                            totavaible = totavail
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCC. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillActivityGrid(string campusid, string classid, string sectionid)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                da.CreateConnection();
                string schquery = "select std.stdid, sp.firstname + ' ' + sp.midname + ' ' + sp.lastname as stdname, sa.sports, sa.assemblypresent, sa.gk, sa.behaviour, " +
                                  "sa.discipline, sa.cleanliness, sa.compliance, sa.taskdeadline, sa.recordno, sa.teachercom, sa.delind, " +
                                  "(select count(*) from std0710 s17 where s17.campusid = '" + campusid + "' and s17.classid = '" + classid + "' and s17.sectionid = '" + sectionid + "') as counts " +
                                  "from schactivitygrade sa " +
                                  "right join std0710 s17 on s17.stdid = sa.stdid " +
                                  "inner join stdpers sp on sp.stdid = s17.stdid " +
                                  "inner join stdmain std on std.stdid = s17.stdid " +
                                  "where s17.classid = '" + classid + "' and s17.sectionid = '" + sectionid + "' " +
                                  "and s17.campusid = '" + campusid + "' and std.stdarea = '5000'";

                string query = schquery + " and sa.delind <> 'X'";
                string orderby = " order by stdname asc";
                string final_query = query + orderby;
                da.InitializeSQLCommandObject(da.GetCurrentConnection, final_query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new JQGridModel
                        {
                            studentId = da.obj_reader["stdid"].ToString(),
                            studentName = da.obj_reader["stdname"].ToString(),
                            status = da.obj_reader["sports"].ToString(),
                            assignppt = da.obj_reader["assemblypresent"].ToString(),
                            gk = da.obj_reader["gk"].ToString(),
                            behave = da.obj_reader["behaviour"].ToString(),
                            discp = da.obj_reader["discipline"].ToString(),
                            clean = da.obj_reader["cleanliness"].ToString(),
                            compliance = da.obj_reader["compliance"].ToString(),
                            task = da.obj_reader["taskdeadline"].ToString(),
                            teachercom = da.obj_reader["teachercom"].ToString(),
                            count = Convert.ToInt32(da.obj_reader["counts"].ToString())
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    final_query = schquery + orderby;
                    da.InitializeSQLCommandObject(da.GetCurrentConnection, final_query);
                    da.obj_reader.Close();
                    da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                    if (da.obj_reader.HasRows)
                    {
                        while (da.obj_reader.Read())
                        {
                            items.Add(new JQGridModel
                            {
                                studentId = da.obj_reader["stdid"].ToString(),
                                studentName = da.obj_reader["stdname"].ToString(),
                                status = "03",
                                assignppt = "03",
                                gk = "03",
                                behave = "03",
                                discp = "03",
                                clean = "03",
                                compliance = "03",
                                task = "03",
                                teachercom = "",
                                count = Convert.ToInt32(da.obj_reader["counts"].ToString())
                            });
                        }
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FAC. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<PPMatricModel> FillPPMatric(string campus, string classes, string section, string subject, string teacher, string date, string module)
        {
            List<PPMatricModel> items = new List<PPMatricModel>();
            try
            {
                da.CreateConnection();
                string query = "select field, collabel from flexobjdtl where flexobjnr = '" + (classes.Trim() + subject.Trim()) + "' order by fieldid";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new PPMatricModel
                        {
                            field = da.obj_reader["field"].ToString(),
                            collabl = da.obj_reader["collabel"].ToString()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FPPM. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<PPMatricModel> FillPPMatricData(string campus, string classes, string section, string subject, string teacher, string date, string module)
        {
            List<PPMatricModel> items = new List<PPMatricModel>();
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
                conn.Open();
                string query = "select distinct sp.firstname + ' ' + sp.midname + ' ' + sp.lastname as stdname, ssc.charisticid, ssc.grade, ssc.charistictxt, " +
                "(select count(*) from std0710 s17 where s17.campusid = '" + campus + "' and s17.classid = '" + classes + "' and s17.sectionid = '" + section + "') as counts, " +
                "(select count(distinct ss.charisticid) from schsubchar ss inner join schpreresult sr on ss.stdid = sr.stdid and ss.subjectid = sr.subjectid where sr.campusid = '" + campus + "' and sr.classid = '" + classes + "' and sr.sectionid = '" + section + "' and sr.delind <> 'X' and ss.delind <> 'X') as colcount " +
                "from stdmain std inner join stdpers sp on std.stdid = sp.stdid inner join std0710 as s18 on std.stdid = s18.stdid " +
                "inner join Schclass on s18.classid = Schclass.classid inner join schsubject on Schclass.classid = schsubject.classid " +
                "inner join schpreresult sch on std.stdid = sch.stdid inner join schsubchar ssc on std.stdid = ssc.stdid " +
                "where std.stdarea = '5000' and sch.campusid = '" + campus + "' and sch.classid = '" + classes + "' and sch.sectionid = '" + section + "' and " +
                "sch.subjectid = '" + subject + "'  and ssc.subjectid = '" + subject + "' and sch.subresltyp = '6' and sch.delind <> 'X' and s18.delind <> 'X' and sp.delind <> 'X' and ssc.delind <> 'X' order by stdname, ssc.charisticid ";
                SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    int rowcount = Convert.ToInt16(ds.Tables[0].Rows[0]["counts"]);
                    int colcount = Convert.ToInt16(ds.Tables[0].Rows[0]["colcount"]);
                    string[,] ppdata = new string[rowcount, 17];
                    string stdname = "";
                    int index = 0;
                    int gradecount = 1;
                    for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
                    {
                        if (ds.Tables[0].Rows[k]["stdname"].ToString() != stdname)
                        {
                            gradecount = 1;
                            stdname = ds.Tables[0].Rows[k]["stdname"].ToString();
                            ppdata[index, 0] = ds.Tables[0].Rows[k]["stdname"].ToString();
                            index++;
                            k -= 1;
                        }
                        else
                        {
                            ppdata[index - 1, gradecount] = ds.Tables[0].Rows[k]["grade"].ToString();
                            gradecount++;
                        }
                    }
                    int i = 0;
                    for (int l = 0; l < rowcount; l++)
                    {
                        items.Add(new PPMatricModel
                        {
                            col0 = ppdata[l, 0],
                            col1 = ppdata[l, 1],
                            col2 = ppdata[l, 2],
                            col3 = ppdata[l, 3],
                            col4 = ppdata[l, 4],
                            col5 = ppdata[l, 5],
                            col6 = ppdata[l, 6],
                            col7 = ppdata[l, 7],
                            col8 = ppdata[l, 8],
                            col9 = ppdata[l, 9],
                            col10 = ppdata[l, 10],
                            col11 = ppdata[l, 11],
                            col12 = ppdata[l, 12],
                            col13 = ppdata[l, 13],
                            col14 = ppdata[l, 14],
                            col15 = ppdata[l, 15]
                        });
                    }
                }
                else
                {
                    string query2 = "select distinct std.stdid, sp.firstname + ' ' + sp.midname + ' ' + sp.lastname as stdname, " +
                                    "(select count(*) from std0710 s17 where s17.campusid = '" + campus + "' and s17.classid = '" + classes + "' and s17.sectionid = '" + section + "') as counts " +
                                    " from stdmain as std" +
                                    " inner join stdpers as sp on std.stdid = sp.stdid" +
                                    " inner join std0710 as s18 on std.stdid = s18.stdid" +
                                    " inner join Schclass on s18.classid = Schclass.classid" +
                                    " inner join schsubject on Schclass.classid = schsubject.classid" +
                                    " where std.stdarea = '5000' and s18.campusid = '" + campus + "' and s18.classid = '" + classes + "'" +
                                    " and s18.sectionid = '" + section + "' order by stdname ";
                    da.CreateConnection();
                    da.OpenConnection();
                    da.InitializeSQLCommandObject(da.GetCurrentConnection, query2);
                    da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                    if (da.obj_reader.HasRows)
                    {
                        while (da.obj_reader.Read())
                        {
                            items.Add(new PPMatricModel
                            {
                                col0 = da.obj_reader["stdname"].ToString(),
                                col1 = "Very Good",
                                col2 = "Very Good",
                                col3 = "Very Good",
                                col4 = "Very Good",
                                col5 = "Very Good",
                                col6 = "Very Good",
                                col7 = "Very Good",
                                col8 = "Very Good",
                                col9 = "Very Good",
                                col10 = "Very Good",
                                col11 = "Very Good",
                                col12 = "Very Good",
                                col13 = "Very Good",
                                col14 = "Very Good",
                                col15 = "Very Good"
                            });
                        }
                        da.obj_reader.Close();
                        da.CloseConnection();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FPPMD. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillStudentAttendance(string campusid, string classid, string sectionid, string studentid, DateTime dateid, string subjectid)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            string status = "";
            int totp = 0; int tota = 0;
            string month = dateid.ToString("yyyy-MM") + "-01";
            string nextmonth = dateid.AddMonths(1).ToString("yyyy-MM") + "-01";
            try
            {
                string query = "select distinct s22.begdate, remarks, std.stdid, sp.firstname, s22.halfdayind, s22.lvdays " +
                            "from stdmain as std inner join stdpers as sp on std.stdid = sp.stdid " +
                            "inner join std0710 as s18 on std.stdid = s18.stdid " +
                            "inner join std0220 as s22 on std.stdid = s22.empid " +
                            "inner join Schclass on s18.classid = Schclass.classid " +
                            "inner join schsubject on Schclass.classid = schsubject.classid " +
                            "where std.stdarea = '5000' and s22.delind <> 'X'  and s22.credate between '" + month + "' and '" + nextmonth + "' and sp.delind <> 'X' and s18.delind <> 'X' ";

                if ((campusid == "" && classid == "" && sectionid == "") || (campusid == null && classid == null && sectionid == null))
                {
                    if (subjectid == "")
                        query += "and std.stdid = '" + studentid + "' ";
                    else
                        query += "and std.stdid = '" + studentid + "' and remarks = '" + subjectid + "'";
                }
                else
                {
                    if (subjectid == "")
                        query += " and std.stdid = '" + studentid + "' and s18.campusid = '" + campusid + "' and s18.classid = '" + classid + "'" +
                        "and s18.sectionid = '" + sectionid + "'";
                    else
                        query += " and std.stdid = '" + studentid + "' and s18.campusid = '" + campusid + "' and s18.classid = '" + classid + "'" +
                        "and s18.sectionid = '" + sectionid + "' and remarks = '" + subjectid + "'";
                }


                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        if (da.obj_reader["halfdayind"].ToString().Trim() == "X")
                        {
                            status = "Leave";
                        }
                        else if (da.obj_reader["lvdays"].ToString().Trim() == "1")
                        {
                            status = "Absent";
                            tota++;
                        }
                        else
                        {
                            status = "Present";
                            totp++;
                        }

                        items.Add(new JQGridModel
                        {
                            date = Convert.ToDateTime(da.obj_reader["begdate"]).ToString("dd-MMMM-yyyy"),
                            day = Convert.ToDateTime(da.obj_reader["begdate"]).ToString("dddd"),
                            status = status,
                            total_present = "" + totp,
                            total_absent = "" + tota
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FStdA. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillEmployeeAttendance(string campusid, string empployeeid, DateTime dateid)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            string status = "";
            int totp = 0; int tota = 0;
            string month = dateid.ToString("yyyy-MM") + "-01";
            string nextmonth = dateid.AddMonths(1).ToString("yyyy-MM") + "-01";

            try
            {
                string query = "select distinct e22.begdate, e22.remarks, e22.halfdayind, e22.lvdays from emp0220 e22 " +
                            "where e22.delind <> 'X' and e22.empid = '" + empployeeid + "'  and e22.credate between '" + month + "' and '" + nextmonth + "'  ";

                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        if (da.obj_reader["halfdayind"].ToString().Trim() == "X")
                        {
                            status = "Leave";
                        }
                        else if (da.obj_reader["lvdays"].ToString().Trim() == "1")
                        {
                            status = "Absent";
                            tota++;
                        }
                        else
                        {
                            status = "Present";
                            totp++;
                        }

                        items.Add(new JQGridModel
                        {
                            date = da.obj_reader["begdate"].ToString(),
                            day = Convert.ToDateTime(da.obj_reader["begdate"]).ToString("dddd"),
                            status = status,
                            total_present = "" + totp,
                            total_absent = "" + tota
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FEmpA. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<ChartModel> FillCamAvergeTestAdmin(string campusid, string moduleid)
        {
            Boolean avgcam = false, avgmat = false;
            List<ChartModel> model = new List<ChartModel>();
            try
            {
                string query = "select * , round(((cast(x.totalpassed as FLOAT) / cast(x.studentappeared as FLOAT)) * 100), 2) as 'Class Average' from " +
                               "(select sc.resulttype, sc.subresltyp, count(stdid) as 'studentappeared', sc.campusid, sc.classid, scl.classtxt " +
                               ", count(case when percentage >= '49.75' then 1 end) as 'totalpassed' from schresult as sc " +
                               "inner join schresltype as r on sc.resulttype = r.reslttyp and sc.subresltyp = r.subresltyp " +
                               "inner join schrsltpb pb on sc.campusid = pb.campusid and sc.classid = pb.classid and sc.sectionid = pb.sectionid and sc.subresltyp = pb.subresultype "+
                               "inner join schclass as scl on sc.classid = scl.classid and sc.campusid = scl.campusid " +
                               "inner join Schclassgroup as scg on scg.classid = sc.classid where sc.delind <> 'X' " +
                               "and sc.subresltyp = '" + moduleid + "' and scg.classgroup = '01' and sc.campusid = '" + campusid + "' and pb.delind <> 'X' " +
                               "group by sc.resulttype, sc.subresltyp, sc.campusid, sc.classid, scl.classtxt) as x order by classid ";

                string query2 = "select * , round(((cast(x.totalpassed as FLOAT) / cast(x.studentappeared as FLOAT)) * 100), 2) as 'Class Average' from " +
                  "(select sc.resulttype, sc.subresltyp, count(stdid) as 'studentappeared', sc.campusid, sc.classid, scl.classtxt " +
                  ", count(case when percentage >= '44.75' then 1 end) as 'totalpassed' from schresult as sc " +
                  "inner join schrsltpb pb on sc.campusid = pb.campusid and sc.classid = pb.classid and sc.sectionid = pb.sectionid and sc.subresltyp =  pb.subresultype " +
                  "inner join schresltype as r on sc.resulttype = r.reslttyp and sc.subresltyp = r.subresltyp " +
                  "inner join schclass as scl on sc.classid = scl.classid and sc.campusid = scl.campusid " +
                  "inner join Schclassgroup as scg on scg.classid = sc.classid where sc.delind <> 'X' " +
                  "and sc.subresltyp = '" + moduleid + "' and scg.classgroup = '02' and sc.campusid = '" + campusid + "'  and pb.delind <> 'X'" +
                  "group by sc.resulttype, sc.subresltyp, sc.campusid, sc.classid, scl.classtxt) as x order by classid";

                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    avgcam = true;
                    while (da.obj_reader.Read())
                    {
                        model.Add(new ChartModel()
                        {
                            ClassName = da.obj_reader["classtxt"].ToString(),
                            averagecam = da.obj_reader["Class Average"].ToString(),
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query2);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    avgmat = true;
                    while (da.obj_reader.Read())
                    {
                        model.Add(new ChartModel()
                        {
                            ClassNameMat = da.obj_reader["classtxt"].ToString(),
                            averagemat = da.obj_reader["Class Average"].ToString(),
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                model.Add(new ChartModel()
                {
                    avgcheckcam = avgcam,
                    avgcheckmat = avgmat
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCATA. Error Details: " + ex.Message);
            }
            return model;
        }

        [HandleError]
        public List<ChartModel> FillStudentTestChart(string classid, string studentid, string moduleid, string id)
        {
            double subcount = 0, totalper = 0.0;
            List<ChartModel> model = new List<ChartModel>();
            try
            {
                string query;
                if (studentid != null)
                    query = "select distinct stdid, sc.campusid, sc.classid, sc.subjectid, sub.subjecttxt, obtainedmarks, totalmarks, percentage " +
                            "from Schresult as sc inner join schsubject as sub on sc.classid = sub.classid " +
                            "and sc.subjectid = sub.subjectid inner join schrsltpb pb on sc.campusid = pb.campusid and sc.classid = pb.classid and sc.sectionid = pb.sectionid and sc.subresltyp = pb.subresultype " +
                            "where stdid = '" + studentid + "' and " +
                            "sc.delind <> 'X' and pb.delind <> 'X' and subresltyp = '" + moduleid + "' and sc.classid = '" + classid + "' ";
                else
                    query = "select distinct stdid, sc.campusid, sc.classid, sc.subjectid, sub.subjecttxt, obtainedmarks, totalmarks, percentage " +
                            "from Schresult as sc inner join schsubject as sub on sc.classid = sub.classid " +
                            "and sc.subjectid = sub.subjectid inner join schrsltpb pb on sc.campusid = pb.campusid and sc.classid = pb.classid and sc.sectionid = pb.sectionid and sc.subresltyp = pb.subresultype " +
                            "where stdid = '" + id + "' and " +
                            "sc.delind <> 'X' and pb.delind <> 'X' and subresltyp = '" + moduleid + "' and sc.classid = '" + classid + "' ";

                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        subcount++;
                        totalper += Convert.ToDouble(da.obj_reader["percentage"]);
                        model.Add(new ChartModel()
                        {
                            subjectname = da.obj_reader["subjecttxt"].ToString(),
                            obtainmarks = da.obj_reader["obtainedmarks"].ToString(),
                            totalmarks = da.obj_reader["totalmarks"].ToString()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                if (subcount != 0)
                    totalper = totalper / subcount;
                else
                    totalper = 0;
                model.Add(new ChartModel()
                {
                    percentage = "" + totalper
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FSTC. Error Details: " + ex.Message);
            }
            return model;
        }

        [HandleError]
        public List<JQGridModel> GetAssisChart(string classid, string studentid, string moduleid, string id)
        {
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                string query;
                if (studentid != null)
                    query = "select sports,assemblypresent,gk,behaviour,discipline,cleanliness,compliance,taskdeadline " +
                            "from schactivitygrade where stdid = '" + studentid + "' and delind <> 'X' and classid = '" + classid + "'";
                else
                    query = "select sports,assemblypresent,gk,behaviour,discipline,cleanliness,compliance,taskdeadline " +
                            "from schactivitygrade where stdid = '" + user_id + "' and delind <> 'X' and classid = '" + classid + "'";

                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new JQGridModel()
                        {
                            status = da.obj_reader["sports"].ToString(),
                            assignppt = da.obj_reader["assemblypresent"].ToString(),
                            gk = da.obj_reader["gk"].ToString(),
                            behave = da.obj_reader["behaviour"].ToString(),
                            discp = da.obj_reader["discipline"].ToString(),
                            clean = da.obj_reader["cleanliness"].ToString(),
                            compliance = da.obj_reader["compliance"].ToString(),
                            task = da.obj_reader["taskdeadline"].ToString()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-GAC. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<SelectListItem> FillWeeks()
        {
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
            return weeklist;
        }

        [HandleError]
        public List<EventSchedular> GetCalendarEvent()
        {
            List<EventSchedular> items = new List<EventSchedular>();
            try
            {
                da.CreateConnection();
                string query = "select caldate6, remarks from calndr where remarks <> '' and " +
                               "calyear between 2017 and year(getdate()) and clubind <> 'X' ";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new EventSchedular
                        {
                            Subject = da.obj_reader["remarks"].ToString(),
                            Start = da.obj_reader["caldate6"].ToString(),
                            IsFullDay = true

                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-GCE. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<JQGridModel> FillAdmissionStatus(string campus)
        {
            JQGridModel jq = new JQGridModel();
            List<JQGridModel> items = new List<JQGridModel>();
            try
            {
                da.CreateConnection();
                string query = "select sc.classtxt + ' ' + '-' + ' ' + sp71.sectionid as 'class/level', " +
                    "count(sp71.classid) as 'class total' , count(case when sp.gender = 'M' then 1 end) as 'Boys', " +
                    "count(case when sp.gender = 'F' then 1 end) as 'Girls' from std0710 as sp71 " +
                    "inner join schclass as sc on sp71.campusid = sc.campusid and sp71.classid = sc.classid and sp71.sectionid = sc.sectionid " +
                    "inner join stdpers as sp on sp71.stdid = sp.stdid where sp71.campusid = '" + campus + "' " +
                    "group by sp71.campusid, sc.classtxt, sp71.sectionid";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new JQGridModel
                        {
                            studentId = da.obj_reader["class/level"].ToString(),
                            studentName = da.obj_reader["class total"].ToString(),
                            fatherName = da.obj_reader["Boys"].ToString(),
                            classes = da.obj_reader["Girls"].ToString(),
                            status = da.obj_reader["Girls"].ToString()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FAC. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<DatabaseModel> FillClasswiseChart(string campusid, string classid, string sectionid, string subjectid, string moduleid)
        {
            List<DatabaseModel> model = new List<DatabaseModel>();
            try
            {
                string query = "select sp.firstname + ' ' + sp.lastname as stdname, sr.obtainedmarks  " +
                "from Schresult sr inner join stdpers sp on sr.stdid = sp.stdid " +
                "where sr.subjectid = '" + subjectid + "' and sr.campusid = '" + campusid + "' and sr.classid = '" + classid + "' " +
                "and sr.delind <> 'X' and sr.sectionid = '" + sectionid + "' and sr.subresltyp = '" + moduleid + "'";
                Double marks = 0.0; string name = "No Name";
                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        if (Convert.ToDouble(da.obj_reader["obtainedmarks"]) > marks)
                        {
                            marks = Convert.ToDouble(da.obj_reader["obtainedmarks"]);
                            name = da.obj_reader["stdname"].ToString();
                        }
                        model.Add(new DatabaseModel()
                        {
                            teachername = da.obj_reader["stdname"].ToString(),
                            marks = da.obj_reader["obtainedmarks"].ToString(),
                            maxmarks = "" + marks.ToString(),
                            highname = name

                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    model.Add(new DatabaseModel()
                    {
                        maxmarks = "" + marks.ToString(),
                        highname = name
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCATA. Error Details: " + ex.Message);
            }
            return model;
        }

        [HandleError]
        public List<ChartModel> FillOverAllTestChart(string campusid, string subjectid, string moduleid, string userid)
        {
            Boolean avgcam = false, avgmat = false;
            double maxavg = 0.0; string name = "No Class";
            List<ChartModel> model = new List<ChartModel>();
            try
            {
                string query = "select * , round(((cast(x.totalpassed as FLOAT) / cast(x.studentappeared as FLOAT)) * 100), 2) as 'Class Average' from " +
                               "(select sc.resulttype, sc.subresltyp, count(stdid) as 'studentappeared', sc.campusid, sc.classid, scl.classtxt " +
                               ", count(case when percentage >= '49.75' then 1 end) as 'totalpassed' from schresult as sc " +
                               "inner join schresltype as r on sc.resulttype = r.reslttyp and sc.subresltyp = r.subresltyp " +
                               "inner join schclass as scl on sc.classid = scl.classid and sc.campusid = scl.campusid " +
                               "inner join Schclassgroup as scg on scg.classid = sc.classid where sc.delind <> 'X' and sc.subjectid = '" + subjectid + "' and sc.teacherid = '" + userid + "' " +
                               "and sc.subresltyp = '" + moduleid + "' and scg.classgroup = '01' and sc.campusid = '" + campusid + "' " +
                               "group by sc.resulttype, sc.subresltyp, sc.campusid, sc.classid, scl.classtxt) as x ";

                string query2 = "select * , round(((cast(x.totalpassed as FLOAT) / cast(x.studentappeared as FLOAT)) * 100), 2) as 'Class Average' from " +
                  "(select sc.resulttype, sc.subresltyp, count(stdid) as 'studentappeared', sc.campusid, sc.classid, scl.classtxt " +
                  ", count(case when percentage >= '44.75' then 1 end) as 'totalpassed' from schresult as sc " +
                  "inner join schresltype as r on sc.resulttype = r.reslttyp and sc.subresltyp = r.subresltyp " +
                  "inner join schclass as scl on sc.classid = scl.classid and sc.campusid = scl.campusid " +
                  "inner join Schclassgroup as scg on scg.classid = sc.classid where sc.delind <> 'X'and sc.subjectid = '" + subjectid + "' and sc.teacherid = '" + userid + "' " +
                  "and sc.subresltyp = '" + moduleid + "' and scg.classgroup = '02' and sc.campusid = '" + campusid + "' " +
                  "group by sc.resulttype, sc.subresltyp, sc.campusid, sc.classid, scl.classtxt) as x ";

                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    avgcam = true;
                    while (da.obj_reader.Read())
                    {
                        if (Convert.ToDouble(da.obj_reader["Class Average"]) > maxavg)
                        {
                            maxavg = Convert.ToDouble(da.obj_reader["Class Average"]);
                            name = da.obj_reader["classtxt"].ToString();
                        }
                        model.Add(new ChartModel()
                        {
                            ClassName = da.obj_reader["classtxt"].ToString(),
                            averagecam = da.obj_reader["Class Average"].ToString(),
                            maxavg = "" + maxavg.ToString(),
                            highclass = name
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }

                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query2);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    avgmat = true;
                    while (da.obj_reader.Read())
                    {
                        if (Convert.ToDouble(da.obj_reader["Class Average"]) > maxavg)
                        {
                            maxavg = Convert.ToDouble(da.obj_reader["Class Average"]);
                            name = da.obj_reader["classtxt"].ToString();
                        }
                        model.Add(new ChartModel()
                        {
                            ClassNameMat = da.obj_reader["classtxt"].ToString(),
                            averagemat = da.obj_reader["Class Average"].ToString(),
                            maxavg = "" + maxavg.ToString(),
                            highclass = name
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    model.Add(new ChartModel()
                    {
                        maxavg = "" + maxavg.ToString(),
                        highclass = name
                    });
                }
                model.Add(new ChartModel()
                {
                    avgcheckcam = avgcam,
                    avgcheckmat = avgmat
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCATA. Error Details: " + ex.Message);
            }
            return model;
        }

        [HandleError]
        public List<SelectListItem> FillFinalReports(string campusId, string classId, string sectionId, string moduleId)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                da.CreateConnection();
                string query = "select cd.reptid, cd.reptname from crydtl cd " +
                               "inner join cryhdr ch on cd.reptid = ch.reptid " +
                               "inner join zrptassignment z on ch.reptid = z.reptid " +
                               "where ch.moduleid = 'SCHL' and z.campusid = '" + campusId + "' and z.classid = '" + classId + "' " +
                               "and z.sectionid = '" + sectionId + "' and z.subresltyp = '" + moduleId + "'";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["reptname"].ToString(),
                            Value = da.obj_reader["reptid"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FFR. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<Data.PublishResultClass> getpublishresult(string campusid, string classid, string moduleid)
        {
            List<Data.PublishResultClass> items = new List<Data.PublishResultClass>();
            try
            {
                da.CreateConnection();
                string query = "select distinct sc.campusid, sc.campustxt, scl.classid, scl.classtxt, rp.credate, " +
                               "ss.sectionid, ss.sectiontxt, sr.begdate from Schresult sr " +
                               "inner join schcampus sc on sc.campusid = sr.campusid " +
                               "inner join schclass scl on scl.classid = sr.classid " +
                               "inner join schsection ss on ss.sectionid = sr.sectionid " +
                               "inner join schsubject ssub on ssub.subjectid = sr.subjectid " +
                               "left join schrsltpb rp on sr.campusid = rp.campusid and sr.classid = rp.classid " +
                               "and sr.sectionid = rp.sectionid and sr.subresltyp = rp.subresultype and sr.delind = rp.delind " +
                               "where sr.campusid = '" + campusid + "' and sr.subresltyp = '" + moduleid + "' and sr.delind <> 'X' " +
                               "order by classid ASC, sectionid ASC ";
                string prequery = "select distinct sc.campusid, sc.campustxt, scl.classid, scl.classtxt, rp.credate , " +
                                  "ss.sectionid, ss.sectiontxt, pr.begdate from schpreresult pr " +
                                  "left join schcampus sc on pr.campusid = sc.campusid " +
                                  "left join schclass scl on pr.classid = scl.classid " +
                                  "left join schsection ss on pr.sectionid = ss.sectionid " +
                                  "left join schsubject ssub on pr.subjectid = ssub.subjectid " +
                                  "left join schrsltpb rp on pr.campusid = rp.campusid and pr.classid = rp.classid and pr.sectionid = rp.sectionid " +
                                  "and pr.subresltyp = rp.subresultype and pr.delind = rp.delind " +
                                  "where sc.campusid = '" + campusid + "' and pr.subresltyp = '" + moduleid + "' and pr.delind <> 'X' order by pr.begdate";

                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        string status = "rnp";
                        string date = Convert.ToString(da.obj_reader["credate"]) ?? "";
                        if (date != "")
                        {
                            status = "rp";
                        }
                        items.Add(new Data.PublishResultClass
                        {
                            classid = da.obj_reader["classid"].ToString().Trim(),
                            classes = da.obj_reader["classtxt"].ToString(),
                            sectionid = da.obj_reader["sectionid"].ToString().Trim(),
                            section = da.obj_reader["sectiontxt"].ToString(),
                            begdate = da.obj_reader["begdate"].ToString(),
                            status = status
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return items;
                }
                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, prequery);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        string status = "rnp";
                        string date = Convert.ToString(da.obj_reader["credate"]) ?? "";
                        if (date != "")
                        {
                            status = "rp";
                        }
                        items.Add(new Data.PublishResultClass
                        {
                            classid = da.obj_reader["classid"].ToString().Trim(),
                            classes = da.obj_reader["classtxt"].ToString(),
                            sectionid = da.obj_reader["sectionid"].ToString().Trim(),
                            section = da.obj_reader["sectiontxt"].ToString(),
                            begdate = da.obj_reader["begdate"].ToString(),
                            status = status
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-GBR. Error Details: " + ex.Message);
            }
            var asc = from m in items orderby m.begdate descending select m;
            items = asc.ToList<Data.PublishResultClass>();
            return items;
        }

        public List<SelectListItem> getemployeename(string userid)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                da.CreateConnection();
                string query = "select ep.firstname + ep.lastname as empname from emppers ep where empid = '"+userid+"' and delind <> 'X'";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        list.Add(new SelectListItem
                        {
                            Text = da.obj_reader["empname"].ToString(),
                            Value = userid.Trim()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
            }
            catch (Exception ex)
            {

            }
            return list;
        }

        [HandleError]
        public List<ESSModel> getemploydetail(string userid)
        {
            List<ESSModel> items = new List<ESSModel>();
            try
            {
                da.CreateConnection();

                string query = "select ep.empid, e11.begdate, esub.eesubgrp, esub.eesubgrptxt, epos.pos, epos.postxt " +
                                "from empmain as ep " +
                                "inner join emporg as eorg on ep.empid = eorg.empid " +
                                "inner join eposhdr as epos on eorg.pos = epos.pos " +
                                "inner join eesubgrp as esub on ep.eesubgrp = esub.eesubgrp " +
                                "inner join emp0011 as e11 on ep.empid = e11.empid " +
                                "where ep.empid = '" + userid + "'";

                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {

                        items.Add(new ESSModel
                        {
                            empid = da.obj_reader["empid"].ToString().Trim(),
                            design = da.obj_reader["postxt"].ToString(),
                            dept = da.obj_reader["eesubgrptxt"].ToString().Trim(),
                            joindate = Convert.ToDateTime(da.obj_reader["begdate"].ToString()).ToString("dd/MMMM/yyyy"),
                            confrdate = Convert.ToDateTime(da.obj_reader["begdate"].ToString()).ToString("dd/MMMM/yyyy")
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                return items;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-GBR. Error Details: " + ex.Message);
            }
        }

        [HandleError]
        public List<SelectListItem> getLoanType(string pagetype)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                string query = "SELECT pagetype,subpagtype,subtypetxt " +
                "FROM subpagtype where pagetype = '"+ pagetype + "'";
                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["subtypetxt"].ToString(),
                            Value = da.obj_reader["subpagtype"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                }
                else
                {
                    da.obj_reader.Close();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCmp. Error Details: " + ex.Message);
            }
            return items;
        }

        public List<SelectListItem> getRecodType()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                int count = 0;
                for (int i = 2; i < 4; i++)
                {
                    string query = "select distinct eap.empid, eap.apprtxt +' - '+  ep.firstname + ' ' + ep.lastname as empname " +
                    "from empaprvr eap inner join emppers ep on eap.empid = ep.empid inner join emp0430 e430 on eap.empid = e430.recod" + i;
                    da.CreateConnection();
                    da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                    da.OpenConnection();
                    da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                    if (da.obj_reader.HasRows)
                    {
                        while (da.obj_reader.Read())
                        {
                            if (count == 0)
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = da.obj_reader["empname"].ToString(),
                                    Value = da.obj_reader["empid"].ToString().Trim()
                                });
                            }
                            else
                            {
                                var lst = items.FirstOrDefault(cus => cus.Value == da.obj_reader["empid"].ToString().Trim());
                                if (lst == null)
                                {
                                    items.Add(new SelectListItem
                                    {
                                        Text = da.obj_reader["empname"].ToString(),
                                        Value = da.obj_reader["empid"].ToString().Trim()
                                    });
                                }
                            }
                            count++;
                        }
                    }
                    da.obj_reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCmp. Error Details: " + ex.Message);
            }
            return items;
        }

        public List<ESSModel> getLoanDetails(string empid)
        {
            List<ESSModel> items = new List<ESSModel>();
            try
            {
                List<SelectListItem> sl = new List<SelectListItem>();
                string query = "select ep.empid, e77.recordno, epers.firstname + ' ' + epers.lastname as empname, " +
                "e11.begdate as joindate, epos.postxt, subpag.subpagtype, subpag.subtypetxt, e77.paymamt, e77.begdate, e77.enddate " +
                "from empmain as ep inner join emp0377 as e77 on ep.empid = e77.empid " +
                "inner join emppers as epers on e77.empid = epers.empid " +
                "inner join emporg as eorg on ep.empid = eorg.empid " +
                "inner join eposhdr as epos on eorg.pos = epos.pos " +
                "inner join eesubgrp as esub on ep.eesubgrp = esub.eesubgrp " +
                "inner join emp0011 as e11 on ep.empid = e11.empid " +
                "inner join empreqtype as reqstat on e77.loanreq = reqstat.reqtype " +
                "inner join subpagtype as subpag on e77.subpagtype = subpag.subpagtype " +
                "where ep.empid = '" + empid + "' and subpag.pagetype = '0360' " +
                "and e77.recordno = (select max(recordno) from emp0377 where reqstat <> '03' and empid = '" + empid + "')";
                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {

                    while (da.obj_reader.Read())
                    {
                        sl.Add(new SelectListItem
                        {
                            Text = da.obj_reader["subtypetxt"].ToString().Trim(),
                            Value = da.obj_reader["subpagtype"].ToString().Trim()
                        });
                        items.Add(new ESSModel
                        {
                            empid = da.obj_reader["empid"].ToString().Trim(),
                            recordno = da.obj_reader["recordno"].ToString().Trim(),
                            empname = da.obj_reader["empname"].ToString().Trim(),
                            joindate = Convert.ToDateTime(da.obj_reader["joindate"].ToString().Trim()).ToString("dd/MMMM/yyyy"),
                            design = da.obj_reader["postxt"].ToString().Trim(),
                            loanid = da.obj_reader["subtypetxt"].ToString().Trim(),
                            loanamt = da.obj_reader["paymamt"].ToString().Trim(),
                            begdate = Convert.ToDateTime(da.obj_reader["begdate"].ToString().Trim()).ToString("dd/MMMM/yyyy"),
                            lastdate = Convert.ToDateTime(da.obj_reader["enddate"].ToString().Trim()).ToString("dd/MMMM/yyyy"),
                            loantyp = sl
                        });
                    }
                    da.obj_reader.Close();
                }
                else
                {
                    da.obj_reader.Close();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCmp. Error Details: " + ex.Message);
            }
            return items;
        }

        public List<SelectListItem> getApprRecodType(string empId, string reqtype, string user_id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                for (int i = 1; i < 8; i++)
                {
                    string query = "select distinct app.empid, app.apprtxt +' - '+  ep.firstname + ' ' + ep.lastname as empname, " +
                    "e4.recod2, e4.recod3 from emp0430 e4 " +
                    "inner join empaprvr app on app.empid = e4.apprvl" + i +
                    " inner join emppers ep on ep.empid = app.empid " +
                    "where e4.empid = '" + empId + "' and e4.reqtype = '" + reqtype + "'";
                    da.CreateConnection();
                    da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                    da.OpenConnection();
                    da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                    if (da.obj_reader.HasRows)
                    {
                        while (da.obj_reader.Read())
                        {
                            if (da.obj_reader["empid"].ToString().Trim() != "" && (user_id == da.obj_reader["recod2"].ToString().Trim() || user_id == da.obj_reader["recod3"].ToString().Trim()))
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = da.obj_reader["empname"].ToString(),
                                    Value = da.obj_reader["empid"].ToString().Trim()
                                });
                            }
                        }
                        da.obj_reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCmp. Error Details: " + ex.Message);
            }
            return items;
        }

        public List<ESSModel> track_loan_status(string userid)
        {
            List<ESSModel> list = new List<ESSModel>();
            try
            {
                string query = "select e7.empid, e8.approver, e8.apprstat, e8.apprcoment from emp0377 e7 " +
                                "inner join emp0378 e8 on e7.empid = e8.empid and e7.recordno = e8.requestno " +
                                "where e7.empid = '" + userid + "' and e7.loanreq = '10' and e7.delind <> 'X' and " +
                                "e7.upduser <> '' and e8.reqtype = '10' and e8.appractid = '20' order by e7.recordno desc";
                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        list.Add(new ESSModel
                        {
                            loan_status = da.obj_reader["apprstat"].ToString().Trim(),
                            appvrid = da.obj_reader["approver"].ToString().Trim(),
                            comments = da.obj_reader["apprcoment"].ToString()
                        });
                    }
                    da.obj_reader.Close();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-loantrack. Error Details: " + ex.Message);
            }
            return list;
        }

        /************************************Leave Approval ******************************************/
        public List<ESSModel> getLeaveDetails(string empid)
        {
            List<ESSModel> items = new List<ESSModel>();
            try
            {
                List<SelectListItem> sl = new List<SelectListItem>();
                string query = "select ep.empid, e77.recordno, epers.firstname + ' ' + epers.lastname as empname, "+
                "e11.begdate as joindate,epos.postxt, subpag.subpagtype, subpag.subtypetxt,e77.lvdays, e77.begdate, e77.enddate "+
                "from empmain as ep inner join emp0277 as e77 on ep.empid = e77.empid "+
                "inner join emppers as epers on e77.empid = epers.empid "+
                "inner join emporg as eorg on ep.empid = eorg.empid "+
                "inner join eposhdr as epos on eorg.pos = epos.pos "+
                "inner join eesubgrp as esub on ep.eesubgrp = esub.eesubgrp "+
                "inner join emp0011 as e11 on ep.empid = e11.empid "+
                "inner join subpagtype as subpag on e77.subpagtype = subpag.subpagtype "+
                "where ep.empid = '"+ empid + "' and subpag.pagetype = '0210' "+
                "and e77.recordno = (select max(recordno) from emp0277 where empid = '"+ empid + "')";
                da.CreateConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {

                    while (da.obj_reader.Read())
                    {
                        sl.Add(new SelectListItem
                        {
                            Text = da.obj_reader["subtypetxt"].ToString().Trim(),
                            Value = da.obj_reader["subpagtype"].ToString().Trim()
                        });
                        items.Add(new ESSModel
                        {
                            empid = da.obj_reader["empid"].ToString().Trim(),
                            recordno = da.obj_reader["recordno"].ToString().Trim(),
                            empname = da.obj_reader["empname"].ToString().Trim(),
                            joindate = Convert.ToDateTime(da.obj_reader["joindate"].ToString().Trim()).ToString("dd/MMMM/yyyy"),
                            design = da.obj_reader["postxt"].ToString().Trim(),
                            loanid = da.obj_reader["subtypetxt"].ToString().Trim(),
                            totdays = da.obj_reader["lvdays"].ToString().Trim(),
                            begdate = Convert.ToDateTime(da.obj_reader["begdate"].ToString().Trim()).ToString("dd/MMMM/yyyy"),
                            lastdate = Convert.ToDateTime(da.obj_reader["enddate"].ToString().Trim()).ToString("dd/MMMM/yyyy"),
                            loantyp = sl
                        });
                    }
                    da.obj_reader.Close();
                }
                else
                {
                    da.obj_reader.Close();
                    return items;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCmp. Error Details: " + ex.Message);
            }
            return items;
        }

        /************************************Leave Approval ******************************************/

         public bool Filltimesheet(string empid, string Name , string client, DateTime checkindt )
        {
            DataTable dt = new DataTable();
            da.CreateConnection();
            SqlConnection conn = da.GetCurrentConnection;
            conn.Open();
            string CheckQuery = "select * from emptimesht where empid ='" + empid+"'";
            SqlCommand command = new SqlCommand(CheckQuery, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(CheckQuery, conn);
            adapter.Fill(dt);
            if (dt.Rows.Count>0)
            {

                string updateQuery = "UPDATE emptimesht SET checkoutdt ='" + checkindt + "' WHERE checkindt IN (SELECT MAX(checkindt) AS lastdt FROM emptimesht);";
                command = new SqlCommand(updateQuery, conn);
                int Uchk = command.ExecuteNonQuery();
                if (Uchk > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                try
                {

                    string query = "INSERT INTO emptimesht VALUES('" + empid + "','" + Name + "','" + client + "','" + checkindt + "','1990-01-01')";
                    command = new SqlCommand(query, conn);
                    int i = command.ExecuteNonQuery();
                    if (i > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error Occured: While Processing DBClass-FTN. Error Details: " + ex.Message);
                }
            }
            
        }
        public List<SelectListItem> getClient()
        {

            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                for (int i = 0; i < 8; i++)
                {
                    string query = "SELECT DISTINCT * FROM Clienttb";
                    da.CreateConnection();
                    da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                    da.OpenConnection();
                    da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                    if (da.obj_reader.HasRows)
                    {
                        while (da.obj_reader.Read())
                        {
                            if (da.obj_reader["id"].ToString().Trim() != "")
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = da.obj_reader["Name"].ToString(),
                                    //Value = da.obj_reader["id"].ToString().Trim()
                                });
                            }
                        }
                        da.obj_reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FCmp. Error Details: " + ex.Message);
            }
            return items;
        }

        [HandleError]
        public List<SelectListItem> FillSNSEmployee()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                da.CreateConnection();
                string query = "select distinct ep.empid, ep.firstname + ' ' + ep.midname + ' ' + ep.lastname as 'empname' from emppers as ep " +
                                "where delind <> 'X' "+
                                " order by empname ASC";
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.OpenConnection();
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = da.obj_reader["empname"].ToString(),
                            //Value = da.obj_reader["empid"].ToString().Trim()
                        });
                    }
                    da.obj_reader.Close();
                    da.CloseConnection();
                }
                else
                {
                    da.obj_reader.Close();
                    da.CloseConnection();
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occured: While Processing DBClass-FEmp. Error Details: " + ex.Message);
            }
            return items;
        }

        //public string getempname(string id)
        //{
        //    da.CreateConnection();
        //    da.OpenConnection();
        //    SqlConnection conn = da.GetCurrentConnection;
        //    DataTable dt = new DataTable();
        //    SqlCommand cmd = new SqlCommand("SELECT empname FROM emppers WHERE empid='" + id + "'", conn);
        //    dt.Load(cmd.ExecuteReader());
        //    da.CloseConnection();
        //    return dt.Rows[0][0].ToString();
        //}
    }
}