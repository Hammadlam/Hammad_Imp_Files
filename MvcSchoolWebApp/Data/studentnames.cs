using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSchoolWebApp.Data
{
    public class studentnames
    {
        public string campus_id;
        public string class_id;
        public string class_txt;
        public string section_id;
        public string section_txt;
        public string student_id;
        public string student_txt;


        public studentnames(string cid, string clsid, string clstxt, string secid, string sectxt, string stdid, string stdtxt)
        {
            campus_id = cid;
            class_id = clsid;
            class_txt = clstxt;
            section_id = secid;
            section_txt = sectxt;
            student_id = stdid;
            student_txt = stdtxt;
        }
    }
}