using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MvcSchoolWebApp.Controllers;

namespace MvcSchoolWebApp.Models
{
    public class MessageCls
    {

        public int MsgId { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public bool unread { get; set; }
        public string MsgDate { get; set; }
        public string UserName { get; set; }
        public string RecordNo { get; set; }
        public string Groupid { get; set; }
        public string recordno { get; set; }
        public string senderId { get; set; }
        public string imgpath { get; set; }
        public string msgdate { get; set; }
        public string msgtime { get; set; }
        public string fullmsgdate { get; set; }
        public string filepath { get; set; }

        string cs = ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString;
        public List<MessageCls> GetNotifications()
        {
            List<MessageCls> lst = new List<MessageCls>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                string userid = HttpContext.Current.Session["User_Id"].ToString();

                string query = "select distinct it.msgid, it.subject, it.dbtimestmp,it.unread,it.sender,(ep.firstname + ' ' + ep.lastname) as empname, "+
                               "(sp.firstname + ' ' + sp.lastname) as stdname, (select count(distinct msgid) from inbox where recip = '"+userid+"' and status <> 'X')  as countRecord from inbox it "+
                               "left join emppers ep on ep.empid = it.sender "+
                               "left join stdpers sp on sp.stdid = it.sender where it.recip = '" + userid + "' and it.recordno in " +
                               "(select max(it2.recordno) from inbox it2 where it.recip = it2.recip and it.msgid = it2.msgid) order by it.dbtimestmp DESC";

                //string grpquery = "select distinct ib.recordno, ib.msgid, ib.unread, grp.groupid, grp.grouptxt, ib.groupid, msg.subject, ib.dbtimestmp," +
                //                  "(select count(distinct groupid) from schgrpinbox where recip = '"+userid+"' and status <> 'X') as countgrp from schgrpinbox ib " +
                //                  "inner join schgrpmsg msg on ib.groupid = msg.groupid and ib.msgid = msg.msgid and ib.recordno = msg.recordno "+
                //                  "inner join schgrpdtl grp on ib.groupid = grp.groupid "+
                //                  "where ib.status <> 'X' and ib.recip = '"+userid+"' and ib.recordno in "+
                //                  "(select max(ib2.recordno) from schgrpinbox ib2 where ib.groupid = ib2.groupid and ib.msgid = ib2.msgid) order by ib.recordno desc";

                string grpquery = "select top(1) ib.recordno, ib.msgid, ib.unread, grp.groupid, grp.grouptxt, ib.groupid, msg.subject, ib.dbtimestmp, " +
                                  "(select count(distinct groupid) from schgrpinbox where recip = '"+userid+"' and status <> 'X') as countgrp from schgrpinbox ib " +
                                  "inner join schgrpmsg msg on ib.groupid = msg.groupid and ib.msgid = msg.msgid and ib.recordno = msg.recordno " +
                                  "inner join schgrpdtl grp on ib.groupid = grp.groupid " +
                                  " where ib.recip = '"+userid+"' and status<> 'X' order by ib.recordno desc ";
                try
                {
                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(query, con);
                    DataSet ds = new DataSet();
                    DatabaeseClass dc = new DatabaeseClass();
                    var count = sda.Fill(ds);
                    if (count > 0)
                    {
                        int countRecord = Convert.ToInt32(ds.Tables[0].Rows[0]["countRecord"]);
                        for (int i = 0; i < countRecord; i++)
                        {
                            bool unread = false;
                            if (ds.Tables[0].Rows[i]["unread"].ToString().Trim() == "X")
                            {
                                unread = true;
                            }
                            DateTime date = dc.convertpsttousertimezone(Convert.ToDateTime(ds.Tables[0].Rows[i]["dbtimestmp"]).ToString());
                            lst.Add(new MessageCls()
                            {
                                MsgId = Convert.ToInt32(ds.Tables[0].Rows[i]["msgid"]),
                                senderId = string.IsNullOrEmpty(ds.Tables[0].Rows[i]["empname"].ToString()) ? ds.Tables[0].Rows[i]["stdname"].ToString() : ds.Tables[0].Rows[i]["empname"].ToString(),
                                unread = unread,
                                msgdate = date.ToString("yyyy-MM-dd HH:mm:ss"),
                                Subject = ds.Tables[0].Rows[i]["subject"].ToString()
                            });
                        }
                    }
                    sda = new SqlDataAdapter(grpquery, con);
                    ds = new DataSet();
                    count = sda.Fill(ds);
                    if (count > 0)
                    {
                        int countrecord = Convert.ToInt32(ds.Tables[0].Rows[0]["countgrp"]);
                        for (int i = 0; i < countrecord; i++)
                        {
                            bool unread = false;
                            if (ds.Tables[0].Rows[i]["unread"].ToString().Trim() == "X")
                            {
                                unread = true;
                            }
                            DateTime date = dc.convertpsttousertimezone(Convert.ToDateTime(ds.Tables[0].Rows[i]["dbtimestmp"]).ToString());
                            lst.Add(new MessageCls()
                            {
                                MsgId = Convert.ToInt32(ds.Tables[0].Rows[i]["msgid"]),
                                Groupid = Convert.ToString(ds.Tables[0].Rows[i]["groupid"]).Trim(),
                                senderId = Convert.ToString(ds.Tables[0].Rows[i]["grouptxt"]).Trim(),
                                recordno = Convert.ToString(ds.Tables[0].Rows[i]["recordno"]),
                                unread = unread,
                                msgdate = date.ToString("yyyy-MM-dd HH:mm:ss"),
                                Subject = ds.Tables[0].Rows[i]["subject"].ToString().Trim()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            var asc = from m in lst    orderby Convert.ToDateTime(m.msgdate) descending    select m;
            lst = asc.ToList<MessageCls>();
            int k = 0;
            foreach (var item in lst)
            {
                DateTime datetime = DateTime.ParseExact(item.msgdate, "yyyy-MM-dd HH:mm:ss", null);
                string date = datetime.ToString("yyyy-MM-dd HH:mm:ss");
                string time = datetime.ToString("h:mm tt");
                DatabaeseClass dc = new DatabaeseClass();
                string currdate = dc.convertservertousertimezone(DateTime.Now.ToString()).ToString("dd-MM-yyyy");
                lst[k].fullmsgdate = date;
                date = Convert.ToDateTime(date).ToString("dd-MM-yyyy");
                if (date == currdate)
                {
                    item.msgdate = time;
                }
                else
                {
                    item.msgdate = datetime.ToString("dd-MM-yy"); //Convert.ToDateTime(date).ToString("dd-MM-yy");
                }
                lst[k].msgdate = item.msgdate;
                
                k++;
            }
            return lst;
        }


        public string NumberofNotifications()
        {
            string UnreadMsg = null;
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    string userid = HttpContext.Current.Session["User_Id"].ToString();
                    string query = "select count(distinct ib.msgid) from inbox ib where ib.recip='" + userid + "' and ib.unread ='X' and ib.status <> 'X' and " +
                                    "ib.recordno in (select max(it2.recordno) from inbox it2 where ib.msgid = it2.msgid and ib.recordno = it2.recordno)";
                    //string grpquery = "select count(distinct msgid) from schgrpinbox where recip = '" + userid + "' and unread = 'X' and status <> 'X' group by groupid";
                    string grpquery = "select count(*) from schgrpinbox where recip = '"+userid+"' and unread = 'X' and status<> 'X' and recordno in (select max(recordno) from schgrpinbox where unread = 'X' and status<> 'X' group by groupid)";
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    UnreadMsg = cmd.ExecuteScalar().ToString();
                    cmd = new SqlCommand(grpquery, con);
                    UnreadMsg = (Convert.ToInt16(UnreadMsg) + Convert.ToInt16(cmd.ExecuteScalar())).ToString();
                    if (UnreadMsg == "0")
                    {
                        UnreadMsg = null;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return UnreadMsg;
        }



        //Insert Message when upload a assignment/ timetable

        public void InsertMgs_UploadFile(string title, string msg, string[] sendTo)
        {


            HttpContext.Current.Session["User_Role"].ToString();
            string cs = ConfigurationManager.ConnectionStrings["Falconlocal"].ConnectionString.ToString();
            using (SqlConnection conn = new SqlConnection(cs))
            {

                for (int i = 0; i < sendTo.Length; i++)
                {
                    string query = "insert into inbox(msgid,recordno,subject,message,sender,recip,unread,status,dbtimestmp) values((select ISNULL(MAX(msgid),0)+1 from inbox),'1','" + title + "','" + msg + "','" + HttpContext.Current.Session["User_Role"] + "','" + sendTo[i] + "','X',' ','" + DateTime.Now.ToString() + "')";
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }

        }
    }
}
