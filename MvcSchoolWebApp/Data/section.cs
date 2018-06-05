using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSchoolWebApp.Data
{
    public class section
    {
        public string campusid;
        public string classtxt;
        public string classid;
        public string sectionid;
        public string sectiontxt;

        public section(string cid, string clstxt, string clsid, string secid, string sectxt)
        {
            campusid = cid;

            classtxt = clstxt;
            classid = clsid;
            sectionid = secid;
            sectiontxt = sectxt;
        }
    }
}