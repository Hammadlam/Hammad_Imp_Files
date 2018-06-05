using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Controllers
{
    public class EventController : Controller
    {
        MessageCls msgobj = new MessageCls();
        DatabaeseClass dc = new DatabaeseClass();
        DatabaseInsertClass dic = new DatabaseInsertClass();
        static bool check = false;

            // GET: Event
        public ActionResult Index()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            return View();
        }

        public JsonResult GetEvents()
        {

            List<EventSchedular> items = new List<EventSchedular>();
            String day, month, year;
            string tem;
            string[] splitString;
            string date = "";

            //items.Add(new EventSchedular
            //{
            //    Subject = "check",
            //    Start = "11-Nov-2017"
            //});

            if (dc.GetCalendarEvent() != null)
            {
                items = dc.GetCalendarEvent();
                for (int i = 0; i < items.Count; i++)
                {
                    tem = items.ElementAt(i).Start;
                    splitString = tem.Split();
                    day = splitString[0];
                    month = splitString[1];
                    year = splitString[2];
                    date = day + "-" + month + "-" + year;
                    items.ElementAt(i).Start = date;
                    date = "";
                }
            }
            else
            {
                items.Add(new EventSchedular
                {
                    Start = ""
                });
            }
            return new JsonResult { Data = items, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult DeleteEvent(String eventID)
        {
            List<EventSchedular> tem = new List<EventSchedular>();
            String status ="false";
            String day, month, year;
            string[] splitString;
            string[] splityear;
            if (eventID != null)
            {
                splitString = eventID.Split('-');

                day = splitString[0];
                month = splitString[1];
                year = splitString[2];

                splityear = year.Split();
                year = splityear[0];
                day = day + " " + month + " " + year + " ";
                dic.update_event_db(day);
                status = "true";
            }
            tem.Add(new EventSchedular
            {
                status = status
            });
            return Json(tem, JsonRequestBehavior.AllowGet);
        }
    }
}