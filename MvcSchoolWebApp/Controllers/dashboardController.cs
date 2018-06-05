using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Controllers
{
    public class dashboardController : Controller
    {
        MessageCls msgobj = new MessageCls();
        DatabaeseClass db;
        string cs = ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString.ToString();
        private Database.Database da = new Database.Database("Falconlocal");
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_moudule;
        public static string user_section;
        public static string user_subject;
        public static string user_subjectcamp;
        public static List<Users> user_dtl;


        Storage st = new Storage();
        List<campus> user = new List<campus>();
        List<class_d> usr_class = new List<class_d>();
        List<class_d> get_clist = new List<class_d>();
        List<section> usr_section = new List<section>();
        List<subject> usr_subject = new List<subject>();
        List<studentnames> usrstdnames = new List<studentnames>();

        public static List<SelectListItem> user_campuses = new List<SelectListItem>();
        public static List<SelectListItem> user_classes = new List<SelectListItem>();
        public static List<SelectListItem> user_sections = new List<SelectListItem>();
        public static List<SelectListItem> user_subjects = new List<SelectListItem>();

        // GET: dashboard

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

        public ActionResult Student()
        {
            return RedirectToAction("StudentDashboard", "dashboard");
        }
        public ActionResult others()
        {
            return RedirectToAction("Index", "dashboard");
        }

        //[HttpPost]
        //public JsonResult user_timezone(String timezoneid)
        //{
        //    HttpContext.Session["usr_timezone"] = timezoneid;
        //    return Json(timezoneid, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult Index()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            List<campus> lst = (List<campus>)HttpContext.Session["ssn_usr_campus"];

            if (user_dtl[0].user_earea == "4000" || user_dtl[0].user_earea == "5000")
            {
                return RedirectToAction("StudentDashboard", "dashboard");
            }
            else if (user_dtl[0].user_earea == "3000")
            {
                return RedirectToAction("TeacherDashboard", "dashboard");
            }
            else if (user_dtl[0].user_earea == "1000")
            {
                return RedirectToAction("MasterDashboard", "dashboard");
            }

            //Fill_usrdtl();

            db = new DatabaeseClass();
            DatabaseModel adminModel = new DatabaseModel();
            //adminModel.campus = db.FillCamp(user_dtl[0].user_id);
            adminModel.campus = db.getcampus();
            adminModel.campusid = adminModel.campus[0].Value;

            adminModel.module = db.FillSubModule();
            user_moudule = adminModel.module[0].Value;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "select count(distinct sm.stdid) as totalstd from stdmain sm inner join std0710 s71 on sm.stdid = s71.stdid where sm.delind <> 'X' and s71.delind <> 'X' and stdarea = '5000' and s71.campusid = '" + adminModel.campus[0].Value + "'";
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                ViewBag.TotalStidents = cmd.ExecuteScalar().ToString();
            }
            st.ConvertLists();
            return View(adminModel);
        }

        public ActionResult StudentDashboard()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            //user_role = Session["User_Role"].ToString();
            if (user_dtl[0].user_earea == "2000")
            {
                return RedirectToAction("Index", "dashboard");
            }
            else if (user_dtl[0].user_earea == "3000")
            {
                return RedirectToAction("TeacherDashboard", "dashboard");
            }
            else if (user_dtl[0].user_earea == "1000")
            {
                return RedirectToAction("MasterDashboard", "dashboard");
            }

            db = new DatabaeseClass();
            DatabaseModel stdModel = new DatabaseModel();
            stdModel.campus = db.getcampus();
            user_campus = stdModel.campus[0].Value;

            stdModel.classes = db.getclass(user_campus, user_dtl[0].user_id);
            user_class = stdModel.classes[0].Value;

            stdModel.section = db.getsection(user_campus, user_class, user_dtl[0].user_id);
            user_section = stdModel.section[0].Value;

            stdModel.module = db.FillSubModule();
            user_moudule = stdModel.module[0].Value;

            stdModel.student = db.getstudentname(stdModel.campus[0].Value, stdModel.classes[0].Value, user_section);

            return View(stdModel);
        }

        public ActionResult MasterDashboard()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            //user_role = Session["User_Role"].ToString();
            if (user_dtl[0].user_earea == "2000")
            {
                return RedirectToAction("Index", "dashboard");
            }
            else if (user_dtl[0].user_earea == "3000")
            {
                return RedirectToAction("TeacherDashboard", "dashboard");
            }
            else if (user_dtl[0].user_earea == "4000" || user_dtl[0].user_earea == "5000")
            {
                return RedirectToAction("StudentDashboard", "dashboard");
            }

            db = new DatabaeseClass();
            DatabaseModel adminModel = new DatabaseModel();
            //adminModel.campus = db.FillCamp(user_dtl[0].user_id);
            adminModel.campus = db.getcampus();
            adminModel.campusid = adminModel.campus[0].Value;

            adminModel.module = db.FillSubModule();
            user_moudule = adminModel.module[0].Value;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "select count(distinct stdid) as totalstd from stdmain where delind <> 'X' and stdarea = '5000' ";
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                ViewBag.TotalStidents = cmd.ExecuteScalar().ToString();
            }
            st.ConvertLists();
            return View(adminModel);
        }
        public ActionResult TeacherDashboard()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            if (user_dtl[0].user_earea == "4000" || user_dtl[0].user_earea == "5000")
            {
                return RedirectToAction("StudentDashboard", "dashboard");
            }
            else if (user_dtl[0].user_earea == "2000" )
            {
                return RedirectToAction("index", "dashboard");
            }
            else if (user_dtl[0].user_earea == "1000")
            {
                return RedirectToAction("MasterDashboard", "dashboard");
            }

            db = new DatabaeseClass();
            DatabaseModel adminModel = new DatabaseModel();

            adminModel.campus = db.getcampus();
            user_campus = adminModel.campus[0].Value;

            adminModel.classes = db.getclass(adminModel.campus[0].Value, user_dtl[0].user_id);
            user_class = adminModel.classes[0].Value;

            adminModel.section = db.getsection(adminModel.campus[0].Value, adminModel.classes[0].Value, user_dtl[0].user_id);
            user_section = adminModel.section[0].Value;

            adminModel.subjectcamp = adminModel.subject = db.getsubject(adminModel.campus[0].Value, adminModel.classes[0].Value, adminModel.section[0].Value, user_dtl[0].user_id);
            user_subjectcamp = user_subject = adminModel.subject[0].Value;

            adminModel.module = db.FillSubModule();
            user_moudule = adminModel.module[0].Value;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "select count(distinct sm.stdid) as totalstd from stdmain sm inner join std0710 s71 on sm.stdid = s71.stdid where sm.delind <> 'X' and s71.delind <> 'X' and stdarea = '5000' and s71.campusid = '" + adminModel.campus[0].Value + "'";
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                ViewBag.TotalStidents = cmd.ExecuteScalar().ToString();
            }
            return View(adminModel);
        }

        [HandleError]
        public ActionResult destroy_session()
        {
            try
            {
                Session.Clear();
                user_dtl.Clear();
                Data.data.user_dtl.Clear();
                return RedirectToAction("index", "login");
            }
            catch (Exception ex)
            {
                throw new Exception("");
            }
        }

        public JsonResult getModuleJson(string classid)
        {
            db = new DatabaeseClass();
            return Json(db.FillSubModule());
        }

        public JsonResult getStudentNameJson(string classid)
        {
            db = new DatabaeseClass();
            if (user_dtl[0].user_earea == "4000")
            {
                user_section = db.getsection(user_campus, classid, user_id)[0].Value;
                return Json(db.getstudentname(user_campus, classid, user_section));
            }
            return null;
        }

        public JsonResult getAverageTestJson(string campusid, string moduleid)
        {
            if (user_dtl[0].user_earea != "1000" && user_dtl[0].user_earea != "2000")
                return null;
            else
            {
                db = new DatabaeseClass();
                if ((campusid == "" || campusid == null) && (moduleid == "" || moduleid == null))
                {
                    List<campus> lst = (List<campus>)System.Web.HttpContext.Current.Session["ssn_usr_campus"];
                    campusid = lst[0].campusid;
                    moduleid = user_moudule;
                }
                return Json(db.FillCamAvergeTestAdmin(campusid, moduleid), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCount()
        {
            if (user_dtl[0].user_earea != "1000" && user_dtl[0].user_earea != "2000")
                return null;
            else
            {
                List<ChartModel> model = new List<ChartModel>();
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string query = "select s71.campusid, sc.campustxt, count(s71.stdid) as 'students'," +
                                   "(select count(stdid) from std0710 where   delind<>'X') " +
                                   "as totalstd from std0710 as s71 inner join Schcampus as sc on s71.campusid = sc.campusid " +
                                   "where s71.delind <> 'X' group by s71.campusid, sc.campustxt order by s71.campusid";

                    SqlDataAdapter sda = new SqlDataAdapter(query, con);

                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        //ViewBag.TotalStidents = ds.Tables[0].Rows[0]["totalstd"].ToString();

                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            model.Add(new ChartModel()
                            {
                                ClassName = dr["campustxt"].ToString(),
                                Strength = dr["students"].ToString(),

                            });
                        }
                    }
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAveragGraph()
        {
            db = new DatabaeseClass();
            DatabaseModel adminModel = new DatabaseModel();
            List<ChartModel> model = new List<ChartModel>();
            adminModel.campus = db.FillCamp(user_dtl[0].user_id);
            adminModel.campusid = adminModel.campus[0].Value;
            adminModel.module = db.FillSubModule();
            adminModel.moduleid = adminModel.module[0].Value;
            return Json(db.FillCamAvergeTestAdmin(adminModel.campusid, adminModel.moduleid), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getStudentTestJson(string classid, string studentid, string moduleid)
        {
            db = new DatabaeseClass();
            List<SelectListItem> stdname;
            if (user_dtl[0].user_earea == "4000")
            {
                if ((classid == "" || classid == null) && (moduleid == "" || moduleid == null))
                {
                    user_section = db.getsection(user_campus, user_class, user_id)[0].Value;
                    stdname = db.getstudentname(user_campus, user_class, user_section);
                    studentid = stdname[0].Value;
                }
            }

            if ((classid == "" || classid == null) && (moduleid == "" || moduleid == null))
            {
                classid = user_class;
                moduleid = user_moudule;
                
            }
            return Json(db.FillStudentTestChart(classid, studentid, moduleid, user_id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssisment(string classid, string studentid, string moduleid)
        {
            db = new DatabaeseClass();
            List<SelectListItem> stdname;
            if (user_dtl[0].user_earea == "4000")
            {
                if ((classid == "" || classid == null) && (moduleid == "" || moduleid == null))
                {
                    user_section = db.getsection(user_campus, user_class, user_id)[0].Value;
                    stdname = db.getstudentname(user_campus, user_class, user_section);
                    studentid = stdname[0].Value;
                }
            }

            if ((classid == "" || classid == null) && (moduleid == "" || moduleid == null))
            {
                classid = user_class;
                moduleid = user_moudule;

            }
            return Json(db.GetAssisChart(classid, studentid, moduleid, user_id), JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public JsonResult getSubjectJson(string campusId, string classId, string sectionId, string teacherId, string selectsubjectId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsubject(campusId, classId, sectionId, user_id));
        }

        public JsonResult getClasswiseTestJson(string campusid, string classid, string sectionid, string subjectid, string moduleid)
        {
            if (user_dtl[0].user_earea != "3000")
                return null;
            else
            {
                if (campusid == null || classid == null || sectionid == null || subjectid == null || moduleid == null)
                {
                    campusid = user_campus;
                    classid = user_class;
                    sectionid = user_section;
                    subjectid = user_subject;
                    moduleid = user_moudule;
                }
                db = new DatabaeseClass();
                return Json(db.FillClasswiseChart(campusid, classid, sectionid, subjectid, moduleid), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getOverallTestJson(string campusid, string subjectid, string moduleid)
        {
            if (user_dtl[0].user_earea != "3000")
                return null;
            else
            {
                if (campusid == null || subjectid == null || moduleid == null)
                {
                    campusid = user_campus;
                    subjectid = user_subject;
                    moduleid = user_moudule;
                }
                db = new DatabaeseClass();
                return Json(db.FillOverAllTestChart(campusid, subjectid, moduleid, user_id), JsonRequestBehavior.AllowGet);
            }
        }
    }
}