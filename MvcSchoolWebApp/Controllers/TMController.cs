using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MvcSchoolWebApp.Data;
using MvcSchoolWebApp.Models;

namespace MvcSchoolWebApp.Controllers
{
    public class TMController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string pagename;
        private Database.Database da = new Database.Database("Falconlocal");

        Data.data data = new data();
        private string camp_id;
        private string emp_id;
        public static string[] popupinfo = new string[3];
        public static List<Users> user_dtl;
        public Page Page { get; private set; }
        DataModel _context = new DataModel();
        DatabaeseClass db;
        public static System.IO.MemoryStream workStream;
        public static string strPDFFileName;
        // GET: TM

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Session["User_Dtl"] != null)
            {
                user_dtl = (List<Users>)HttpContext.Session["User_Dtl"];
                base.OnActionExecuting(filterContext);
                user_role = HttpContext.Session["User_Role"].ToString();
                user_id = HttpContext.Session["User_Id"].ToString();

            }
            else
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
        }

    
        [HttpGet]
        public ActionResult essTimeMgmt()
        {
            

            String dateId = "";
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            user_role = HttpContext.Session["User_Role"].ToString();
            user_id = HttpContext.Session["User_Id"].ToString();
            if (list[61].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");          //user has no right to access this page, return to dashboard
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            Timesheetmodal tsm = new Timesheetmodal();
            tsm.empname = db.FillSNSEmployee();
            tsm.empid = user_id;
            tsm.clientname = db.FillClient();
            tsm.date = db.convertservertousertimezone(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            tsm.time = db.convertservertousertimezone(DateTime.Now.ToString()).ToString("hh:mm tt");
            List<ESSModel> esslist = db.getemploydetail(user_id);
            bool isactive = db.isactiveuser(user_id, db.convertservertousertimezone(DateTime.Now.ToString()));
            tsm.empdesignation = esslist[0].design;
            tsm.empdepart = esslist[0].dept;
            if (isactive == true)
            {
                ViewBag.disabletimein = true;
                tsm.clientid = db.getclientid(user_id, dateId);
            }
            else
            {
                tsm.clientid = "150046";
            }
            return View(tsm);
        }

        [HttpGet]
        public ActionResult mssTimeMgmt()
        {
            String dateId = "";
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            user_role = HttpContext.Session["User_Role"].ToString();
            user_id = HttpContext.Session["User_Id"].ToString();
            if (list[63].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");          //user has no right to access this page, return to dashboard
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            Timesheetmodal tsm = new Timesheetmodal();
            tsm.empname = db.FillSNSEmployee();
            tsm.empid = user_id;
            tsm.clientname = db.FillClient();
            tsm.date = db.convertservertousertimezone(DateTime.Now.ToString()).ToString("dd-MMMM-yyyy");
            tsm.time = db.convertservertousertimezone(DateTime.Now.ToString()).ToString("hh:mm tt");
            List<ESSModel> esslist = db.getemploydetail(user_id);
            bool isactive = db.isactiveuser(user_id, db.convertservertousertimezone(DateTime.Now.ToString()));
            tsm.empdesignation = esslist[0].design;
            tsm.empdepart = esslist[0].dept;
            if (isactive == true)
            {
                ViewBag.disabletimein = true;
                tsm.clientid = db.getclientid(user_id, dateId);
            }
            else
            {
                tsm.clientid = "150046";
            }
            return View(tsm);
        }

        public JsonResult insertattendancerecord(string empid, string clientid, DateTime date, string time, string type, string remarks, string lattd, string lngtd)
        {
            DatabaseInsertClass dc = new DatabaseInsertClass();
            db = new DatabaeseClass();
            //if (lattd != null || lngtd != null)
            //{

                dc.InsertEmployeeAttendance(empid, clientid, date, time, type, remarks, lattd, lngtd);

                return Json(db.GetEmployeeAttendanceHistory(empid), JsonRequestBehavior.AllowGet);

            //}
            //else {

            //    return Json(false, JsonRequestBehavior.AllowGet);
            //}
          
           
        }

        public JsonResult filluserinformation(string empid)
        {
            DatabaeseClass db = new DatabaeseClass();
            List<ESSModel> list = new List<ESSModel>();
            if (user_role != "1000")
            {
                empid = user_id;
            }
            list = db.getemploydetail(empid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TimeSheetP(FormCollection formValue)
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[61].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            if (ModelState.IsValid)
            {
                db = new DatabaeseClass();
                string user_id;
                string username;
                if (HttpContext.Session["User_Dtl"] != null)
                {
                    user_dtl = (List<Users>)HttpContext.Session["User_Dtl"];
                    user_id = user_dtl[0].user_id;
                    username = user_dtl[0].user_fullname.ToString();
                    string client = formValue[1].ToString();
                    string employee = formValue["Employee"].ToString();

                    DateTime chkindt = DateTime.UtcNow;
                    string time = chkindt.ToShortTimeString();

                    bool chk;
                    if (user_role == "1000")
                    {

                        chk = db.Filltimesheet(user_id, employee, client, Convert.ToDateTime(time));
                    }
                    else
                    {
                        chk = db.Filltimesheet(user_id, username, client, Convert.ToDateTime(time));
                    }

                    if (chk == true)
                    {
                        return RedirectToAction("TimeSheet", "TM");
                    }
                    else
                    {
                        return RedirectToAction("TimeSheet", "TM");
                    }
                }
            }
            else
            {
                return RedirectToAction("TimeSheet", "TM");
            }
            return RedirectToAction("TimeSheet", "TM");
        }

        public ActionResult Index()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[62].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            Timesheetmodal attendance = new Timesheetmodal();
            ////Timesheetmodal timesheet = new Timesheetmodal();
            attendance.empname = db.FillSNSEmployee();
            attendance.empid = user_id;
            return View(attendance);
        }

        public ActionResult machine()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[62].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            Timesheetmodal attendance = new Timesheetmodal();
            ////Timesheetmodal timesheet = new Timesheetmodal();
            attendance.empname = db.FillSNSEmployee();
            attendance.empid = user_id;
            return View(attendance);
        }



        public JsonResult getempattdlog(string empid)
        {
            DatabaeseClass db = new DatabaeseClass();
            return Json(db.GetEmployeeAttendanceHistory(empid), JsonRequestBehavior.AllowGet);
        }

        public JsonResult isactiveemployee(string empid, DateTime date)
        {
            DatabaeseClass db = new DatabaeseClass();
            return Json(db.isactiveuser(empid, date), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getclientid(string empid, string dateId)
        {
            DatabaeseClass db = new DatabaeseClass();
            return Json(db.getclientid(empid, dateId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateConveyanceReport(DateTime Month)
        {
            workStream = new System.IO.MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.UtcNow;
            //file name to be created   
            strPDFFileName = string.Format("Conveyance_Timesheet_Pdf" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout = new PdfPTable(7);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   
            doc.Add(Conveyance_Add_Content_To_PDF(tableLayout, Month));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreatePdf(string empid, DateTime date)
        {
            workStream = new System.IO.MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.UtcNow;
            //file name to be created   
            strPDFFileName = string.Format("Employee_Timesheet_Pdf" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout = new PdfPTable(6);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   
            doc.Add(Add_Content_To_PDF_Machine(tableLayout, empid, date));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return Json("", JsonRequestBehavior.AllowGet);


        }

        public FileResult launch_report()
        {
            return File(workStream, "application/pdf");
        }

        public FileResult launch_Conveyance_report()
        {
            return File(workStream, "application/pdf");
        }

        protected PdfPTable Add_Content_To_PDF_Machine(PdfPTable tableLayout, string empid, DateTime date)
        {
            float[] headers = { 27, 24, 25, 25, 70, 30 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            db = new DatabaeseClass();
            List<Timesheetmodal> list = db.FillTimeSheetAttendanceMachine(empid, date);
            string empname = db.getempname(empid);


            tableLayout.AddCell(new PdfPCell(new Phrase("Employee Time Sheet Report (Machine)", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            tableLayout.AddCell(new PdfPCell(new Phrase("Employee : " + empname + "   |  Id: " + empid + " ", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 1,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });


            ////Add header  
            AddCellToHeader(tableLayout, "Date");
            AddCellToHeader(tableLayout, "Day");
            AddCellToHeader(tableLayout, "Checkin Time");
            AddCellToHeader(tableLayout, "Checkout Time");
            AddCellToHeader(tableLayout, "Client");
            AddCellToHeader(tableLayout, "Location");
            // AddCellToHeader(tableLayout, "Marked By");


            foreach (var emp in list)
            {
                string clientname = "Supernova Solutions";
            

                AddCellToBody(tableLayout, emp.date);
                AddCellToBody(tableLayout, emp.day);
                AddCellToBody(tableLayout, emp.checkintime);
                AddCellToBody(tableLayout, emp.checkouttime);

                AddCellToBody(tableLayout, clientname);
                AddCellToHeader(tableLayout, "-");

            }
            return tableLayout;
        }



        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, string empid, DateTime date)
        {
            float[] headers = { 27, 24, 25, 25, 70, 30 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            db = new DatabaeseClass();
            List<Timesheetmodal> list = db.FillTimeSheetAttendance(empid, date);
            string empname = db.getempname(empid);


            tableLayout.AddCell(new PdfPCell(new Phrase("Employee Time Sheet Report ", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            tableLayout.AddCell(new PdfPCell(new Phrase("Employee : " + empname + "   |  Id: " + empid + " ", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 1,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });


            ////Add header  
            AddCellToHeader(tableLayout, "Date");
            AddCellToHeader(tableLayout, "Day");
            AddCellToHeader(tableLayout, "Checkin Time");
            AddCellToHeader(tableLayout, "Checkout Time");
            AddCellToHeader(tableLayout, "Client");
            AddCellToHeader(tableLayout, "Location");
            // AddCellToHeader(tableLayout, "Marked By");


            foreach (var emp in list)
            {
                string clientname = "Supernova Solutions";
                string clienid = emp.clientid.Trim();
                if (clienid != "0343")
                {
                    clientname = emp.client;
                }

                AddCellToBody(tableLayout, emp.date);
                AddCellToBody(tableLayout, emp.day);
                AddCellToBody(tableLayout, emp.checkintime);
                AddCellToBody(tableLayout, emp.checkouttime);
                
                AddCellToBody(tableLayout, clientname);
                AddCellToHeader(tableLayout, "-");

            }
            return tableLayout;
        }

        protected PdfPTable Conveyance_Add_Content_To_PDF(PdfPTable tableLayout, DateTime Month)
        {
            float[] headers = { 30, 30, 30, 30,30,30,30}; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            db = new DatabaeseClass();
            List<Timesheetmodal> list = db.FillTimeSheetConveyance(Month);


            tableLayout.AddCell(new PdfPCell(new Phrase("Client Visit Report ", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 10,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            //tableLayout.AddCell(new PdfPCell(new Phrase("Employee : " + empname + "   |  Id: " + empid + " ", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            //{
            //    Colspan = 12,
            //    Border = 1,
            //    PaddingBottom = 5,
            //    HorizontalAlignment = Element.ALIGN_CENTER
            //});


            ////Add header  
            AddCellToHeader(tableLayout, "Name");
            AddCellToHeader(tableLayout, "Client");
            AddCellToHeader(tableLayout, "No. of Visit");
            AddCellToHeader(tableLayout, "Total Km");
            AddCellToHeader(tableLayout, "Rate per km");
            AddCellToHeader(tableLayout, "Rate Per Visit");
            AddCellToHeader(tableLayout, "Total");
            // AddCellToHeader(tableLayout, "Marked By");


            foreach (var emp in list)
            {
                AddCellToBody(tableLayout, emp.Name);
                AddCellToBody(tableLayout, emp.client);
                AddCellToBody(tableLayout, emp.noofvisit);
                AddCellToBody(tableLayout, emp.totalkm);
                AddCellToBody(tableLayout, emp.rateperkm);
                AddCellToBody(tableLayout, emp.rates);
                AddCellToBody(tableLayout, emp.total);
            }
            return tableLayout;
        }

        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(254, 90, 90)
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }

        public JsonResult getAttendanceFillJQGrid(string empid, DateTime dateid)
        {
            if (empid == null || empid == "")
                empid = user_id;
            db = new DatabaeseClass();
            return Json(db.FillTimeSheetAttendance(empid, dateid), JsonRequestBehavior.AllowGet);
        }


        public JsonResult getAttendanceFillJQGridMachine(string empid, DateTime dateid)
        {
            if (empid == null || empid == "")
                empid = user_id;
            db = new DatabaeseClass();
            return Json(db.FillTimeSheetAttendanceMachine(empid, dateid), JsonRequestBehavior.AllowGet);
        }



        public ActionResult Dailytimesheet()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[62].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            // db = new DatabaeseClass();
            //Timesheetmodal attendance = new Timesheetmodal();
            //attendance.empname = db.FillSNSEmployee();
            //attendance.empid = user_id;
            db = new DatabaeseClass();
            ViewBag.date = db.convertservertousertimezone(DateTime.UtcNow.ToString()).ToString("dd-MMMM-yyyy");
            return View();
        }


        public ActionResult Dailytimesheet2()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[62].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            // db = new DatabaeseClass();
            //Timesheetmodal attendance = new Timesheetmodal();
            //attendance.empname = db.FillSNSEmployee();
            //attendance.empid = user_id;
            db = new DatabaeseClass();
            ViewBag.date = db.convertservertousertimezone(DateTime.UtcNow.ToString()).ToString("dd-MMMM-yyyy");
            return View();
        }
        public ActionResult DisplayConveyanceReport()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            if (list[66].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            return View();
        }

        public JsonResult CreateDailyReport(DateTime date)
        {
            workStream = new System.IO.MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.UtcNow;
            //file name to be created   
            strPDFFileName = string.Format("Daily_Timesheet_Pdf" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 6 columns  
            PdfPTable tableLayout = new PdfPTable(9);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   
            doc.Add(Add_Content_To_RPT(tableLayout, date));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            //return JsonResult(workStream, "application/pdf");
            return Json("", JsonRequestBehavior.AllowGet);

        }


        public JsonResult CreateDailyReport2(DateTime date)
        {
            workStream = new System.IO.MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.UtcNow;
            //file name to be created   
            strPDFFileName = string.Format("Daily_Timesheet_Pdf" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 6 columns  
            PdfPTable tableLayout = new PdfPTable(9);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   
            doc.Add(Add_Content_To_RPT2(tableLayout, date));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            //return JsonResult(workStream, "application/pdf");
            return Json("", JsonRequestBehavior.AllowGet);

        }

        protected PdfPTable Add_Content_To_RPT2(PdfPTable tableLayout, DateTime date)
        {
            float[] headers = { 12, 40, 19, 19, 65, 25, 25, 25, 25 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            db = new DatabaeseClass();
            List<Timesheetmodal> list = db.FillDailyReport2(date);
            //string empname = db.getempname(empid);
            if (list != null)
            {

                tableLayout.AddCell(new PdfPCell(new Phrase("Daily Time Sheet Report (Machine)", new Font(Font.FontFamily.COURIER, 14, 1, new iTextSharp.text.BaseColor(244, 245, 245))))
                {
                    Colspan = 12,
                    Border = 0,
                    PaddingBottom = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BackgroundColor = new iTextSharp.text.BaseColor(92, 176, 242)
                });


                tableLayout.AddCell(new PdfPCell(new Phrase("Date : " + list[0].date + "\nDay  : " + list[0].day + " ", new Font(Font.FontFamily.COURIER, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
                {
                    Colspan = 12,
                    Border = 1,
                    PaddingBottom = 5,
                    HorizontalAlignment = Element.ALIGN_LEFT
                });

                tableLayout.AddCell(createCell("E-ID", 1, 2, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Name", 1, 2, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Time In", 1, 2, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Time Out", 1, 2, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Client", 1, 2, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Location Time In", 2, 1, PdfPCell.TOP_BORDER));
                tableLayout.AddCell(createCell("Location Time Out", 2, 1, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Longitude ", 1, 1, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Latitude", 1, 1, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Longitude ", 1, 1, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Latitude", 1, 1, PdfPCell.BOX));

                foreach (var emp in list)
                {

                    AddCellToBodyRpt(tableLayout, emp.empid);
                    AddCellToBodyRpt(tableLayout, emp.employeename);
                    AddCellToBodyRpt(tableLayout, emp.checkintime);
                    AddCellToBodyRpt(tableLayout, emp.checkouttime);
                    if (string.IsNullOrWhiteSpace(emp.client))
                    { AddCellToBodyRpt(tableLayout, "Supernova Solutions"); }
                    else { AddCellToBodyRpt(tableLayout, emp.client); }
                    AddCellToBodyRpt(tableLayout, emp.tinlong);
                    AddCellToBodyRpt(tableLayout, emp.tinlat);
                    AddCellToBodyRpt(tableLayout, emp.toutlong);
                    AddCellToBodyRpt(tableLayout, emp.toutlat);
                }
            }
            else
            {

                tableLayout = new PdfPTable(1);
                AddCellToHeaderRpt(tableLayout, "No Data Found in This Daily Timesheet Report");
                AddCellToBodyRpt(tableLayout, "No Record Found for Selected Date !");

            }
            return tableLayout;
        }
        protected PdfPTable Add_Content_To_RPT(PdfPTable tableLayout, DateTime date)
        {
            float[] headers = { 12, 40, 19, 19, 65, 25, 25, 25, 25 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            db = new DatabaeseClass();
            List<Timesheetmodal> list = db.FillDailyReport(date);
            //string empname = db.getempname(empid);
            if (list != null)
            {

                tableLayout.AddCell(new PdfPCell(new Phrase("Daily Time Sheet Report ", new Font(Font.FontFamily.COURIER, 14, 1, new iTextSharp.text.BaseColor(244, 245, 245))))
                {
                    Colspan = 12,
                    Border = 0,
                    PaddingBottom = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BackgroundColor = new iTextSharp.text.BaseColor(92, 176, 242)
                });


                tableLayout.AddCell(new PdfPCell(new Phrase("Date : " + list[0].date + "\nDay  : " + list[0].day + " ", new Font(Font.FontFamily.COURIER, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
                {
                    Colspan = 12,
                    Border = 1,
                    PaddingBottom = 5,

                    HorizontalAlignment = Element.ALIGN_LEFT
                });

                tableLayout.AddCell(createCell("E-ID", 1, 2, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Name", 1, 2, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Time In", 1, 2, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Time Out", 1, 2, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Client", 1, 2, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Location Time In", 2, 1, PdfPCell.TOP_BORDER));
                tableLayout.AddCell(createCell("Location Time Out", 2, 1, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Longitude ", 1, 1, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Latitude", 1, 1, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Longitude ", 1, 1, PdfPCell.BOX));
                tableLayout.AddCell(createCell("Latitude", 1, 1, PdfPCell.BOX));


                foreach (var emp in list)
                {

                    TimeSpan seconds = TimeSpan.Parse(emp.checkintime.Replace(" AM", "").Replace(" PM", ""));
                    //double seconds =     TimeSpan.Parse(emp.checkintime).TotalSeconds;
                    TimeSpan second = TimeSpan.Parse("09:30:00");

                    if (seconds > second && emp.client == "Supernova Solutions 206")
                    {

                        //tableLayout.AddCell(new PdfPCell(new Phrase(emp, new Font(Font.FontFamily.COURIER, 10, 1, new iTextSharp.text.BaseColor(244, 245, 245))))
                        //);
                        tableLayout.AddCell(createCell(emp.empid, 1, 2, PdfPCell.BOX));
                        tableLayout.AddCell(createCell(emp.employeename, 1, 2, PdfPCell.BOX));
                        tableLayout.AddCell(createCell(emp.checkintime, 1, 2, PdfPCell.BOX));
                        tableLayout.AddCell(createCell(emp.checkouttime, 1, 2, PdfPCell.BOX));
                        tableLayout.AddCell(createCell(emp.client, 1, 2, PdfPCell.BOX));
                        tableLayout.AddCell(createCell(emp.tinlong, 1, 2, PdfPCell.BOX));
                        tableLayout.AddCell(createCell(emp.tinlat, 1, 2, PdfPCell.BOX));
                        tableLayout.AddCell(createCell(emp.toutlong, 1, 2, PdfPCell.BOX));
                        tableLayout.AddCell(createCell(emp.toutlat, 1, 2, PdfPCell.BOX));


                    }
                    else
                    //tableLayout.AddCell(createCell(emp.empid, 1, 2, PdfPCell.BOX));
                    //tableLayout.AddCell(createCell(emp.employeename, 1, 2, PdfPCell.BOX));
                    //tableLayout.AddCell(createCell(emp.checkintime, 1, 2, PdfPCell.BOX));
                    //tableLayout.AddCell(createCell(emp.checkouttime, 1, 2, PdfPCell.BOX));
                    //tableLayout.AddCell(createCell(emp.client, 1, 2, PdfPCell.BOX));
                    //tableLayout.AddCell(createCell(emp.tinlong, 1, 2, PdfPCell.BOX));
                    //tableLayout.AddCell(createCell(emp.tinlat, 1, 2, PdfPCell.BOX));
                    //tableLayout.AddCell(createCell(emp.toutlong, 1, 2, PdfPCell.BOX));
                    //tableLayout.AddCell(createCell(emp.toutlat, 1, 2, PdfPCell.BOX));


                    {

                        AddCellToBodyRpt(tableLayout, emp.empid);
                        AddCellToBodyRpt(tableLayout, emp.employeename);
                        AddCellToBodyRpt(tableLayout, emp.checkintime);
                        AddCellToBodyRpt(tableLayout, emp.checkouttime);
                        if (string.IsNullOrWhiteSpace(emp.client))
                        { AddCellToBodyRpt(tableLayout, "Supernova Solutions"); }
                        else { AddCellToBodyRpt(tableLayout, emp.client); }
                        AddCellToBodyRpt(tableLayout, emp.tinlong);
                        AddCellToBodyRpt(tableLayout, emp.tinlat);
                        AddCellToBodyRpt(tableLayout, emp.toutlong);
                        AddCellToBodyRpt(tableLayout, emp.toutlat);
                    }
                }
            }
            else
            {

                tableLayout = new PdfPTable(1);
                AddCellToHeaderRpt(tableLayout, "No Data Found in This Daily Timesheet Report");
                AddCellToBodyRpt(tableLayout, "No Record Found for Selected Date !");

            }
            return tableLayout;
        }
        // Method to add single cell to the Header  
        private static void AddCellToHeaderRpt(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 14, 1, iTextSharp.text.BaseColor.WHITE)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(254, 90, 90)
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBodyRpt(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.TIMES_ROMAN, 7, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }
        public PdfPCell createCell(string content, int colspan, int rowspan, int border)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, new Font(Font.FontFamily.COURIER, 8, 1, iTextSharp.text.BaseColor.BLACK)));
            cell.Colspan=colspan;
            cell.Rowspan=rowspan;
            cell.BorderWidth = 1;
            cell.Border=border;
            cell.HorizontalAlignment =Element.ALIGN_CENTER;
            cell.BackgroundColor = new iTextSharp.text.BaseColor(254, 90, 90);
            return cell;
        }
        public FileResult launch_Daily_report()
        {
            return File(workStream, "application/pdf");
        }

        public JsonResult DeleteTime(DateTime date, string clientid, string empid)
        {
            DatabaseInsertClass din = new DatabaseInsertClass();
            if (empid == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(din.UpdateTimeSheet(date, clientid, empid), JsonRequestBehavior.AllowGet);
        }
    }
}