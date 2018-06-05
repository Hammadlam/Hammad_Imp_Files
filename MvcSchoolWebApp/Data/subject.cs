using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSchoolWebApp.Data
{
    public class subject
    {
        public string campus_id;
        public string class_id;
        public string class_txt;
        public string section_id;
        public string section_txt;
        public string subject_id;
        public string subject_txt;


        public subject(string cid, string clsid, string clstxt, string secid, string sectxt, string subid, string subtxt)
        {
            campus_id = cid;
            class_id = clsid;
            class_txt = clstxt;
            section_id = secid;
            section_txt = sectxt;
            subject_id = subid;
            subject_txt = subtxt;
        }
    }
}