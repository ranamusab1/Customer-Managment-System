using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace PIA_CMS
{
    public partial class ReplyComplaint : Page
    {
        private static readonly string DataPath = @"C:\PIA";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminUser"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (!IsPostBack)
            {
                string complaintId = Request.QueryString["complaintId"];
                if (!string.IsNullOrEmpty(complaintId))
                {
                    LoadComplaintData(int.Parse(complaintId));
                }
                else
                {
                    lblMsg.Text = "No complaint selected.";
                    lblMsg.Visible = true;
                }
            }
        }

        private void LoadComplaintData(int complaintId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT email, Subject, cstatus, ffnum FROM cms WHERE tkt_no = @Tkt_no";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Tkt_no", complaintId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtReplyEmail.Text = reader["email"].ToString();
                                txtReplySubject.Text = $"Re: {reader["Subject"]}";
                                txtReplyBody.Text = $"\n\n--- Original Complaint ---\nSubject: {reader["Subject"]}\nDetails: {(reader["cstatus"].ToString() == "O" ? "Open Complaint" : "Closed Complaint")}\nMembership No: {reader["ffnum"]}";
                            }
                            else
                            {
                                lblMsg.Text = "Complaint not found.";
                                lblMsg.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error loading complaint data: {ex.Message}";
                lblMsg.Visible = true;
            }
        }

        protected void btnReplySend_Click(object sender, EventArgs e)
        {
            try
            {
                string complaintId = Request.QueryString["complaintId"];
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                string ffnum = "";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT ffnum FROM cms WHERE tkt_no = @Tkt_no";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Tkt_no", complaintId);
                        con.Open();
                        ffnum = (string)cmd.ExecuteScalar();
                        con.Close();
                    }

                    query = "INSERT INTO sendmaillist (userid, senddate, emlsub, sendto, ffnum) VALUES (@Userid, @Senddate, @Emlsub, @Sendto, @Ffnum); " +
                            "UPDATE cms SET cstatus = 'C', UpdateDate = @UpdateDate, UpdateBy = @UpdateBy WHERE tkt_no = @Tkt_no";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Userid", Session["AdminUser"].ToString());
                        cmd.Parameters.AddWithValue("@Senddate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Emlsub", txtReplySubject.Text);
                        cmd.Parameters.AddWithValue("@Sendto", txtReplyEmail.Text);
                        cmd.Parameters.AddWithValue("@Ffnum", ffnum);
                        cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UpdateBy", Session["AdminUser"].ToString());
                        cmd.Parameters.AddWithValue("@Tkt_no", complaintId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                string filePath = Path.Combine(DataPath, $"{complaintId}.txt");
                string replyContent = $"\n\n--- Reply by {Session["AdminUser"]} on {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---\nSubject: {txtReplySubject.Text}\nBody: {txtReplyBody.Text}";
                File.AppendAllText(filePath, replyContent);

                lblMsg.Text = "Reply sent successfully.";
                lblMsg.CssClass = "alert alert-success";
                lblMsg.Visible = true;
                txtReplyEmail.Text = "";
                txtReplySubject.Text = "";
                txtReplyBody.Text = "";
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error sending reply: {ex.Message}";
                lblMsg.Visible = true;
            }
        }
    }
}