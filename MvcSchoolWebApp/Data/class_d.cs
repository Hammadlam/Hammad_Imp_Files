using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSchoolWebApp.Data
{
    public class class_d
    {
        public string campusid;
        public string classid;
        public string classtxt;



        public class_d(string cid, string clsid, string ctxt)
        {
            campusid = cid;
            classid = clsid;
            classtxt = ctxt;
        }
    }
}