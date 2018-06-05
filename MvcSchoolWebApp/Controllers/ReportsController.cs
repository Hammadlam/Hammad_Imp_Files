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
using System.Collections;

//System.Reflection.Missing.value;

namespace MvcSchoolWebApp.Controllers
{
    public class ReportsController : Controller
    {
        public static string rptid;
        public static string updatedqry;
        // GET: Reports
        //public ActionResult Index()
        //{
        //    return View();
        //}
        MessageCls msgobj = new MessageCls();
        int getParam;
        string getSquery = string.Empty;

        #region Reports
        [HttpGet]
        public ActionResult Index()
        {
            TempData.Keep("User_Role");
            return View();
        }

        //  string cs = ConfigurationManager.ConnectionStrings["smarter"].ConnectionString;

        // string cs = ConfigurationManager.ConnectionStrings["smarter2"].ConnectionString;

        //string cs = ConfigurationManager.ConnectionStrings["AIGroup"].ConnectionString;
        string cs = ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString;

        //Data Source = wdb1.my - hosting - panel.com; Initial Catalog = snova786_ermfalcon; User ID = snova786_falcon; Password=sns123
        // string cs = "Data Source =wdb1.my-hosting-panel.com; Initial Catalog =snova786_ermfalcon; User ID =snova786_falcon; Password=sns123";

        public string getsquery(string reportid)
        {
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            string query = "SELECT hdr.squery,hdr.sfortotal,dtl.reptname FROM cryquehdr hdr inner join crydtl dtl on dtl.reptid=hdr.sforid WHERE sforid = '" + reportid + "'";
            SqlDataAdapter sda = new SqlDataAdapter(query, con);
            DataSet dss = new DataSet();
            sda.Fill(dss);
            return dss.Tables[0].Rows[0]["squery"].ToString();
        }

        public string getupdatedquery(string squery, string[] param)
        {
            string UpdatedQuery ="";
            for (int i = 0; i<param.Length; i++)
            {
                if (param[i] != "")
                {
                    UpdatedQuery = squery.Replace("%" + i + "%", "'" + param[i] + "'");

                    squery = UpdatedQuery;
                    UpdatedQuery = UpdatedQuery.Replace("\r\n", " ");
                    UpdatedQuery = UpdatedQuery.Replace("\n", " ");
                    UpdatedQuery = UpdatedQuery.Replace("\t", " ");
                    UpdatedQuery = UpdatedQuery.Replace("\r", " ");
            }
                else
                {
                UpdatedQuery = squery;
                UpdatedQuery = UpdatedQuery.Replace("\r\n", " ");
                UpdatedQuery = UpdatedQuery.Replace("\n", " ");
                UpdatedQuery = UpdatedQuery.Replace("\t", " ");
                UpdatedQuery = UpdatedQuery.Replace("\r", " ");
            }
        }
            return Query_Edit_after_Filldata(UpdatedQuery);
        }

        [HttpPost]
        public ActionResult Index(string reportid)
        {


            // ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text", "Func()", true);
            // ClientScript.RegisterStartupScript(GetType(), "id", "callMyJSFunction()", true);

            TempData.Keep("User_Role");

            using (SqlConnection con = new SqlConnection(cs))
            {
                //SELECT hdr.squery,hdr.sfortotal,dtl.reptname FROM cryquehdr hdr inner join crydtl dtl on dtl.reptid=hdr.sforid WHERE sforid = 'gl001'

                //string query = "SELECT squery,sfortotal FROM cryquehdr WHERE sforid = '" + reportid + "'";

                string query = "SELECT hdr.squery,hdr.sfortotal,dtl.reptname FROM cryquehdr hdr inner join crydtl dtl on dtl.reptid=hdr.sforid WHERE sforid = '" + reportid + "'";
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                DataSet dss = new DataSet();
                sda.Fill(dss);

                getSquery = dss.Tables[0].Rows[0]["squery"].ToString();
                getParam = Convert.ToInt32(dss.Tables[0].Rows[0]["sfortotal"]);
                ViewBag.ReportName = dss.Tables[0].Rows[0]["reptname"].ToString();

                TempData["Num-of-Param"] = getParam;
                TempData["Squery"] = getSquery;
                TempData["ReportId"] = reportid;

            }

            //dynamic fields generate into another page code below

            //Response.Redirect("/Home/ParamView");
            // Response.Redirect("/Home/ParamView");

            //dynamic fields generate into same page code below

            TempData.Keep("User_Role");
            TempData.Keep("Num-of-Param");
            int ParamRange = Convert.ToInt32(TempData["Num-of-Param"]);
            TempData.Keep("Squery");
            TempData.Keep("ReportId");


            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string query = "select sforfieldid from cryquedtl where sforid='" + TempData["ReportId"].ToString() + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                sda.Fill(ds);
            }

            string[] labelText = new string[ParamRange];

            for (int i = 0; i < labelText.Length; i++)
            {
                labelText[i] = ds.Tables[0].Rows[i]["sforfieldid"].ToString();
            }

            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < ParamRange; j++)
            {

                //string value = "<div style='margin:2% 2% 2% 2%'> <label> " + labelText[j] + " </label><input style='width:10%; margin-left:5%;' type='text' id='txt" + j + "' name='txt" + j + "'></div>";
                //Response.Write(value);


                sb.Append("<label> " + labelText[j] + " </label><input type='text' required class='form-control' id='txt" + j + "' placeholder='" + labelText[j] + "' name='txt" + j + "'>");
                //  ViewBag.DynamicControls = sb.ToString();
            }

            ViewBag.DynamicControls = sb.ToString();
            ViewBag.showsection = "showsection();";
            return View();

        }


        [HttpGet]
        public JsonResult SendParam()
        {

            TempData.Keep("User_Role");
            TempData.Keep("Num-of-Param");
            TempData.Keep("Squery");
            TempData.Keep("ReportId");

            string sendParam = TempData["Num-of-Param"].ToString();
            return Json(sendParam, JsonRequestBehavior.AllowGet);

        }

        public void getValues(string getvalues)
        {


            TempData.Keep("Num-of-Param");
            TempData.Keep("Squery");
            TempData.Keep("ReportId");

            ///*** 19Sept ***////// 

            //string get = getvalues;

            //string setSquery = TempData["Squery"].ToString();

            //int TotalParam = Convert.ToInt32(TempData["Num-of-Param"]);
            //string[] getSqueryArr = get.Split(',');

            //if (TotalParam == getSqueryArr.Length)
            //{

            //    string UpdatedQuery;
            //    for (int i = 0; i < getSqueryArr.Length; i++)
            //    {
            //        UpdatedQuery = setSquery.Replace("%" + i + "%", "'" + getSqueryArr[i] + "'");

            //        setSquery = UpdatedQuery;

            //        UpdatedQuery = UpdatedQuery.Replace("\r\n", " ");
            //        UpdatedQuery = UpdatedQuery.Replace("\t", " ");
            //        UpdatedQuery = UpdatedQuery.Replace("\r", " ");

            //        TempData["UpdatedQuery"] = UpdatedQuery;

            //    }

            //    // Response.Redirect("/Home/ExecuteReport");
            //    Response.Redirect("~/Reports/ExecuteReport"); 
            //} 

            ///*** 19Sept ***////// 

            string setSquery = TempData["Squery"].ToString();
            int TotalParam = Convert.ToInt32(TempData["Num-of-Param"]);
            string get = getvalues;


            string[] getSqueryArr = get.Split(',');

            if (TotalParam == getSqueryArr.Length)
            {

                string UpdatedQuery = null;

                for (int i = 0; i < getSqueryArr.Length; i++)
                {

                    if (getSqueryArr[i] != "")
                    {
                        UpdatedQuery = setSquery.Replace("%" + i + "%", "'" + getSqueryArr[i] + "'");

                        setSquery = UpdatedQuery;
                        UpdatedQuery = UpdatedQuery.Replace("\r\n", " ");
                        UpdatedQuery = UpdatedQuery.Replace("\n", " ");
                        UpdatedQuery = UpdatedQuery.Replace("\t", " ");
                        UpdatedQuery = UpdatedQuery.Replace("\r", " ");
                    }
                    else
                    {
                        //setSquery = UpdatedQuery;
                        UpdatedQuery = setSquery;
                        UpdatedQuery = UpdatedQuery.Replace("\r\n", " ");
                        UpdatedQuery = UpdatedQuery.Replace("\n", " ");
                        UpdatedQuery = UpdatedQuery.Replace("\t", " ");
                        UpdatedQuery = UpdatedQuery.Replace("\r", " ");
                    }

                    //UpdatedQuery = setSquery.Replace("%" + i + "%", "'" + getSqueryArr[i] + "'");
                    //setSquery = UpdatedQuery;
                    //UpdatedQuery = UpdatedQuery.Replace("\r\n", " ");
                    //UpdatedQuery = UpdatedQuery.Replace("\t", " ");
                    //UpdatedQuery = UpdatedQuery.Replace("\r", " ");

                    TempData["UpdatedQuery"] = UpdatedQuery;
                }

                //Response.Redirect("~/Reports/ExecuteReport");

                string VSQL = TempData["UpdatedQuery"].ToString();
                Query_Edit_after_Filldata(VSQL);
                Response.Redirect("~/Reports/ExecuteReport");

            }
        }

        public ActionResult ParamView()
        {
            TempData.Keep("User_Role");


            TempData.Keep("Num-of-Param");
            int ParamRange = Convert.ToInt32(TempData["Num-of-Param"]);
            TempData.Keep("Squery");
            TempData.Keep("ReportId");

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string query = "select sforfieldid from cryquedtl where sforid='" + TempData["ReportId"].ToString() + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                sda.Fill(ds);
            }

            string[] labelText = new string[ParamRange];

            for (int i = 0; i < labelText.Length; i++)
            {
                labelText[i] = ds.Tables[0].Rows[i]["sforfieldid"].ToString();
            }

            for (int j = 0; j < ParamRange; j++)
            {


                string value = "<div style='margin:2% 2% 2% 2%'> <label> " + labelText[j] + " </label><input style='width:10%; margin-left:5%;' type='text' id='txt" + j + "' name='txt" + j + "'></div>";
                Response.Write(value);


            }

            return View();
        }
        public ActionResult ExecuteReport()
        {

            // ***** 13 Sept ***** 

            TempData.Keep("ReportId");
            TempData.Keep("UpdatedQuery");

            string reportid = TempData["ReportId"].ToString();
            string UpdatedQuery = TempData["UpdatedQuery"].ToString();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "select rptfile from crydtl where reptid = '" + reportid + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                string rptfilename = cmd.ExecuteScalar().ToString();
                //Getting Name of RPT file
                string xmlfilename = rptfilename.Replace(".rpt", ".xml");
                DataSet ds = new DataSet();

                string query2 = "select cryrptfile.rptfile from cryrptfile inner join crydtl on cryrptfile.reptid = crydtl.reptid where crydtl.reptid ='" + reportid + "'";
                SqlCommand cmd2 = new SqlCommand(query2, con);
                cmd2.CommandTimeout = 300;
                string loadvalue = cmd2.ExecuteScalar().ToString();
                //Getting xml text from cryquehdr 
                string VReportPath = Server.MapPath("~/TemporaryReports/");
                FileStream fs = null;

                if (!System.IO.File.Exists(VReportPath + "\\" + xmlfilename))
                {
                    fs = new FileStream(VReportPath + "\\" + xmlfilename, FileMode.OpenOrCreate);
                    //Creating Directory if not exist to save xml file temporarily
                }
                else
                {
                    System.IO.File.Delete(VReportPath + "\\" + xmlfilename);
                    fs = new FileStream(VReportPath + "\\" + xmlfilename, FileMode.OpenOrCreate);
                }

                //Converting xml text into RPT file
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
                    //Creating Directory if not exist to save RPT file temporarily
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
                sda.SelectCommand = new SqlCommand(UpdatedQuery, conn2); //UpdatedQuery contains report query with parameters

                ReportDocument rd = new ReportDocument();

                sda.Fill(dt);
                //Fill DataTable from UpdatedQuery

                string checkpath = VReportPath + "\\" + rptfilename;
                rd.Load(VReportPath + "\\" + rptfilename);
                for (int i = 0; i < rd.Database.Tables.Count; i++)
                {
                    rd.Database.Tables[i].SetDataSource(dt); //SetDataSource(dt);
                }

                //Passing DataTable to ReportDocument

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cs);
                string servername = builder.DataSource;
                string dbname = builder.InitialCatalog;
                string username = builder.UserID;
                string pass = builder.Password;
                //Passing DBConnection to ReportDocument
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
                    table.ApplyLogOnInfo(tableLogOnInfo2);
                    table.LogOnInfo.ConnectionInfo.ServerName = connInfo.ServerName;
                    table.LogOnInfo.ConnectionInfo.DatabaseName = connInfo.DatabaseName;
                    table.LogOnInfo.ConnectionInfo.UserID = connInfo.UserID;
                    table.LogOnInfo.ConnectionInfo.Password = connInfo.Password;
                }
                
                //int ObjIndex = 0;
                //object[] strarray = new object[150];
                //string originalformula;
                //foreach (FormulaFieldDefinition txtformulafield in rd.DataDefinition.FormulaFields)
                //{
                //    if (rd.DataDefinition.FormulaFields.Count > 0)
                //    {
                //        originalformula = txtformulafield.Text;
                //        for (int i=0; i<rd.DataDefinition.FormulaFields.Count-1;i++)
                //        {
                //            object[] a;
                //            if (originalformula != null)
                //            {
                //                string StrBegin = "{";
                //                string StrEnd = "}";
                //                Boolean boolstart = true;
                //                Boolean boolEnd = true;
                //                string forfields = originalformula;

                //                int iIndexOfBegin = forfields.IndexOf(StrBegin);
                //                if (iIndexOfBegin != -1)
                //                {
                //                    if (boolstart != false)
                //                    {
                //                        iIndexOfBegin -= StrBegin.Length;
                //                        forfields = forfields.Substring(iIndexOfBegin + StrBegin.Length);
                //                        int iEnd = forfields.IndexOf(StrEnd);
                //                        if (iEnd != -1)
                //                        {
                //                            if (boolEnd != false)
                //                            {
                //                                iEnd -= StrEnd.Length;
                //                                string strchk = "";
                //                                strchk = forfields.Substring(1, iEnd);
                //                                if (char.IsLetter(strchk[0]))
                //                                {
                //                                    strarray[ObjIndex] = forfields.Substring(1, iEnd) + ",";
                //                                    ObjIndex += 1;
                //                                }
                //                                if ((iEnd + StrEnd.Length) < forfields.Length)
                //                                {
                //                                    originalformula = forfields.Substring(iEnd+StrEnd.Length);
                //                                }

                //                            }
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                //might be cause of an error
                //string getformula = null;
                //int getindex;
                //foreach (string value in strarray)
                //{
                //    for (getindex =0; getindex<strarray.Length; getindex++)
                //    {
                //        getformula = value + strarray[getindex];
                //    }
                //}

                //string SqlQueryStr = UpdatedQuery;
                //SqlQueryStr = SqlQueryStr.Insert(7,getformula);

                //CrystalDecisions.ReportAppServer.DataDefModel.Database boDatabase;
                //CrystalDecisions.ReportAppServer.DataDefModel.CommandTable commandtable;

                //foreach (CrystalDecisions.ReportAppServer.DataDefModel.Table boTable in boDatabase.Tables)
                //{
                //    if (boTable.Name.ToLower() == "command")
                //    {
                //        commandtable = (CrystalDecisions.ReportAppServer.DataDefModel.CommandTable)boTable;
                //    }
                //}

                //commandtable.Name = "Demo";
                //commandtable.CommandText = SqlQueryStr;
                //rd.SetSQLCommandTable(connInfo, "Demo", SqlQueryStr);


                rd.SetDatabaseLogon(username, pass, servername, dbname);

                for (int i = 0; i < rd.Subreports.Count; i++)
                {
                    rd.Subreports[i].SetDatabaseLogon(username, pass, servername, dbname);
                }

                //rd.SetSQLCommandTable(connInfo,"demo",UpdatedQuery);
                //Passing DBConnection to SubReport
                //rd.RecordSelectionFormula = "{@Balances_000011111} = 0";

                Stream streams = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                //rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "D:\\Falcon House App\\123.pdf");
                System.IO.File.Delete(VReportPath + "\\" + rptfilename);
                System.IO.File.Delete(VReportPath + "\\" + xmlfilename);
                return File(streams, "application/pdf");
            }

        }

        public Stream getrptstream(string rptid, string updatedquery)
        {

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "select rptfile from crydtl where reptid = '" + rptid + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                string rptfilename = cmd.ExecuteScalar().ToString();
                //Getting Name of RPT file
                string xmlfilename = rptfilename.Replace(".rpt", ".xml");
                DataSet ds = new DataSet();

                string query2 = "select cryrptfile.rptfile from cryrptfile inner join crydtl on cryrptfile.reptid = crydtl.reptid where crydtl.reptid ='" + rptid + "'";
                SqlCommand cmd2 = new SqlCommand(query2, con);
                cmd2.CommandTimeout = 300;
                string loadvalue = cmd2.ExecuteScalar().ToString();
                //Getting xml text from cryquehdr 
                string VReportPath = System.Web.HttpContext.Current.Server.MapPath("~/TemporaryReports/");
                FileStream fs = null;

                if (!System.IO.File.Exists(VReportPath + "\\" + xmlfilename))
                {
                    fs = new FileStream(VReportPath + "\\" + xmlfilename, FileMode.OpenOrCreate);
                    //Creating Directory if not exist to save xml file temporarily
                }
                else
                {
                    System.IO.File.Delete(VReportPath + "\\" + xmlfilename);
                    fs = new FileStream(VReportPath + "\\" + xmlfilename, FileMode.OpenOrCreate);
                }

                //Converting xml text into RPT file
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
                    //Creating Directory if not exist to save RPT file temporarily
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
                sda.SelectCommand = new SqlCommand(updatedquery, conn2); //UpdatedQuery contains report query with parameters

                ReportDocument rd = new ReportDocument();

                sda.Fill(dt);
                //Fill DataTable from UpdatedQuery

                string checkpath = VReportPath + "\\" + rptfilename;
                rd.Load(VReportPath + "\\" + rptfilename);
                for (int i = 0; i < rd.Database.Tables.Count; i++)
                {
                    rd.Database.Tables[i].SetDataSource(dt); //SetDataSource(dt);
                }

                //Passing DataTable to ReportDocument

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cs);
                string servername = builder.DataSource;
                string dbname = builder.InitialCatalog;
                string username = builder.UserID;
                string pass = builder.Password;
                //Passing DBConnection to ReportDocument
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
                    table.ApplyLogOnInfo(tableLogOnInfo2);
                    table.LogOnInfo.ConnectionInfo.ServerName = connInfo.ServerName;
                    table.LogOnInfo.ConnectionInfo.DatabaseName = connInfo.DatabaseName;
                    table.LogOnInfo.ConnectionInfo.UserID = connInfo.UserID;
                    table.LogOnInfo.ConnectionInfo.Password = connInfo.Password;
                }
                
                rd.SetDatabaseLogon(username, pass, servername, dbname);

                for (int i = 0; i < rd.Subreports.Count; i++)
                {
                    rd.Subreports[i].SetDatabaseLogon(username, pass, servername, dbname);
                }
                
                Stream streams = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                
                System.IO.File.Delete(VReportPath + "\\" + rptfilename);
                System.IO.File.Delete(VReportPath + "\\" + xmlfilename);
                return streams;
            }

        }

        //Add Query_Edit_after_Filldata
        public string Query_Edit_after_Filldata(string VSQL)
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

                // if (Strings.Mid(VSQL, vpos, 1) == "%" | Strings.Mid(VSQL, vpos, 1) == "#")
                // if ((VSQL.Substring(vpos,1)== "%" )|| (VSQL.Substring(vpos, 1) == "#"))
                if ((VSQL.Substring(vpos, 1) == "%") || (VSQL.Substring(vpos, 1) == "#"))
                {
                    // if (Strings.Mid(VSQL, vpos + 2, 1) == "%" | Strings.Mid(VSQL, vpos + 2, 1) == "#")
                    if ((VSQL.Substring(vpos + 2, 1) == "%") || (VSQL.Substring(vpos + 2, 1) == "#"))
                    {
                        Array.Resize(ref Vpos_persent, Counter + 1);

                        //Vpos_persent(Counter) = Strings.Mid(VSQL, vpos, 3);
                        Vpos_persent[Counter] = VSQL.Substring(vpos, 3);
                        vcount = vcount + 1;
                        Counter = Counter + 1;
                        //code mofify by MI 17/05/2010 for hangle more then 10 criteria


                    }
                    //else if (Strings.Mid(VSQL, vpos + 3, 1) == "%" | Strings.Mid(VSQL, vpos + 3, 1) == "#")
                    else if ((VSQL.Substring(vpos + 3, 1) == "%") || (VSQL.Substring(vpos + 3, 1) == "#"))
                    {
                        // if (Information.IsNumeric(Strings.Mid(VSQL, vpos + 2, 1)))
                        if (VSQL.Substring(vpos + 2, 1).All(char.IsDigit))//VSQL.i Information.IsNumeric(VSQL.Substring(vpos + 2, 1))  )
                        {
                            Array.Resize(ref Vpos_persent, Counter + 1);


                            // Vpos_persent(Counter) = Strings.Mid(VSQL, vpos, 4);
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
            return VSQL;
        }
        public ActionResult FlexData(string reportid)
        {
            TempData.Keep("User_Role");
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT hdr.squery,hdr.sfortotal,dtl.reptname FROM cryquehdr hdr inner join crydtl dtl on dtl.reptid=hdr.sforid WHERE sforid = '" + reportid + "'";
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                DataSet dss = new DataSet();
                sda.Fill(dss);

                getSquery = dss.Tables[0].Rows[0]["squery"].ToString();
                getParam = Convert.ToInt32(dss.Tables[0].Rows[0]["sfortotal"]);
                ViewBag.ReportName = dss.Tables[0].Rows[0]["reptname"].ToString();

                TempData["Num-of-Param"] = getParam;
                TempData["Squery"] = getSquery;
                TempData["ReportId"] = reportid; 

            }

            //dynamic fields generate into another page code below

            //Response.Redirect("/Home/ParamView");
            // Response.Redirect("/Home/ParamView");

            //dynamic fields generate into same page code below

            TempData.Keep("User_Role");
            TempData.Keep("Num-of-Param");
            int ParamRange = Convert.ToInt32(TempData["Num-of-Param"]);
            TempData.Keep("Squery");
            TempData.Keep("ReportId");


            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string query = "select sforfieldid,txtposition,ismandt from cryquedtl where sforid='" + TempData["ReportId"].ToString() + "'";
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                sda.Fill(ds);
            }

            string[] labelText = new string[ParamRange];
            string[] txtposition = new string[ParamRange];
            string[] mandttextbox = new string[ParamRange];

            for (int i = 0; i < labelText.Length; i++)
            {
                labelText[i] = ds.Tables[0].Rows[i]["sforfieldid"].ToString();
                txtposition[i] = ds.Tables[0].Rows[i]["txtposition"].ToString();
                mandttextbox[i] = ds.Tables[0].Rows[i]["ismandt"].ToString();
            }

            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < ParamRange; j++)
            {
                sb.Append("<div class='row'>");
                sb.Append("<div class='col-md-12'>");
                sb.Append("<div class='col-md-4'>");
                if (mandttextbox[j].Trim() == "X")
                {
                    sb.Append("<label> " + labelText[j] + " </label><input type='text' required class='form-control' id='txt" + j + "' placeholder='" + labelText[j] + "' name='txt" + j + "'>");
                }
                else
                {
                    sb.Append("<label> " + labelText[j] + " </label><input type='text' class='form-control' id='txt" + j + "' placeholder='" + labelText[j] + "' name='txt" + j + "'>");
                }

                sb.Append("</div>");

                if ((j + 1) < ParamRange)
                {
                    if (txtposition[j + 1] == "X")
                    {
                        j++;
                        sb.Append("<div class='col-md-4'>");
                        if (mandttextbox[j].Trim() == "X")
                        {
                            sb.Append("<label> " + labelText[j] + " </label><input type='text' required class='form-control' id='txt" + j + "' placeholder='" + labelText[j] + "' name='txt" + j + "'>");
                        }
                        else
                        {
                            sb.Append("<label> " + labelText[j] + " </label><input type='text' class='form-control' id='txt" + j + "' placeholder='" + labelText[j] + "' name='txt" + j + "'>");
                        }
                        
                        sb.Append("</div>");
                        sb.Append("</div>");
                        sb.Append("</div>");
                    }
                }
                else
                {
                    sb.Append("</div>");
                    sb.Append("</div>");
                }
            }

            ViewBag.DynamicControls = sb.ToString();
            ViewBag.showsection = "showsection();";

            return View("Index");

        }
        #endregion

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

        [HttpPost]
        public string GetDataFromDB(string Moduleid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand("select ch.reptid as reportid,cd.reptname as reportname from cryhdr ch inner join crydtl cd on ch.reptid=cd.reptid where ch.moduleid='" + Moduleid + "'", con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);

                    }
                    var jsonSerialiser = new JavaScriptSerializer();
                    string jsonvalue = jsonSerialiser.Serialize(rows);

                    return jsonvalue;
                }
            }
        }
        public bool checksquery(string setSquery)
        {

            string check = setSquery;
            if (setSquery.Contains("between"))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public bool checknullfnc(string[] getSqueryArr)
        {
            bool set = false;
            for (int i = 0; i < getSqueryArr.Length; i++)
            {
                if (getSqueryArr[i] == "")
                {
                    set = true;
                }
                else
                {
                    set = false;
                    break;
                }
            }
            return set;
        }
    }
}