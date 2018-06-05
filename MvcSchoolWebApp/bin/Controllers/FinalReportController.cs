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
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace MvcSchoolWebApp.Controllers
{
    public class FinalReportController : Controller
    {
        MessageCls msgobj = new MessageCls();
        public static string user_role;
        public static string user_id;
        private string campusid;
        private string classid;
        private string sectionid;
        private string nameid;
        private string session;
        private string moduleid;
        private static string reportid;
        private string getSquery;
        private static string Updatedquery;
        string cs = ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString;
        DatabaeseClass db;
        private static Stream streams;
        public static List<Users> user_dtl;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Session["User_Dtl"] != null)
            {
                user_dtl = (List<Users>)HttpContext.Session["User_Dtl"];
                base.OnActionExecuting(filterContext);
                user_role = user_dtl[0].user_earea;
                user_id = user_dtl[0].user_id;
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "none")]
        public ActionResult FinalReport()
        {
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            List<SelectListItem> sl = new List<SelectListItem>();
            sl.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            db = new DatabaeseClass();
            DatabaseModel report = new DatabaseModel();

            report.campus = db.getcampus();
            report.campusid = report.campus[0].Value;

            report.classes = db.getclass(report.campus[0].Value, user_id);
            report.classesid = "";
            //report.classesid = report.classes[0].Value;

            report.section = sl;
            //report.section = db.getsection(report.campus[0].Value, report.classesid, user_id);
            //report.sectionid = report.section[0].Value;

            report.stdname = sl;
            //report.stdname = db.getstudentname(report.campus[0].Value, report.classesid, report.sectionid);
            //report.studentid = report.stdname[0].Value;
            
            report.module = db.FillSubModule();
            report.reports = sl;
            return View(report);
        }

        [HttpPost]
        public void Getsquery(string campusId, string classId, string sectionId, string name, string session, string moduleid, string reportid)
        {
            this.campusid = campusId;
            this.classid = classId;
            this.sectionid = sectionId;
            this.nameid = name;
            this.moduleid = moduleid;
            FinalReportController.reportid = reportid;

            getSquery = string.Empty;
            string[] ParamValues = new string[5];
            ParamValues[0] = campusid;
            ParamValues[1] = classid;
            ParamValues[2] = sectionid;
            ParamValues[3] = nameid;
            ParamValues[4] = moduleid;
            //ParamValues[5] = session;

            using (SqlConnection con = new SqlConnection(cs))
            {

                string query = "SELECT hdr.squery,dtl.reptname FROM cryquehdr hdr inner join crydtl dtl on dtl.reptid=hdr.sforid WHERE sforid = '" + reportid + "'";
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                DataSet dss = new DataSet();
                sda.Fill(dss);

                getSquery = dss.Tables[0].Rows[0]["squery"].ToString();
            }

            Replacesquery(ParamValues); //call fucntion to replace
        }

        public void Replacesquery(string[] ParamValues)
        {
            string UpdatedQuery = null;
            string setSquery = getSquery;
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

                VSQL = UpdatedQuery;
                //Query_Edit_after_Filldata(VSQL);
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

            Updatedquery = VSQL;
            Response.Redirect("~/FinalReport/ExecuteReport");

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

        public void ExecuteReport()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {

                string query = "select rptfile from crydtl where reptid = '" + reportid + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                string rptfilename = cmd.ExecuteScalar().ToString();
                string xmlfilename = rptfilename.Replace(".rpt", ".xml");
                //DataSet ds = new DataSet();

                string query2 = "select cryrptfile.rptfile from cryrptfile inner join crydtl on cryrptfile.reptid = crydtl.reptid where crydtl.reptid ='" + reportid + "'";
                SqlCommand cmd2 = new SqlCommand(query2, con);
                cmd2.CommandTimeout = 300;
                string loadvalue = cmd2.ExecuteScalar().ToString();
                con.Close();
                string VReportPath = Server.MapPath("~/TemporaryReports/"); //4

                FileStream fs = null;

                if (!System.IO.File.Exists(VReportPath + "\\" + xmlfilename))
                {
                    fs = new FileStream(VReportPath + "\\" + xmlfilename, FileMode.OpenOrCreate);
                }
                else
                {
                    System.IO.File.Delete(VReportPath + "\\" + xmlfilename);
                    fs = new FileStream(VReportPath + "\\" + xmlfilename, FileMode.OpenOrCreate);
                }


                byte[] info = new UTF8Encoding(true).GetBytes(loadvalue);
                fs.Write(info, 0, info.Length);

                ADODB.Recordset vrs = new ADODB.Recordset();

                int vlength = 0;

                byte[] vfilearr = null;
                fs.Close();

                object oMissing = System.Reflection.Missing.Value;

                vrs.Open(VReportPath + "\\" + xmlfilename, oMissing);

                foreach (ADODB.Field vfield in vrs.Fields)
                {
                    vlength = vfield.ActualSize;
                    vfilearr = (byte[])vfield.GetChunk(vlength);

                    break;
                }

                System.IO.FileStream fss = null;
                if (!System.IO.File.Exists(VReportPath + "\\" + rptfilename))
                {
                    fss = new FileStream(VReportPath + "\\" + rptfilename, FileMode.OpenOrCreate);
                }
                else
                {
                    System.IO.File.Delete(VReportPath + "\\" + rptfilename);
                    fss = new FileStream(VReportPath + "\\" + rptfilename, FileMode.OpenOrCreate);
                }

                fss.Write(vfilearr, 0, vlength);
                fss.Close();
                System.Data.DataTable dt = new System.Data.DataTable();
                SqlConnection conn2 = new SqlConnection(cs);
                conn2.Open();
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = new SqlCommand(Updatedquery, conn2);

                ReportDocument rd = new ReportDocument();
                sda.Fill(dt);
                rd.Load(VReportPath + "\\" + rptfilename);
                rd.SetDataSource(dt);

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cs);
                string servername = builder.DataSource;
                string dbname = builder.InitialCatalog;
                string username = builder.UserID;
                string pass = builder.Password;

                rd.SetDatabaseLogon(username, pass, servername, dbname);

                CrystalDecisions.Shared.ConnectionInfo connInfo = new CrystalDecisions.Shared.ConnectionInfo();
                connInfo.ServerName = servername;
                connInfo.DatabaseName = dbname;
                connInfo.UserID = username;
                connInfo.Password = pass;

                rd.DataSourceConnections.Clear();

                TableLogOnInfo tableLogOnInfo2 = new TableLogOnInfo();
                tableLogOnInfo2.ConnectionInfo = connInfo;

                foreach (CrystalDecisions.CrystalReports.Engine.Table table in rd.Database.Tables)
                {
                    table.LogOnInfo.ConnectionInfo.ServerName = connInfo.ServerName;
                    table.LogOnInfo.ConnectionInfo.DatabaseName = connInfo.DatabaseName;
                    table.LogOnInfo.ConnectionInfo.UserID = connInfo.UserID;
                    table.LogOnInfo.ConnectionInfo.Password = connInfo.Password;
                    table.ApplyLogOnInfo(tableLogOnInfo2);
                }

                rd.SetDatabaseLogon(username, pass, servername, dbname);

                for (int i = 0; i < rd.Subreports.Count; i++)
                {
                    rd.Subreports[i].SetDatabaseLogon(username, pass, servername, dbname);
                }

                streams = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                System.IO.File.Delete(VReportPath + "\\" + rptfilename);
                System.IO.File.Delete(VReportPath + "\\" + xmlfilename);
                conn2.Close();
            }
        }

        public ActionResult ViewReport()
        {
            return File(streams, "application/pdf");
        }

        public ActionResult ExecuteReportTest()
        {
            string query = "SELECT DISTINCT    ss.subjectid, ss.subjecttxt,    e17.empid,     sr.subresltyp, " +
                           "sr.testmarks, sr.oralmarks, sr.assignmentmarks, sr.obtainedmarks, ep.lastname, ep.firstname, " +
                           "sc.classtxt, scm.campustxt, s2.lvdays, s2.remarks, gk3.activitysymbol, gk4.activitysymbol, " +
                           "gk5.activitysymbol, gk6.activitysymbol, gk2.activitysymbol, gk1.activitysymbol FROM " +
                           "empmain emp  inner join emp0710 e17 on e17.empid = emp.empid " +
                           "inner join schcampus scm on scm.campusid = e17.campusid " +
                           "inner join emppers ep on ep.empid = e17.empid " +
                           "inner join schresult sr on sr.stdid = e17.empid " +
                           "right join schresltype sr2 on sr.subresltyp = sr2.subresltyp " +
                           "inner join schclass sc on sc.campusid = e17.campusid " +
                           "inner join schsubject ss on ss.subjectid = sr.subjectid " +
                           "left join schactivitygrade ag on emp.empid = ag.stdid " +
                           "left join std0220 s2 on s2.empid = sr.stdid " +
                           "left join schactivitygradekey gk1 on ag.sports = gk1.activityid " +
                           "left join schactivitygradekey gk2 on ag.assemblypresent = gk2.activityid " +
                           "left join schactivitygradekey gk3 on ag.gk = gk3.activityid " +
                           "left join schactivitygradekey gk4 on ag.behaviour = gk4.activityid " +
                           "left join schactivitygradekey gk5 on ag.discipline = gk5.activityid " +
                           "left join schactivitygradekey gk6 on ag.cleanliness = gk6.activityid " +
                           "where 1 = 1 and emp.delind <> 'X' and sr.delind <> 'X' and e17.delind <> 'X' and e17.empid = '00037' " +
                           "and sc.classid = 'S1' and sc.campusid = '1001' and sc.sectionid = 'A' and emp.earea = '9100' " +
                           "and sr.subresltyp = '1' ORDER BY    e17.empid ASC,    ss.subjectid ASC";

            SqlConnection con = new SqlConnection(cs);
            string reportname = Session["ReportId"].ToString();
            System.Data.DataTable dt = new System.Data.DataTable();
            con.Open();
            //rc.Load(Server.MapPath("/CrystalReports/" + reportname + ".rpt"));
            SqlDataAdapter sda = new SqlDataAdapter();
            sda.SelectCommand = new SqlCommand(query, con);
            ReportDocument rc = new ReportDocument();
            sda.Fill(dt);
            System.Data.DataSet ds = new System.Data.DataSet();
            ds.Tables.Add(dt);
            rc.Load(Server.MapPath("/TemporaryReports/" + reportname + ".rpt"));
            rc.DataSourceConnections.Clear();

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cs);
            string servername = builder.DataSource;
            string dbname = builder.InitialCatalog;
            string username = builder.UserID;
            string pass = builder.Password;

            rc.SetDatabaseLogon(username, pass, servername, dbname);
            rc.SetDataSource(ds);

            CrystalDecisions.Shared.ConnectionInfo connInfo = new CrystalDecisions.Shared.ConnectionInfo();
            connInfo.ServerName = servername;
            connInfo.DatabaseName = dbname;
            connInfo.UserID = username;
            connInfo.Password = pass;



            TableLogOnInfo tableLogOnInfo2 = new TableLogOnInfo();
            tableLogOnInfo2.ConnectionInfo = connInfo;

            foreach (CrystalDecisions.CrystalReports.Engine.Table table in rc.Database.Tables)
            {
                table.ApplyLogOnInfo(tableLogOnInfo2);
                table.LogOnInfo.ConnectionInfo.ServerName = connInfo.ServerName;
                table.LogOnInfo.ConnectionInfo.DatabaseName = connInfo.DatabaseName;
                table.LogOnInfo.ConnectionInfo.UserID = connInfo.UserID;
                table.LogOnInfo.ConnectionInfo.Password = connInfo.Password;
            }

            rc.SetDatabaseLogon(username, pass, servername, dbname);

            Stream stream = rc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            return File(stream, "application/pdf");


        }
    }
}