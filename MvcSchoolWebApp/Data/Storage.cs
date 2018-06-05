using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Data
{
    public class Storage
    {
        static public string role;

        public static List<campus> get_camlist = new List<campus>();
        public static List<class_d> get_clist = new List<class_d>();
        public static List<section> get_seclist = new List<section>();
        public static List<subject> get_sublist = new List<subject>();
        public static List<studentnames> get_stdlist = new List<studentnames>();

        static public List<campus> cam_list = new List<campus>();
        static public List<class_d> class_list = new List<class_d>();
        static public List<section> sec_list = new List<section>();
        static public List<subject> sub_list = new List<subject>();
        static public List<studentnames> std_list = new List<studentnames>();



        public static List<SelectListItem> user_campuses = new List<SelectListItem>();
        public static List<SelectListItem> user_classes = new List<SelectListItem>();
        public static List<SelectListItem> user_sections = new List<SelectListItem>();
        public static List<SelectListItem> user_subjects = new List<SelectListItem>();

        public void ConvertLists()
        {
            for (int i = 0; i < cam_list.Count; i++)
            {
                user_campuses.Add(new SelectListItem
                {
                    Value = cam_list[i].campusid,
                    Text = cam_list[i].campusname
                });
            }

            for (int i =0; i < class_list.Count; i++)
            {
                user_classes.Add(new SelectListItem
                {
                    Value = class_list[i].classid,
                    Text = class_list[i].classtxt
                });
            }

            for (int i = 0; i < sec_list.Count; i++)
            {
                user_sections.Add(new SelectListItem
                {
                    Value = sec_list[i].sectionid,
                    Text = sec_list[i].sectiontxt
                });
            }

            for (int i = 0; i < sub_list.Count; i++)
            {
                user_subjects.Add(new SelectListItem
                {
                    Value = sub_list[i].subject_id,
                    Text = sub_list[i].subject_txt
                });
            }
        }

        public void fill_list(List<campus> cam)
        {
            cam_list = cam;
        }

        public List<campus> get_cam_list()
        {
            return cam_list;
        }

        //class/level

        public void fill_class_list(List<class_d> cam)
        {


            for (int i = 0; i < cam.Count; i++)
            {
                class_list.Add(cam.ElementAt(i));
            }

        }

        public List<class_d> get_class_list()
        {
            return class_list;
        }

        public void set_role(string r)
        {
            role = r;
        }

        public string get_role()
        {
            return role;
        }

        public void fill_sec_list(List<section> cam)
        {
            for (int i = 0; i < cam.Count; i++)
            {
                sec_list.Add(cam.ElementAt(i));
            }
        }

        public List<section> get_sec_list()
        {
            return sec_list;
        }


        public void fill_sub_list(List<subject> cam)
        {


            for (int i = 0; i < cam.Count; i++)
            {
                sub_list.Add(cam.ElementAt(i));
            }

        }

        public void fill_std_list(List<studentnames> cam)
        {
            for (int i = 0; i < cam.Count; i++)
            {
                std_list.Add(cam.ElementAt(i));
            }
        }

        public List<subject> get_sub_list()
        {
            return sub_list;
        }

        public static void clear_list()
        {
            cam_list.Clear();
            class_list.Clear();
            sec_list.Clear();
            sub_list.Clear();
        }
    }
}