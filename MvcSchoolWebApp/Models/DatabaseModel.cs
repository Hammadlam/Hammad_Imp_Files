using MvcSchoolWebApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Models
{
    public class DatabaseModel
    {
        //public List<SelectListItem> campus { get; set; }
        public List<campus> temcam { get; set; }
        public List<class_d> temclass { get; set; }
        public List<SelectListItem> campus { get; set; }
        public string campusid { get; set; }

        public List<SelectListItem> classes { get; set; }
        public string classesid { get; set; }
        public List<SelectListItem> section { get; set; }
        public string sectionid { get; set; }
        public List<SelectListItem> subject { get; set; }
        public string subjectid { get; set; }
        public List<SelectListItem> subjectcamp { get; set; }
        public string subjectidcamp { get; set; }
        public List<SelectListItem> session { get; set; }
        public string sessionid { get; set; }
        public List<SelectListItem> category { get; set; }
        public string categoryid { get; set; }
        public string date { get; set; }
        public List<SelectListItem> student { get; set; }
        public string studentid { get; set; }
        public string teachername { get; set; }
        public string week { get; set; }
        public List<SelectListItem> weeklist { get; set; }
        public string topic { get; set; }
        public string objective { get; set; }
        public List<SelectListItem> time_break { get; set; }
        public string time_break_id { get; set; }
        public List<SelectListItem> obj_time_break { get; set; }
        public string obj_time_break_id { get; set; }
        public List<SelectListItem> eval_time_break { get; set; }
        public string eval_time_break_id { get; set; }
        public List<SelectListItem> tm_time_break { get; set; }
        public string tm_time_break_id { get; set; }
        public List<SelectListItem> rd_time_break { get; set; }
        public string rd_time_break_id { get; set; }
        public List<SelectListItem> ww_time_break { get; set; }
        public string ww_time_break_id { get; set; }
        public List<SelectListItem> wu_time_break { get; set; }
        public string wu_time_break_id { get; set; }
        public string resource { get; set; }
        public string evaluation { get; set; }
        public string writtenwork { get; set; }
        public string wrapup { get; set; }
        public string princ_comments { get; set; }
        public string princ_comments_new { get; set; }
        public string evaluationstdid { get; set; }
        public List<SelectListItem> evaluationstd { get; set; }
        public string evaluationteacherid { get; set; }
        public List<SelectListItem> evaluationteacher { get; set; }
        public List<SelectListItem> stdname { get; set; }
        public int stdnameID { get; set; }
        public List<SelectListItem> module { get; set; }
        public string moduleid { get; set; }
        public string marks { get; set; }
        public string marksp2 { get; set; }
        public string marksp3 { get; set; }
        public string obtainedmarks { get; set; }
        public string month { get; set; }
        public DateTime feeMonth { get; set; }
        public string teacherid { get; set; }
        public string imagepath { get; set; }
        public string begdate { get; set; }
        public string enddate { get; set; }
        public string filename { get; set; }
        public double percentage { get; set; }
        public string exammarks { get; set; }
        public string projectmarks { get; set; }
        public string testmarks { get; set; }
        public string oralmarks { get; set; }
        public string assignmarks { get; set; }
        public string moduletxt { get; set; }
        public string total_strength { get; set; }
        public string total_boys { get; set; }
        public string total_girls { get; set; }
        public string increment { get; set; }
        public string total_present { get; set; }
        public string total_absent { get; set; }
        public string teach_method { get; set; }
        public string read_disc { get; set; }
        public string user { get; set; }
        public string maxmarks { get; set; }
        public string maxavg { get; set; }
        public string highname { get; set; }
        public string highclass { get; set; }
        public string reptid { get; set; }

        public List<SelectListItem> reports { get; set; }

        //public ActionResult GetMsgDetail(string title, string msg, string sendTo, string groupid, string attachfile)
        public string title { get; set; }
        public string msg { get; set; }
        public string sendTo { get; set; }
        public string groupid { get; set; }
        public string grouptxt { get; set; }





    }
}