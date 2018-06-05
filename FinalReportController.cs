using CrystalDecisions.CrystalReports.Engine;
using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using MvcSchoolWebApp.Data;
using System.Configuration;
using System.Text;
using CrystalDecisions.Shared;
using System.Data.OleDb;
using System.Runtime.Serialization.Formatters.Binary;

namespace MvcSchoolWebApp.Controllers
{
    
    public class FinalReportController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;

        // GET: FinalReport
        DatabaeseClass db;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["User_Role"] != null)
            {
                base.OnActionExecuting(filterContext);
                user_role = HttpContext.Session["User_Role"].ToString();
                user_id = HttpContext.Session["User_Id"].ToString();
            }
            else
                filterContext.Result = new RedirectResult("/Login");
        }



        string cs = ConfigurationManager.ConnectionStrings["smarterdb"].ConnectionString;
        public ActionResult FinalReport()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            db = new DatabaeseClass();
            DatabaseModel report = new DatabaseModel();

            report.campus = db.FillCamp(user_id);
            report.classes = db.FillClass(report.campus[0].Value, user_id);
            report.section = db.FillSection(report.campus[0].Value, report.classes[0].Value, user_id);
            report.stdname = db.studentname(report.campus[0].Value, report.classes[0].Value, report.section[0].Value);
            report.campusid = report.campus[0].Value;

            if (user_role == "student")
            {
                report.classesid = report.classes[0].Value;
                report.sectionid = report.section[0].Value;
                report.studentid = report.stdname[0].Value;
            }
            else
            {
                report.classesid = "";
                report.sectionid = "";
                report.studentid = "";
            }

            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            report.subject = sl;

            return View(report);
        }
        public ActionResult GetReportParam(IList<ReportParamCls> model)
        {

            Session["Campus"] = model[0].Campus;
            Session["Class"] = model[0].Class;
            Session["Section"] = model[0].Section;
            Session["StudentName"] = model[0].StudentName;
            Session["Subject"] = model[0].Subject;
            Session["ReportId"] = model[0].ReportId;

            //Response.Redirect("~/FinalReport/Getsquery");
            string url = Url.Action("Getsquery", "FinalReport");
            //string url = Url.Action("ExecuteReportTest", "FinalReport");
            return Json(url);
        }
        public void Getsquery()
        {
            string getSquery = string.Empty;
            string[] ParamValues = new string[4];
            ParamValues[0] = Session["Campus"].ToString();
            ParamValues[1] = Session["Class"].ToString();
            ParamValues[2] = Session["Section"].ToString();
            ParamValues[3] = Session["StudentName"].ToString();
            //ParamValues[4] = Session["Subject"].ToString();

            using (SqlConnection con = new SqlConnection(cs))
            {

                string query = "SELECT hdr.squery,dtl.reptname FROM cryquehdr hdr inner join crydtl dtl on dtl.reptid=hdr.sforid WHERE sforid = '" + Session["ReportId"] + "'";
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                DataSet dss = new DataSet();
                sda.Fill(dss);

                getSquery = dss.Tables[0].Rows[0]["squery"].ToString();
                Session["Squery"] = getSquery;
                Session["ReportId"] = Session["ReportId"];
            }

            Replacesquery(ParamValues); //call fucntion to replace
        }

        public void Replacesquery(string[] ParamValues)
        {
            string UpdatedQuery = null;
            string setSquery = Session["Squery"].ToString();
            string VSQL = "";
            for (int i = 0; i < ParamValues.Length; i++)
            {
                if (ParamValues[i] != "")
                {
                    UpdatedQuery = setSquery.Replace("%" + i + "%", "'" + ParamValues[i] + "'");

                    setSquery = UpdatedQuery;
                    UpdatedQuery = UpdatedQuery.Replace("\r\n", " ");
                    UpdatedQuery = UpdatedQuery.Replace("\n", " ");
                    UpdatedQuery = UpdatedQuery.Replace("\t", " ");
                    UpdatedQuery = UpdatedQuery.Replace("\r", " ");
                }
                else
                {
                   
                    UpdatedQuery = setSquery;
                    UpdatedQuery = UpdatedQuery.Replace("\r\n", " ");
                    UpdatedQuery = UpdatedQuery.Replace("\n", " ");
                    UpdatedQuery = UpdatedQuery.Replace("\t", " ");
                    UpdatedQuery = UpdatedQuery.Replace("\r", " ");
                }

                Session["UpdatedQuery"] = UpdatedQuery;
                VSQL = Session["UpdatedQuery"].ToString();
                
                //call Ai
                //Call executereport fnc
            }
            Query_Edit_after_Filldata(VSQL);
        }

        private void Query_Edit_after_Filldata(string VSQL)
        {
            int functionReturnValue = 0;
            object IsInitialize = null;
            // ERROR: Not supported in C#: OnErrorStatement

            int vcount = 0;
            int Vpos_Str = 0;
            int VPos_RvStr = 0;
            int Persent_Value = 0;
            int Counter = 0;
            int vpos = 0;
            string[] Vpos_persent = null;
            string vQuery = null;
            int VtotPos = 0;

            ///''''''''''''''''''''''''''''''''''''''''''''''''
            // Edit Query For Persentage Sign
            ///''''''''''''''''''''''''''''''''''''''''''''''''

            ///'' Filling Array''''''
            vpos = 1;
            Counter = 0;

            //while (vpos <= Strings.Len(VSQL))
            long size = 0;
            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, VSQL);
                size = s.Length;
            }

            while (vpos < VSQL.Length - 3)
            {
                
                if ((VSQL.Substring(vpos, 1) == "%") || (VSQL.Substring(vpos, 1) == "#"))
                {

                    if ((VSQL.Substring(vpos + 2, 1) == "%") || (VSQL.Substring(vpos + 2, 1) == "#"))
                    {
                        Array.Resize(ref Vpos_persent, Counter + 1);

                        Vpos_persent[Counter] = VSQL.Substring(vpos, 3);
                        vcount = vcount + 1;
                        Counter = Counter + 1;
                    
                    }

                    else if ((VSQL.Substring(vpos + 3, 1) == "%") || (VSQL.Substring(vpos + 3, 1) == "#"))
                    {

                        if (VSQL.Substring(vpos + 2, 1).All(char.IsDigit))
                        {
                            Array.Resize(ref Vpos_persent, Counter + 1);

                            Vpos_persent[Counter] = VSQL.Substring(vpos, 4);
                            vcount = vcount + 1;
                            Counter = Counter + 1;
                        }
                    }
                    else
                    {
                        vcount = vcount + 1;
                    }
                }
                vpos = vpos + 1;
            }
            ///''''''''''''

            if (Vpos_persent != null) // IsInitialize(Vpos_persent))
            {

                for (Counter = 0; Counter < Vpos_persent.GetLength(0); Counter++)
                {

                    //Vpos_Str = Strings.InStr(1, VSQL, Vpos_persent(Counter));
                    Vpos_Str = VSQL.IndexOf(Vpos_persent[Counter]);
                    ///' Searching the position of String


                    //RvrStringfnc(VSQL.Substring(0, Vpos_Str));

                    //VPos_RvStr = Vpos_Str -  VSQL.Substring(0, Vpos_Str).Reverse().ToString().ToUpper().IndexOf("DNA");
                    VPos_RvStr = Vpos_Str - RvrStringfnc(VSQL.Substring(0, Vpos_Str)).ToUpper().IndexOf("DNA");
                    VPos_RvStr = VPos_RvStr - 2;
                    //VPos_RvStr = Strings.InStrRev(Strings.UCase(VSQL), "AND",  Vpos_Str);
                    ///' Rev Searching
                    VtotPos = Vpos_Str + 5 - VPos_RvStr;
                    ///''Differec between Start String & End String

                    ///vQuery = Strings.Mid(VSQL, VPos_RvStr - 1, VtotPos);
                    vQuery = VSQL.Substring(VPos_RvStr - 1, VtotPos);
                    ///' Getting the Higlight String for Delete

                    VSQL = VSQL.Replace(vQuery, " ");
                    ///'Deleteing the Highlight String
                }
            }

            TempData["UpdatedQuery"] = VSQL;
            //ExecuteReport();
            //Response.Flush();
            //try
            //{
                Response.Redirect("~/FinalReport/ExecuteReport");
            //}
            //catch (Exception ex)
            //{

            //}
            
        }

        public string RvrStringfnc(string Reversing)
        {
            string Reversed = null;
            for (int i = Reversing.Length - 1; i >= 0; i--)
            {
                Reversed += Reversing[i];
            }
            string check = Reversed;
            return Reversed;
        }

        public ActionResult ExecuteReport()
        {
            // ***** 13 Sept *****

            //TempData.Keep("ReportId");
            //TempData.Keep("UpdatedQuery");


            //string ReportsId = TempData["ReportId"].ToString();


            //string reportname;

            //using (SqlConnection con1 = new SqlConnection(cs))
            //{
            //    string query = "SELECT reptname FROM crydtl WHERE(reptid = '" + ReportsId + "')";
            //    con1.Open();
            //    SqlCommand cmd = new SqlCommand(query, con1);
            //    reportname = cmd.ExecuteScalar().ToString();
            //    con1.Close();
            //}


            //DataTable dt = new DataTable();
            //using (SqlConnection conn = new SqlConnection(cs))
            //{
            //    conn.Open();
            //    string UpdateQuery = TempData["UpdatedQuery"].ToString();

            //    SqlDataAdapter sda = new SqlDataAdapter();
            //    sda.SelectCommand = new SqlCommand(UpdateQuery, conn);

            //    sda.Fill(dt);
            //    conn.Close();
            //}

            //// ReportClass rc = new ReportClass();
            ////ReportDocument myreport = new ReportDocument();
            //ReportDocument rc = new ReportDocument();

            ////rc.FileName = Server.MapPath("/CrystalReports/" + reportname + ".rpt");
            //rc.Load(Server.MapPath("/CrystalReports/" + reportname + ".rpt"));

            //rc.SetDataSource(dt);

            ////rc.SetDatabaseLogon("snova786_falcon", "sns123", "wdb1.my-hosting-panel.com", "snova786_ermfalcon");
            //// rc.SetDatabaseLogon("DB_A2997E_falcon_admin", "bilal123", "sql5034.smarterasp.net", "DB_A2997E_falcon");
            //rc.SetDatabaseLogon("DB_A2A9A5_snova786_admin", "snova@786", "sql7001.smarterasp.net", "DB_A2A9A5_snova786");
            //Stream stream = rc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            //return File(stream, "application/pdf"); 

            // ***** 13 Sept ***** 

            TempData.Keep("ReportId");
            TempData.Keep("UpdatedQuery");

            string reportid = Session["ReportId"].ToString();
            //Getsquery();
            string UpdatedQuery = Session["UpdatedQuery"].ToString();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "select rptfile from crydtl where reptid = '" + reportid + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                string rptfilename = cmd.ExecuteScalar().ToString();
                string xmlfilename = rptfilename.Replace(".rpt", ".xml");
                SqlDataAdapter sda = new SqlDataAdapter(UpdatedQuery, con);
                //DataTable dt = new DataTable();
                //sda.Fill(dt);

                //ReportDocument rc = new ReportDocument();
                //rc.Load(Server.MapPath("/CrystalReports/" + rptfilename + ".rpt"));
                //rc.SetDataSource(dt);
                ////rc.SetDataSource(dt);

                //rc.SetDatabaseLogon("DB_A2A9A5_snova786_admin", "snova@786", "sql7001.smarterasp.net", "DB_A2A9A5_snova786");
                //CrystalDecisions.Shared.ConnectionInfo connInfo = new CrystalDecisions.Shared.ConnectionInfo();
                //connInfo.ServerName = "sql7001.smarterasp.net";
                //connInfo.DatabaseName = "DB_A2A9A5_snova786";
                //connInfo.UserID = "DB_A2A9A5_snova786_admin";
                //connInfo.Password = "snova@786";

                //rc.DataSourceConnections.Clear();

                //TableLogOnInfo tableLogOnInfo2 = new TableLogOnInfo();
                //tableLogOnInfo2.ConnectionInfo = connInfo;

                //foreach (CrystalDecisions.CrystalReports.Engine.Table table in rc.Database.Tables)
                //{
                //    table.ApplyLogOnInfo(tableLogOnInfo2);
                //    table.LogOnInfo.ConnectionInfo.ServerName = connInfo.ServerName;
                //    table.LogOnInfo.ConnectionInfo.DatabaseName = connInfo.DatabaseName;
                //    table.LogOnInfo.ConnectionInfo.UserID = connInfo.UserID;
                //    table.LogOnInfo.ConnectionInfo.Password = connInfo.Password;
                //}

                //rc.SetDatabaseLogon("DB_A2A9A5_snova786_admin", "snova@786", "sql7001.smarterasp.net", "DB_A2A9A5_snova786");
                //Stream stream = rc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                //return File(stream, "application/pdf");



                System.Data.DataTable dt = new System.Data.DataTable();


                SqlConnection conn = new SqlConnection(cs);

                conn.Open();
                //SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = new SqlCommand(UpdatedQuery, conn);

                ReportDocument rd = new ReportDocument();

                sda.Fill(dt);

                rd.Load(Server.MapPath("/CrystalReports/" + rptfilename + ".rpt"));

                rd.SetDataSource(dt);

                /* Comment by Moiz */

                rd.SetDatabaseLogon("DB_A2A9A5_snova786_admin", "snova@786", "sql7001.smarterasp.net", "DB_A2A9A5_snova786");
                CrystalDecisions.Shared.ConnectionInfo connInfo = new CrystalDecisions.Shared.ConnectionInfo();
                connInfo.ServerName = "sql7001.smarterasp.net";
                connInfo.DatabaseName = "DB_A2A9A5_snova786";
                connInfo.UserID = "DB_A2A9A5_snova786_admin";
                connInfo.Password = "snova@786";

                rd.DataSourceConnections.Clear();

                TableLogOnInfo tableLogOnInfo2 = new TableLogOnInfo();
                tableLogOnInfo2.ConnectionInfo = connInfo;

                foreach (CrystalDecisions.CrystalReports.Engine.Table table in rd.Database.Tables)
                {
                    table.ApplyLogOnInfo(tableLogOnInfo2);
                    table.LogOnInfo.ConnectionInfo.ServerName = connInfo.ServerName;
                    table.LogOnInfo.ConnectionInfo.DatabaseName = connInfo.DatabaseName;
                    table.LogOnInfo.ConnectionInfo.UserID = connInfo.UserID;
                    table.LogOnInfo.ConnectionInfo.Password = connInfo.Password;
                }

                rd.SetDatabaseLogon("DB_A2A9A5_snova786_admin", "snova@786", "sql7001.smarterasp.net", "DB_A2A9A5_snova786");


                /* Comment by Moiz */

                Stream streams = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                
                return File(streams, "application/pdf");




                /* Comment by Moiz */

                //string query2 = "select cryrptfile.rptfile from cryrptfile inner join crydtl on cryrptfile.reptid = crydtl.reptid where crydtl.reptid ='" + reportid + "'";
                //SqlCommand cmd2 = new SqlCommand(query2, con);
                //string loadvalue = cmd2.ExecuteScalar().ToString();

                //string VReportPath = Server.MapPath("~/TemporaryReports/"); //4

                //FileStream fs = null;

                //if (!System.IO.File.Exists(VReportPath + "\\" + xmlfilename))
                //{
                //    fs = new FileStream(VReportPath + "\\" + xmlfilename, FileMode.OpenOrCreate);
                //}
                //else
                //{
                //    System.IO.File.Delete(VReportPath + "\\" + xmlfilename);
                //    fs = new FileStream(VReportPath + "\\" + xmlfilename, FileMode.OpenOrCreate);
                //}


                //byte[] info = new UTF8Encoding(true).GetBytes(loadvalue);
                //fs.Write(info, 0, info.Length);

                //ADODB.Recordset vrs = new ADODB.Recordset();

                //int vlength = 0;

                //byte[] vfilearr = null;
                //fs.Close();

                //object oMissing = System.Reflection.Missing.Value;

                //vrs.Open(VReportPath + "\\" + xmlfilename, oMissing);

                //foreach (ADODB.Field vfield in vrs.Fields)
                //{
                //    vlength = vfield.ActualSize;
                //    vfilearr = (byte[])vfield.GetChunk(vlength); 

                //    break;
                //}

                //System.IO.FileStream fss = null;
                //if (!System.IO.File.Exists(VReportPath + "\\" + rptfilename))
                //{
                //    fss = new FileStream(VReportPath + "\\" + rptfilename, FileMode.OpenOrCreate);
                //}
                //else
                //{
                //    System.IO.File.Delete(VReportPath + "\\" + rptfilename);
                //    fss = new FileStream(VReportPath + "\\" + rptfilename, FileMode.OpenOrCreate);
                //}

                //fss.Write(vfilearr, 0, vlength);
                //fss.Close();
                ////System.Data.DataTable dt = new System.Data.DataTable();


                //SqlConnection conn2 = new SqlConnection(cs);

                //conn2.Open();
                ////SqlDataAdapter sda = new SqlDataAdapter();
                //sda.SelectCommand = new SqlCommand(UpdatedQuery, conn2);

                //ReportDocument rd = new ReportDocument();

                //sda.Fill(dt);

                //string checkpath = VReportPath + "\\" + rptfilename;

                //rd.Load(VReportPath + "\\" + rptfilename);

                //rd.SetDataSource(dt);

                /* Comment by Moiz */

                //rd.SetDatabaseLogon("DB_A2A9A5_snova786_admin", "snova@786", "sql7001.smarterasp.net", "DB_A2A9A5_snova786");
                //CrystalDecisions.Shared.ConnectionInfo connInfo = new CrystalDecisions.Shared.ConnectionInfo();
                //connInfo.ServerName = "sql7001.smarterasp.net";
                //connInfo.DatabaseName = "DB_A2A9A5_snova786";
                //connInfo.UserID = "DB_A2A9A5_snova786_admin";
                //connInfo.Password = "snova@786";

                //rd.DataSourceConnections.Clear();

                //TableLogOnInfo tableLogOnInfo2 = new TableLogOnInfo();
                //tableLogOnInfo2.ConnectionInfo = connInfo;

                //foreach (CrystalDecisions.CrystalReports.Engine.Table table in rd.Database.Tables)
                //{
                //    table.ApplyLogOnInfo(tableLogOnInfo2);
                //    table.LogOnInfo.ConnectionInfo.ServerName = connInfo.ServerName;
                //    table.LogOnInfo.ConnectionInfo.DatabaseName = connInfo.DatabaseName;
                //    table.LogOnInfo.ConnectionInfo.UserID = connInfo.UserID;
                //    table.LogOnInfo.ConnectionInfo.Password = connInfo.Password;
                //}

                //rd.SetDatabaseLogon("DB_A2A9A5_snova786_admin", "snova@786", "sql7001.smarterasp.net", "DB_A2A9A5_snova786");


                /* Comment by Moiz */

                //Stream streams = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                //System.IO.File.Delete(VReportPath + "\\" + rptfilename);
                //System.IO.File.Delete(VReportPath + "\\" + xmlfilename);
                //return File(streams, "application/pdf");

                /* Comment by Moiz */
            }

        }

        public ActionResult ExecuteReportTest()
        {

            //TempData.Keep("ReportId");
            //TempData.Keep("UpdatedQuery");

            //string ReportsId = TempData["ReportId"].ToString();
            //string reportname;
            //using (SqlConnection con1 = new SqlConnection(cs))
            //{
            //    string query = "SELECT reptname FROM crydtl WHERE(reptid = '" + ReportsId + "')";
            //    con1.Open();
            //    SqlCommand cmd = new SqlCommand(query, con1);
            //    reportname = cmd.ExecuteScalar().ToString();
            //    con1.Close();
            //}

            //DataTable dt = new DataTable();
            //using (SqlConnection conn = new SqlConnection(cs))
            //{
            //    conn.Open();
            //    string UpdateQuery = TempData["UpdatedQuery"].ToString();
            //    SqlDataAdapter sda = new SqlDataAdapter();
            //    sda.SelectCommand = new SqlCommand(UpdateQuery, conn);
            //    sda.Fill(dt);
            //    conn.Close();
            //}

            string reportname = Session["ReportId"].ToString();
            ReportDocument rc = new ReportDocument();
             
            rc.Load(Server.MapPath("/CrystalReports/" + reportname + ".rpt"));

            //rc.SetDataSource(dt);
            
            rc.SetDatabaseLogon("DB_A2A9A5_snova786_admin", "snova@786", "sql7001.smarterasp.net", "DB_A2A9A5_snova786");
            Stream stream = rc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            return File(stream, "application/pdf");

           
        }
    }
}