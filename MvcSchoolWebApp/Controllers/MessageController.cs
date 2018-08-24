using MvcSchoolWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcSchoolWebApp.Controllers
{
    public class MessageController : Controller
    {
        // GET: Message
        bool inprocess = false;
        public static string user_role;
        public static string user_id;
        public static string user_campus;
        public static string user_class;
        public static string user_section;
        public static string popup_status;
        public static List<Users> user_dtl;
        private static DateTime? last_msg_dt;
        MessageCls msgobj = new MessageCls();
        DatabaeseClass db;
        string cs = ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString.ToString();

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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DownloadAttachment(string path)
        {
            string file = path;
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            if (System.IO.File.Exists(file))
            {
                return File(file, contentType, Path.GetFileName(file));
            }
            else
            {
                return null;
            }
        }

        public ActionResult viewchat()
        {
            //Start testing
            MessageCls msgobj = new MessageCls();
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            try
            {
                string msgId = Session["MsgId"].ToString();
                string[] demovar = msgId.Split('?');
                msgId = demovar[0];
                string groupid = demovar[1];
                string recordno = demovar[2];
                ViewBag.DisplayMsg = DisplayMsg();

                using (SqlConnection con = new SqlConnection(cs))
                {
                    string query;
                    if (groupid == "" && recordno == "")
                    {
                        query = "update inbox set unread=' ' where msgid='" + msgId + "' and recip='" + user_id + "'";
                    }
                    else
                    {
                        query = "update schgrpinbox set unread=' ' where groupid = '" + groupid + "' and msgid='" + msgId + "' and recip='" + user_id + "'";
                    }
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                ViewBag.call_alert = "show_alert()";
                ViewBag.message_popup = "Your Session Expire Please Login Again";
                ViewBag.cssclass = "alert-danger";

            }

            return View();
        }

        public ActionResult GetMsgId(string sendmsgid)
        {
            string[] getdata = sendmsgid.Split('?');
            string msgId = getdata[0];
            SqlConnection con = new SqlConnection(cs);
            con.Open();
            SqlCommand cmd = new SqlCommand("select notfcase from inbox where msgid = '"+msgId+"'",con);
            string notfcase = cmd.ExecuteScalar().ToString().Trim();
            string url;
            if (notfcase == "")
            {
                Session["MsgId"] = sendmsgid;
                url = "/Message/viewchat";
            }
            else
            {
                cmd = new SqlCommand("select pagepath from znotificationcase where notfcase = '"+notfcase+"'", con);
                string pagepath = (string)cmd.ExecuteScalar() ?? "";

                switch (notfcase)
                {
                    case "HA":
                        Session["download"] = "Home Assignment";
                        break;
                    case "CA":
                        Session["download"] = "Class Assignment";
                        break;
                    case "P":
                        Session["download"] = "Project";
                        break;

                    case "PRWT":
                        Session["resulttype"] = "Weekly Test";
                        break;
                    case "PRPT":
                        Session["resulttype"] = "Periodical Test";
                        break;
                    case "PRMT":
                        Session["resulttype"] = "Mid/First Term";
                        break;
                    case "PRFT":
                        Session["resulttype"] = "Final Term";
                        break;
                }
                url = pagepath;
            }

            cmd = new SqlCommand("update inbox set unread = '' where msgid = '" + msgId + "' and notfcase = '" + notfcase + "' and recip = '" + user_id + "'", con);
            cmd.ExecuteNonQuery();

            return Json(url);
        }
        public void GetMsgDetail(string title, string msg, string sendTo, string groupid)
        {
            //Startup testing
            DatabaeseClass dc = new DatabaeseClass();
            MessageCls msgobj = new MessageCls();
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            string cs = ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString.ToString();
            string insertdate = dc.convertedinsertdate(DateTime.Now.ToString()).ToString();
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();
            try
            {

                string query = "select ISNULL(MAX(chatviewid),0)+1 from chatview";
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = query;
                cmd.Transaction = trans;
                string chatviewId = cmd.ExecuteScalar().ToString();
                if (sendTo != "" && groupid == "")
                {

                    cmd.CommandText = "insert into inbox(msgid,recordno,subject,message,sender,recip,cc,unread,status,dbtimestmp,chatviewid) values((select ISNULL(MAX(msgid),0)+1 from inbox),'1','" + title + "',@message,'" + user_id + "','" + sendTo + "',' ','X',' ','" + insertdate + "','" + chatviewId + "')";
                    cmd.Parameters.AddWithValue("@message",msg);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "insert into chatview (chatviewid,msgid,userid,Isview) values('" + chatviewId + "',(select ISNULL(MAX(msgid),0) from inbox),'" + user_id + "',' ')";
                    cmd.ExecuteNonQuery();


                    cmd.CommandText = "insert into chatview (chatviewid,msgid,userid,Isview) values('" + chatviewId + "',(select ISNULL(MAX(msgid),0) from inbox),'" + sendTo + "',' ')";
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                else
                {
                    cmd.CommandText = "select count(empid) from schgrpdtl where groupid = '" + groupid + "'";
                    int countid = Convert.ToInt16(cmd.ExecuteScalar());
                    if (countid > 0)
                    {
                        string[] totalid = new string[countid];
                        cmd.CommandText = "select empid from schgrpdtl where groupid = '" + groupid + "'";
                        SqlDataReader sdr;
                        sdr = cmd.ExecuteReader();
                        if (sdr.HasRows)
                        {
                            int i = 0;
                            while (sdr.Read())
                            {
                                totalid[i] = Convert.ToString(sdr["empid"]).Trim();
                                i++;
                            }
                            sdr.Close();
                        }

                        cmd.CommandText = "select isnull(max(recordno),0)+01 from schgrpinbox where groupid = '" + groupid + "'";
                        int recordno = Convert.ToInt16(cmd.ExecuteScalar());
                        for (int i = 0; i < totalid.Length; i++)
                        {
                            cmd.CommandText = "insert into schgrpinbox(groupid, msgid, recordno, sender, recip, status, unread, dbtimestmp) values('" + groupid + "', '1', '" + recordno + "', '" + user_id + "', '" + totalid[i] + "', ' ', 'X', '"+insertdate+"')";
                            cmd.ExecuteNonQuery();
                        }

                        cmd.CommandText = "insert into schgrpmsg(groupid, msgid, recordno, subject, msgtxt) values ('" + groupid + "', '1', '" + recordno + "', '" + title + "', '" + msg + "')";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "update schgrpinbox set unread = '' where groupid = '" + groupid + "' and recordno = '" + recordno + "' and recip = '" + user_id + "'";
                        cmd.ExecuteNonQuery();
                        trans.Commit();

                    }
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                ViewBag.call_alert = "show_alert()";
                ViewBag.message_popup = "Found Some Error! Please Try Again";
                ViewBag.cssclass = "alert-danger";
            }
            finally
            {
                conn.Close();
                trans.Dispose();
            }
        }

        public List<MessageCls> DisplayMsg()
        {

            List<MessageCls> Msglst = new List<MessageCls>();
            DatabaeseClass dc = new DatabaeseClass();
            MessageCls msgobj = new MessageCls();
            ViewBag.msglist = msgobj.GetNotifications();
            ViewBag.TotalNotification = msgobj.NumberofNotifications();
            string msgId = Session["MsgId"].ToString();
            string[] getdata = msgId.Split('?');
            msgId = getdata[0];
            string groupid = getdata[1];
            string recordno = getdata[2];
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query;
                if (groupid == "" && recordno == "")
                {
                    //query = "select distinct msgid, message, inbox.recordno, subject, img.imagepath, (Select top(1)(CONVERT(varchar, inbox.dbtimestmp, 109)) from inbox where msgid = '" + msgId+"' and status <> 'X') as Date, DATENAME(weekday, inbox.dbtimestmp) as Day , (select count(msgid) from inbox where msgid = '"+msgId+ "' and status<> 'X') as countRecord, sender, (ep.firstname + ' ' + ep.lastname) as empname, (sp.firstname + ' ' + sp.lastname) as stdname from inbox left join emppers ep on ep.empid = inbox.sender left join stdpers sp on sp.stdid = inbox.sender left join emp0170 as e17 on e17.empid = inbox.sender  left join imageobj as img on e17.imageid = img.imageid  where msgid = '" + msgId+"' and status<> 'X' group by msgid,message,subject,inbox.dbtimestmp,sender,inbox.recordno,status,ep.firstname,ep.lastname, sp.firstname, sp.lastname, img.imagepath order by recordno ASC";
                    query = " select msgid, message, inbox.recordno, subject, img.imageobj, inbox.dbtimestmp  as Date, "+
                            "inbox.filepath, (select count(msgid) from inbox "+
                            " where msgid = '"+msgId+"' and status <> 'X') as countRecord, sender, "+
                            "(ep.firstname + ' ' + ep.lastname) as empname, (sp.firstname + ' ' + sp.lastname) as stdname from inbox "+
                            " left join emppers ep on ep.empid = inbox.sender "+
                            " left join stdpers sp on sp.stdid = inbox.sender "+                                                                                        
                            "left join emp0170 as e17 on e17.empid = inbox.sender "+
                            "left join imageobj as img on e17.imageid = img.imageid "+
                            " where msgid = '"+msgId+"' and status<> 'X' and ep.delind <> 'X' and(e17.delind <> 'X' OR e17.delind is null) "+
                            " order by recordno ASC";
                }
                else
                {
                    query = "select distinct ib.sender, (ep.firstname + ' ' + ep.lastname) as empname, (sp.firstname + ' '+ sp.lastname) as stdname, img.imagepath, dtl.grouptxt, msg.subject, msg.filepath, ib.recordno, msg.msgtxt as message, ib.dbtimestmp as Date, (select count(distinct recordno) from schgrpinbox where msgid = '1' and groupid = '100001' and status <> 'X'  and recip = '30121') as countRecord from schgrpmsg msg inner join schgrpinbox ib on msg.groupid = ib.groupid and msg.msgid = ib.msgid and ib.recordno = msg.recordno inner join schgrpdtl dtl on dtl.groupid = msg.groupid left join stdpers sp on sp.stdid = ib.sender left join emppers ep on ep.empid = ib.sender  left join emp0170 as e17 on e17.empid = ib.sender  left join imageobj as img on e17.imageid = img.imageid where msg.groupid = '" + groupid+"' and msg.msgid = '"+msgId+"' and ib.recip = '"+user_id+"' and ib.status <> 'X' order by ib.dbtimestmp ASC";
                }
                SqlDataAdapter sdr = new SqlDataAdapter(query, con);
                con.Open();
                DataSet ds = new DataSet();
                sdr.Fill(ds);
                int countrecord = Convert.ToInt32(ds.Tables[0].Rows[0]["countRecord"]);
                ViewBag.Subject = ds.Tables[0].Rows[ds.Tables[0].Rows.Count-1]["subject"].ToString();
                if (groupid == "" && recordno == "")
                {
                    ViewBag.Sender = string.IsNullOrEmpty(ds.Tables[0].Rows[0]["empname"].ToString()) ? ds.Tables[0].Rows[0]["stdname"].ToString() : ds.Tables[0].Rows[0]["empname"].ToString();
                }
                else
                {
                    ViewBag.Sender = ds.Tables[0].Rows[0]["grouptxt"].ToString();
                }
                for (int i = 0; i < countrecord; i++)
                {
                    DateTime dbtimestamp = dc.converteddisplaydate(ds.Tables[0].Rows[i]["date"].ToString());
                    string date = dbtimestamp.ToString("MMM dd yyyy h:mm tt");
                    string day = dbtimestamp.ToString("dddd");
                    string imgsrc = "";
                    if (ds.Tables[0].Rows[i]["imageobj"].ToString() != "")
                    {
                        byte[] header = (byte[])ds.Tables[0].Rows[i]["imageobj"];
                        var base64 = Convert.ToBase64String(header);
                        imgsrc = string.Format("data:image/gif;base64,{0}", base64);
                    }
                    
                    Msglst.Add(new MessageCls()
                    {
                        Message = ds.Tables[0].Rows[i]["message"].ToString(),
                        MsgDate = day + " " + date,
                        UserName = string.IsNullOrEmpty(ds.Tables[0].Rows[i]["empname"].ToString()) ? ds.Tables[0].Rows[i]["stdname"].ToString() : ds.Tables[0].Rows[i]["empname"].ToString(),
                        imgpath = string.IsNullOrEmpty(imgsrc) ? "~/Content/Avatar/avatar2.png" : imgsrc,
                        filepath = ds.Tables[0].Rows[i]["filepath"].ToString(),
                        RecordNo = ds.Tables[0].Rows[i]["recordno"].ToString()
                    });
                }
                return Msglst;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ReplyMessage(string message)
        {
            string path = "";
            Stream stream;
            

            string MsgId = Session["MsgId"].ToString();
            string[] getdata = MsgId.Split('?');
            MsgId = getdata[0];
            string groupid = getdata[1];
            string recordno = getdata[2];
            string Msgsub;
            string sender = user_id;
            DatabaeseClass dc = new DatabaeseClass();
            string insertdate = dc.convertedinsertdate(DateTime.Now.ToString()).ToString();
            string increment = "";
            

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query2 = "select dbo.GetReceiverId('" + sender + "','" + MsgId + "')";
                con.Open();
                SqlCommand cmd2;
                string receiver;
                if (recordno == "" && groupid == "")
                {
                    cmd2 = new SqlCommand(query2, con);
                    receiver = cmd2.ExecuteScalar().ToString();
                    string record = "select isnull(max(recordno),0)+1 from inbox where msgid='" + MsgId + "'";
                    cmd2 = new SqlCommand(record, con);
                    string insrecord = cmd2.ExecuteScalar().ToString();
                    increment = insrecord + MsgId;

                    if (Request.Files.Count > 0)
                    {
                        try
                        {
                            foreach (string file in Request.Files)
                            {
                                var fileContent = Request.Files[file];
                                if (fileContent != null)
                                {
                                    if (fileContent.ContentLength <= 25000000)
                                    {
                                        // get a stream
                                        stream = fileContent.InputStream;
                                        // and optionally write the file to disk
                                        var fileName = fileContent.FileName;
                                        path = Server.MapPath("~/Uploads/MessageAttachments/");
                                        if (!Directory.Exists(path))
                                        {
                                            Directory.CreateDirectory(path);
                                        }
                                        path = path + increment + fileName;
                                        fileContent.SaveAs(path);
                                    }
                                    else
                                    {
                                        return Json("File size must be less than 25MB");
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            return Json("Upload failed");
                        }
                    }
                    string selectIsview = "select rtrim(Isview) from chatview where msgId='" + MsgId + "' and userid='" + receiver + "'";
                    SqlCommand cmdisview = new SqlCommand(selectIsview, con);
                    string checkisview = cmdisview.ExecuteScalar().ToString();
                    if (checkisview != "X")
                    {
                        string query = "select subject from inbox where msgid='" + MsgId + "'";
                        SqlCommand cmd = new SqlCommand(query, con);
                        Msgsub = cmd.ExecuteScalar().ToString();
                        string query3 = "insert into inbox(msgid,recordno,subject,message,sender,recip,cc,unread,status,dbtimestmp,chatviewid,filepath,notfcase) values(@msgid,'"+ insrecord + "',@msgsub,@message,'" + sender + "','" + receiver + "', ' ', 'X', ' ', '" + insertdate + "', '" + MsgId + "','"+path+"','')";
                        SqlCommand cmd3 = new SqlCommand(query3, con);
                        cmd3.Parameters.AddWithValue("@msgid", MsgId);
                        cmd3.Parameters.AddWithValue("@msgsub",Msgsub);
                        cmd3.Parameters.AddWithValue("@message", message);
                        cmd3.ExecuteNonQuery();

                        ViewBag.DisplayMsg = DisplayMsg();
                        Response.Redirect("~/Message/viewchat");
                    }
                    else
                    {
                        ViewBag.call_alert = "show_alert()";
                        ViewBag.message_popup = "Your Recipient Has Closed This Discussion, Please Create a New Message";
                        ViewBag.cssclass = "alert-info";
                        ViewBag.DisplayMsg = DisplayMsg();
                        Response.Redirect("~/Message/viewchat");
                    }
                }
                else
                {
                    string countrecip = "select count(empid) from schgrpdtl where groupid = '" + groupid + "'";
                    string allrecip = "select empid from schgrpdtl where groupid = '" + groupid + "'";
                    cmd2 = new SqlCommand(countrecip, con);
                    int count = Convert.ToInt16(cmd2.ExecuteScalar());
                    cmd2 = new SqlCommand(allrecip, con);

                    SqlDataReader sdr;
                    sdr = cmd2.ExecuteReader();
                    string[] totalids = new string[count];
                    if (sdr.HasRows)
                    {
                        int i = 0;
                        while (sdr.Read())
                        {
                            totalids[i] = sdr["empid"].ToString().Trim();
                            i++;
                        }
                        sdr.Close();
                    }

                    string record = "select isnull(max(recordno),0)+01 from schgrpinbox where groupid = '" + groupid + "'";
                    cmd2 = new SqlCommand(record, con);
                    string insrecord = cmd2.ExecuteScalar().ToString();
                    increment = groupid + MsgId + insrecord;

                    if (Request.Files.Count > 0)
                    {
                        try
                        {
                            foreach (string file in Request.Files)
                            {
                                var fileContent = Request.Files[file];
                                if (fileContent != null)
                                {
                                    if (fileContent.ContentLength <= 25000000)
                                    {
                                        // get a stream
                                        stream = fileContent.InputStream;
                                        // and optionally write the file to disk
                                        var fileName = Path.GetFileName(file);
                                        path = Server.MapPath("~/Uploads/MessageAttachments/");
                                        if (!Directory.Exists(path))
                                        {
                                            Directory.CreateDirectory(path);
                                        }
                                        path = path + increment + fileName;
                                        fileContent.SaveAs(path);
                                    }
                                    else
                                    {
                                        return Json("File size must be less than 25MB");
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            return Json("Upload failed");
                        }
                    }

                    SqlTransaction trans = con.BeginTransaction();
                    cmd2.Transaction = trans;
                    try
                    {
                        cmd2.CommandText = "select max(recordno)+01 from schgrpinbox where groupid = '" + groupid + "'";
                        int incrementno = Convert.ToInt16(cmd2.ExecuteScalar());
                        for (int i = 0; i < totalids.Length; i++)
                        {
                            cmd2.CommandText = "insert into schgrpinbox (groupid, msgid, recordno, sender, recip, status, unread, dbtimestmp) values (@groupid, '1', '" + incrementno + "', '" + sender + "', '" + totalids[i] + "', '', 'X', '"+ insertdate + "')";
                            cmd2.Parameters.AddWithValue("@groupid", groupid);
                            cmd2.ExecuteNonQuery();
                            cmd2.Parameters.Clear();
                        }
                        cmd2.CommandText = "insert into schgrpmsg (groupid, msgid, recordno, subject, msgtxt,filepath) values (@groupid, '1', '" + incrementno + "', (select subject from schgrpmsg where groupid = @groupid and msgid = '1' and recordno = (select max(recordno) from schgrpmsg where groupid = @groupid and msgid = '1')), @grpmessage, '"+path+"' )";
                        cmd2.Parameters.AddWithValue("@groupid", groupid);
                        cmd2.Parameters.AddWithValue("@grpmessage",message);
                        cmd2.ExecuteNonQuery();

                        cmd2.CommandText = "update schgrpinbox set unread = '' where groupid = '" + groupid + "' and recordno = '" + incrementno + "' and recip = '" + user_id + "'";
                        cmd2.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                        con.Close();
                    }
                    ViewBag.DisplayMsg = DisplayMsg();
                    Response.Redirect("~/Message/viewchat");
                }
            }
            return null;
        }

        public void Reset_msgtime()
        {
            last_msg_dt = null;
        }

        public JsonResult RefreshMsg()
        {
            if (inprocess == false)
            {
                inprocess = true;
                msgobj = new MessageCls();
                List<MessageCls> list = msgobj.GetNotifications();
                var unread_msg = from m in list orderby m.fullmsgdate descending where m.unread == true select m;
                list = unread_msg.ToList<MessageCls>();

                if (list.Count == 0)
                {
                    list = null;
                }
                else
                {
                    if (last_msg_dt == null)
                    {
                        db = new DatabaeseClass();
                        last_msg_dt =  db.convertservertopsttimezone(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        int new_msg_count = 0;
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (Convert.ToDateTime(list[i].fullmsgdate) > last_msg_dt)
                            {//orderby m.fullmsgdate descending 
                                //list[i].fullmsgdate;

                                var new_msgs = from m in list orderby m.fullmsgdate where Convert.ToDateTime(m.fullmsgdate) > last_msg_dt select m;
                                
                                list = new_msgs.ToList<MessageCls>();
                                
                                new_msg_count++;
                                
                                //list[i].msgdate = Convert.ToDateTime(list[i].msgdate).ToString("");
                            }

                        }
                        if (new_msg_count == 0)
                        {
                            list = null;
                        }
                        else
                        {
                            db = new DatabaeseClass();
                            last_msg_dt = db.convertservertopsttimezone(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        
                    }
                }
                
                //last_msg_dt = list
                //for (int i =0; i<list.Count;i++)
                //{
                //    if (list[i].unread == true)
                //    {
                //        list.Add(new MessageCls
                //        {
                            
                //        });
                //    }
                //}
                string nof = msgobj.NumberofNotifications();
                inprocess = false;
                return Json(new { notf = list, num = nof }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
            
        }

        public ActionResult DelMsg(string sendrcdid)
        {
            int IsDel;
            string delmsgstatus = "";
            SqlTransaction trans = null;
            using (SqlConnection con = new SqlConnection(cs))
            {
                try
                {
                    string msgid = Session["MsgId"].ToString();
                    string[] getdata = msgid.Split('?');
                    msgid = getdata[0];
                    string groupid = getdata[1];
                    string recordno = getdata[2];
                    string checkquery;
                    if (groupid == "" && recordno == "")
                    {
                        checkquery = "select count(*) from inbox where msgid='" + msgid + "' and recordno='" + sendrcdid + "' and sender='" + user_id + "'";
                    }
                    else
                    {
                        checkquery = "select count(*) from schgrpinbox where groupid = '" + groupid + "' and msgid = '" + msgid + "' and recordno = '" + sendrcdid + "' and sender = '" + user_id + "'";
                    }
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    trans = con.BeginTransaction();
                    cmd.Connection = con;
                    cmd.Transaction = trans;
                    cmd.CommandText = checkquery;

                    IsDel = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                    if (IsDel == 0)
                    {
                        delmsgstatus = "failed";
                    }
                    else
                    {
                        if (groupid == "" && recordno == "")
                        {
                            cmd.CommandText = "update inbox set status='X' where msgid='" + msgid + "' and recordno='" + sendrcdid + "' and sender='" + user_id + "'";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd.CommandText = "update schgrpinbox set status = 'X' where groupid = '"+groupid+"' and msgid = '"+msgid+"' and recordno = '"+sendrcdid+"' and recip = '"+user_id+"'";
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                        delmsgstatus = "success";
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                }
                finally
                {
                    con.Close();
                }
                return Json(delmsgstatus);
            }
        }

        public ActionResult ShowDelMsgStatus(string status)
        {

            ViewBag.DisplayMsg = DisplayMsg();
            if (status == "failed")
            {
                ViewBag.call_alert = "show_alert()";
                ViewBag.message_popup = "You Only Delete Your Message";
                ViewBag.cssclass = "alert-danger";
            }
            else
            {

                ViewBag.call_alert = "show_alert()";
                ViewBag.message_popup = "Successfully Delete Message";
                ViewBag.cssclass = "alert-success";
            }

            return View("viewchat");
        }

        public ActionResult CloseDiscussion()
        {
            string status = "";
            using (SqlConnection con = new SqlConnection(cs))
            {
                string msgid = Session["MsgId"].ToString();
                string[] getdata = msgid.Split('?');
                msgid = getdata[0];
                string groupid = getdata[1];
                string recordno = getdata[2];
                string query;
                if (groupid == "" && recordno == "")
                {
                    query = "update chatview set Isview='X' where msgid='" + msgid + "' and userid='" + user_id + "' ";
                }
                else
                {
                    query = "update schgrpinbox set status = 'X' where groupid = '"+groupid+"' and msgid = '"+msgid+"' and recip = '"+user_id+"'";
                }
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                status = "success";
            }
            return Json(status);
        }
    }
}