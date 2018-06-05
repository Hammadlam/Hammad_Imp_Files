using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSchoolWebApp.Data
{
    public class campus
    {
        public string campusid;
        public string campusname;

        public campus(string cid, string cname)
        {
            campusid = cid;

            campusname = cname;

        }
    }
}