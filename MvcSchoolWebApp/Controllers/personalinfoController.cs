using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Script.Serialization;
using System.Web.UI;
using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Configuration;

namespace MvcSchoolWebApp.Controllers
{
    public class personalinfoController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string pagename;
        private Database.Database da = new Database.Database("Falconlocal");

        Data.data data = new data();
        private string camp_id;
        private string emp_id;
        public static string[] popupinfo = new string[3];
        public static List<Users> user_dtl;
        public Page Page { get; private set; }

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
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[30].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            Personalinfo cam = new Personalinfo();
            cam.image = "~/Content/Avatar/avatar2.png";
            //cam.campus = db.FillCamp(user_id);
            cam.campus = db.getcampus();
            cam.section = sl;

            if (pagename == "Employee Personal")
                cam.employeeid = db.FillEmployee(cam.campus[0].Value);
            else
            {
                cam.classes = db.getclass(cam.campus[0].Value, user_id);
                cam.employeeid = sl;
            }
            cam.campusid = cam.campus[0].Value;
            return View(cam);
        }
        [HttpPost]
        public ActionResult Index(Personalinfo pro)
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[30].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            List<SelectListItem> sl = new List<SelectListItem>();
            camp_id = pro.campusid;
            emp_id = pro.textbox;

            string query = "";

            if (pro.classId == null && pro.sectionId == null) // Employee
            {
                query = "select distinct emp.empid, emp.earea , e.eareatxt , eesubarea , esub.esubareat , emp.eegroup, " +
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
                    "where emp.empid = '" + emp_id + "' and e18.campusid = '" + pro.campusid + " ' and emp.delind <> 'X' and e18.delind <> 'X' and  info.delind <> 'X' and empadd.delind <> 'X'";
                pro.employeeid = db.FillEmployee(pro.campusid);
                pro.textbox = pro.textbox;
                da.CreateConnection();
                da.OpenConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();

                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        DateTime date = new DateTime();
                        pro.empid = da.obj_reader["empid"].ToString();
                        pro.txtcampus = da.obj_reader["campustxt"].ToString();
                        pro.campusid = camp_id;
                        pro.status = da.obj_reader["eegrouptxt"].ToString();
                        pro.department = da.obj_reader["esubareat"].ToString();
                        pro.birthcountry = da.obj_reader["cobirth"].ToString();
                        pro.careof = da.obj_reader["careof"].ToString();
                        //pro.initial = da.obj_reader["initials"].ToString();
                        date = Convert.ToDateTime(da.obj_reader["birthdate"]);
                        pro.dob = date.ToString("dd/MMMM/yyyy");
                        pro.city = da.obj_reader["city"].ToString();
                        pro.district = da.obj_reader["district"].ToString();
                        pro.zipcode = da.obj_reader["zipcode"].ToString() ?? "";
                        pro.paddress = da.obj_reader["street1"].ToString();
                        pro.paddress2 = da.obj_reader["street2"].ToString();
                        pro.firstname = da.obj_reader["firstname"].ToString();
                        pro.lastname = da.obj_reader["lastname"].ToString();
                        pro.secondname = da.obj_reader["secname"].ToString();
                        pro.middlename = da.obj_reader["midname"].ToString();
                        pro.fathername = da.obj_reader["birthname"].ToString();
                        pro.birthplace = da.obj_reader["birthplace"].ToString();
                        date = Convert.ToDateTime(da.obj_reader["begdate"]);
                        pro.fromdate = date.ToString("dd/MMMM/yyyy");
                        date = Convert.ToDateTime(da.obj_reader["enddate"]);
                        pro.todate = date.ToString("dd/MMMM/yyyy");
                        pro.title = da.obj_reader["formadd"].ToString();
                        pro.gender = da.obj_reader["gender"].ToString();
                        pro.phone = da.obj_reader["phone"].ToString();
                        pro.roletxt = da.obj_reader["eareatxt"].ToString();
                        pro.image = da.obj_reader["imagepath"].ToString();
                        pro.txtaddtype = da.obj_reader["typetxt"].ToString();
                        pro.nic = da.obj_reader["idnumber"].ToString();
                        pro.nationality = da.obj_reader["nationality"].ToString();
                        pro.country = da.obj_reader["cobirth"].ToString();
                        if (da.obj_reader["imagepath"].ToString().Trim() != "")
                            pro.image = da.obj_reader["imagepath"].ToString();
                        else
                            pro.image = "~/Content/Avatar/avatar2.png";
                        ViewBag.scriptCall = "getprofile();";
                    }
                }
                else
                {
                    ViewBag.call_alert = "show_alert()";
                    ViewBag.message_popup = "No Record Found";
                    ViewBag.cssclass = "alert-danger";
                    pro.image = "~/Content/Avatar/256.jpg";
                    da.CloseConnection();
                }
            }
            else                                             // Student
            {
                query = "select distinct info.title, std.stdid, std.stdarea , s.stdareatxt , std.stdsubarea , ssub.stdsubareat , std.stdgroup,  "+
                    "sg.stdgrouptxt , std.begdate ,camp.campustxt, std.enddate, info.firstname, info.midname, info.lastname,  "+
                    "info.birthdate, info.gender, info.birthplace,  info.nationality, img.imagepath, "+
                    "info.street1, info.street2, info.zipcode, info.city, info.district, info.homeph from stdmain as std "+
                    "inner join stdarea as s on std.stdarea = s.stdarea "+
                    "inner join stdsubarea as ssub on std.stdsubarea = ssub.stdsubarea and std.stdarea = ssub.stdarea "+
                    "inner join stdgroup as sg on std.stdgroup = sg.stdgroup inner join stdpers as info on std.stdid = info.stdid "+
                    "inner join std0710 as s18 on std.stdid = s18.stdid inner join schcampus as camp on s18.campusid = camp.campusid "+
                    "left outer join emp0170 as e17 on std.stdid = e17.empid left outer join imageobj as img on e17.imageid = img.imageid "+
                    "where std.stdid = '"+emp_id+"' and s18.campusid = '"+pro.campusid+"' and s18.classid = '"+pro.classId+"' and s18.sectionid = '"+pro.sectionId+"' and std.delind <> 'X' and s18.delind <> 'X' and info.delind <> 'X'" ;
                da.CreateConnection();
                da.OpenConnection();
                da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
                da.obj_reader = da.obj_sqlcommand.ExecuteReader();
                if (da.obj_reader.HasRows)
                {
                    while (da.obj_reader.Read())
                    {
                        DateTime date = new DateTime();
                        pro.txtcampus = da.obj_reader["campustxt"].ToString() ?? "-";
                        pro.roleid = Convert.ToInt32(da.obj_reader["stdid"]);
                        pro.status = "Student";
                        pro.department = da.obj_reader["stdgrouptxt"].ToString() ?? "-";
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
                        pro.txtaddtype = "Mailing Address";
                        pro.nic = "-";
                        pro.nationality = da.obj_reader["nationality"].ToString() ?? "-";
                    }
                }
                pro.employeeid = db.getstudentname(pro.campusid, pro.classId, pro.sectionId);
                pro.textbox = pro.textbox;
                ViewBag.scriptCall = "getprofile();";
            }

            

            pro.campus = db.getcampus();
            pro.classes = db.getclass(pro.campus[0].Value, user_id);
            pro.section = db.getsection(pro.campus[0].Value, pro.classes[0].Value, user_id);

            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            return View(pro);
        }

        public JsonResult funcpersonal(string datatosave)
        {
            string url = "";
            try
            {
                if (datatosave == "Student Wise Personal" || datatosave == "Employee Personal")
                {
                    pagename = datatosave;
                    url = "/personalinfo/Index";
                }
                else
                {
                    url = "/personalinfo/ClassWisepersonal";
                }
                Session["personal"] = datatosave;
            }
            catch (Exception)
            {
                Session["personal"] = "Employee Personal";
            }
            return Json(url, JsonRequestBehavior.AllowGet);
        }

        /*[HttpPost]
        public ActionResult uploadimage(HttpPostedFileBase postedFile)
        {
            string empid = "1000000000";
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/Images/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    ViewBag.message = "no path";
                }

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
                con.Open();
                string query1 = "select isnull(max(imageid), 1000000000) from imageobj";
                SqlCommand cmd = new SqlCommand(query1, con);
                int id = (Convert.ToInt32(cmd.ExecuteScalar()) + 1);
                con.Close();

                postedFile.SaveAs(path + id + " - " + Path.GetFileName(postedFile.FileName));

                DatabaseInsertClass dbinsert = new DatabaseInsertClass();
                dbinsert.uploadimage(path, postedFile.FileName, Path.GetExtension(postedFile.FileName), empid, id.ToString());
                ViewBag.message = "file uploaded successfully.";
            }
            else
            {
                ViewBag.message = "no posted file";
            }

            return RedirectToAction("Index", "personalinfo");
        }*/

        public void getpopupmessage(string call_alert, string message_popup, string cssclass)
        {
            popupinfo[0] = call_alert;
            popupinfo[1] = message_popup;
            popupinfo[2] = cssclass;
        }

        public ActionResult Studentpersonal()
        {
            return View();
        }
        public ActionResult Teacherpersonal()
        {
            return View();
        }

        DatabaeseClass db;
        public ActionResult ClassWisepersonal()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[5].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();

            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });

            DatabaseModel classwise = new DatabaseModel();
            db = new DatabaeseClass();
            classwise.campus = db.getcampus();
            classwise.campusid = classwise.campus[0].Value;
            classwise.classes = db.getclass(classwise.campus[0].Value, user_id);
            classwise.section = db.getsection(classwise.campus[0].Value, classwise.classes[0].Value, user_id);

            return View(classwise);
        }

        [HttpPost]
        public JsonResult getSectionJson(String campusId, String classId, string selectsectionId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getsection(campusId, classId, user_id));
        }

        public JsonResult getClassJson(string campusId, string selectCityId = null)
        {
            db = new DatabaeseClass();
            return Json(db.getclass(campusId, user_id));
        }

        public JsonResult getJQGridJson(string campus, string classes, string section)
        {
            db = new DatabaeseClass();
            return Json(db.FillClassWiseGrid(campus, classes, section), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getEmployeeJson(string campusid)
        {
            db = new DatabaeseClass();
            return Json(db.FillEmployee(campusid));
        }

        public JsonResult getstudentname(string campusId, string classId, string sectionId, string selectstudentnameid = null)
        {
            db = new DatabaeseClass();
            return Json(db.getstudentname(campusId, classId, sectionId));
        }
    }

}