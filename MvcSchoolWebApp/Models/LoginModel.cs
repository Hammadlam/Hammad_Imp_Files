using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSchoolWebApp.Models
{
    public class LoginModel
    {
        public bool loginstatus { get; set; }
        public string menuprof { get; set; }
        public string menuid { get; set; }
        public string tcode { get; set; }
        public string menustat { get; set; }
        public string earea { get; set; }
    }
}