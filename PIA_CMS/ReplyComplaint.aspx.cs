using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace PIA_CMS
{
    public partial class ReplyComplaint : System.Web.UI.Page
    {
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
                    string query = "SELECT Email, Subject, Body, MembershipNo FROM Complaints WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ComplaintID", complaintId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtEmailTo.Text = reader["Email"].ToString();
                                txtSubject.Text = $"Re: {reader["Subject"]}";
                                txtBody.Text = $"\n\n--- Original Complaint ---\nSubject: {reader["Subject"]}\nDetails: {reader["Body"]}\nMembership No: {reader["MembershipNo"]}";
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

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO EmailsSent (EmailFrom, EmailTo, EmailSubject, EmailBody, SentDate, UserID, MembershipNo) VALUES (@EmailFrom, @EmailTo, @EmailSubject, @EmailBody, @SentDate, @UserID, @MembershipNo)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmailFrom", Session["AdminUser"].ToString() + "@pia.com");
                        cmd.Parameters.AddWithValue("@EmailTo", txtEmailTo.Text);
                        cmd.Parameters.AddWithValue("@EmailSubject", txtSubject.Text);
                        cmd.Parameters.AddWithValue("@EmailBody", txtBody.Text);
                        cmd.Parameters.AddWithValue("@SentDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserID", Session["AdminUser"].ToString());
                        cmd.Parameters.AddWithValue("@MembershipNo", "");
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                lblMsg.Text = "Reply sent successfully.";
                lblMsg.CssClass = "alert alert-success";
                lblMsg.Visible = true;
                txtEmailTo.Text = "";
                txtSubject.Text = "";
                txtBody.Text = "";
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error sending reply: {ex.Message}";
                lblMsg.Visible = true;
            }
        }
    }
}