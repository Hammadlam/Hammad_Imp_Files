using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcSchoolWebApp.Models
{
    public class ChartModel
    {
        public string ClassName  { get; set; }
        public string Strength { get; set; }
        public string averagecam { get; set; }
        public string averagemat { get; set; }
        public string ClassNameMat { get; set; }
        public Boolean avgcheckcam { get; set; }
        public Boolean avgcheckmat { get; set; }
        public string subjectname { get; set; }
        public string obtainmarks { get; set; }
        public string totalmarks { get; set; }
        public string percentage { get; set; }
        public string maxmarks { get; set; }
        public string maxavg { get; set; }
        public string highname { get; set; }
        public string highclass { get; set; }
    }

}