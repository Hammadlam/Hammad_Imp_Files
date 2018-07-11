﻿using System;
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
        // GET: TM
        [HttpGet]
        public ActionResult TimeSheet()
        {
            var list = HttpContext.Session["User_Rights"] as List<MvcSchoolWebApp.Models.LoginModel>;
            user_role = HttpContext.Session["User_Role"].ToString();
            if (list[61].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }
            else
            {
                //if (user_role == "1000")
                //{
                    db = new DatabaeseClass();
                    ViewData["Employee"] = db.FillSNSEmployee();
                     var fromDatabaseEF = db.getClient();
                    ViewData["DBMySkills"] = fromDatabaseEF;
                    return View("MATimeSheet");
                //}
                //else
                //{
                //    db = new DatabaeseClass();
                //    var fromDatabaseEF = db.getClient();
                //    ViewData["DBMySkills"] = fromDatabaseEF;
                //    return View("TimeSheet");
                //}

            }
            return View("TimeSheet");
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

                    DateTime chkindt = DateTime.Now;
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
            if (list[61].menustat != "X")
            {
                return RedirectToAction("Index", "dashboard");
            }

            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            //Timesheetmodal timesheet = new Timesheetmodal();
            ViewData["empnames"] = db.FillSNSEmployee();
            return View();
        }



        public FileResult CreatePdf (FormCollection data)
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("TimeSheetReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout = new PdfPTable(7);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  
            string empid = data[0].ToString();
            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);
            

            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   
            doc.Add(Add_Content_To_PDF(tableLayout, empid));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_PDF (PdfPTable tableLayout, string empid)
        {

            float[] headers = { 50, 20, 45, 45, 50, 45,25 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  
            //string query = "SELECT * FROM emptimesht WHERE empid ='" + empid + "'";

            List<Timesheetmodal> timssht = _context.Timesheets.ToList();
            string name="";
            foreach (var empnm in timssht)
            {
                name = empnm.Name.ToString();
            }


                tableLayout.AddCell(new PdfPCell(new Phrase("Time Sheet Report", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0,0,0)))) {
                Colspan = 12, Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            tableLayout.AddCell(new PdfPCell(new Phrase("Employee Id "+empid+" Employee "+name, new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            ////Add header  
            AddCellToHeader(tableLayout,"Date");
            AddCellToHeader(tableLayout, "Day");
            //AddCellToHeader(tableLayout,"Name");
            AddCellToHeader(tableLayout,"Checkin Date");
            AddCellToHeader(tableLayout,"Checkout Date");
            AddCellToHeader(tableLayout, "Client");
            AddCellToHeader(tableLayout, "Location");
            AddCellToHeader(tableLayout, "Marked By");
            ////Add body  

            foreach (var emp in timssht)
            {
                DateTime date = Convert.ToDateTime(emp.Checkindt.ToShortDateString());
                DateTime day = emp.Checkindt;
                AddCellToBody(tableLayout, date.ToString("dd-MMMM-yyyy"));
                AddCellToBody(tableLayout, day.ToString("ddd"));
                AddCellToBody(tableLayout, emp.Checkindt.ToString("hh:mm:sstt"));
                AddCellToBody(tableLayout, emp.Checkoutdt.ToString("hh:mm:sstt"));
                AddCellToBody(tableLayout, emp.Client);
                AddCellToBody(tableLayout, " ");
                AddCellToHeader(tableLayout, "Self");

            }

            return tableLayout;
        }
        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.RED)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(185, 211, 255)
    });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK)))
             {
                HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
             });
        }
    }
}