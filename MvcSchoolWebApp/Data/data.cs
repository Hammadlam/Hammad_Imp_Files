using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using MvcSchoolWebApp.Models;
using MvcSchoolWebApp.Controllers;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Data
{
    public class data
    {
        public static string user_id;
        public static string user_earea;
        public static string user_fname;
        public static string user_lname;
        public static string user_profimg;
        public static string user_role;
        public static string user_fullname;
        public static List<SelectListItem> campuses;
        public static List<SelectListItem> classes;
        public static List<SelectListItem> sections;
        public static List<SelectListItem> subjects;
        public static List<Users> user_dtl = new List<Users>();

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString);
        public Page Page { get; private set; }
        private Database.Database da = new Database.Database("Falconlocal");


        [HandleError]
        public List<LoginModel> validatelogin(string username, string password)
        {
            List<LoginModel> item = new List<LoginModel>();
            
            bool status = false;
            da.CreateConnection();
            string query = "select usr.userid, usr.passwd, usr.fname, usr.lname, usr.acdtitle, usr.menuprof, " +
                           "img.imagepath, mpd.menustat, mpd.menulabel, mpd.menuid, mpd.tcode, std.stdareatxt from usr01 usr "+
                           "inner join menuprofdtl mpd on usr.menuprof = mpd.menuprof "+
                           "left join imageobj img on usr.userid = img.imageid "+
                           "inner join stdarea std on usr.acdtitle = std.stdarea "+
                           "where userid = '" + username + "' order by mpd.menuid";
            da.InitializeSQLCommandObject(da.GetCurrentConnection, query);
            da.OpenConnection();
            da.obj_reader = da.obj_sqlcommand.ExecuteReader();
            if (da.obj_reader.HasRows)
            {
                while (da.obj_reader.Read())
                {
                    string pass = da.obj_reader["passwd"].ToString();
                    if (pass == password)
                    {
                        user_dtl.Add(new Users
                        {
                            user_id = da.obj_reader["userid"].ToString().Trim(),
                            user_earea = da.obj_reader["acdtitle"].ToString().Trim(),
                            user_fullname = da.obj_reader["fname"].ToString() + " " + da.obj_reader["lname"].ToString(),
                            user_image = da.obj_reader["imagepath"].ToString(),
                            user_role = da.obj_reader["stdareatxt"].ToString()
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
                        return item;
                    }
                }
            }
            da.CloseConnection();
            return item;
        }
    }
}