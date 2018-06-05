using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSchoolWebApp.Models
{
    public class EventSchedular
    {
        public int EventID { get; set; }


        public string Subject { get; set; }

        public string Description { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public string ThemeColor { get; set; }

        public bool IsFullDay { get; set; }

        public string status { get; set; }

    }
}