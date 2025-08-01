using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.Services;

namespace PIA_CMS
{
    public partial class EmailDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminUser"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (!IsPostBack)
            {
                if (Request.QueryString["EmailID"] != null)
                {
                    hfEmailID.Value = Request.QueryString["EmailID"];
                    LoadEmailDetails();
                }
                else
                {
                    lblMsg.Text = "No email selected.";
                    lblMsg.Visible = true;
                }
            }
        }

        private void LoadEmailDetails()
        {
            try
            {
                int emailId;
                if (!int.TryParse(hfEmailID.Value, out emailId))
                {
                    lblMsg.Text = "Invalid email ID.";
                    lblMsg.Visible = true;
                    return;
                }

                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT EmailFrom, EmailTo, EmailSubject, EmailBody, SentDate, MembershipNo, UserID FROM EmailsSent WHERE EmailID = @EmailID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmailID", emailId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblEmailDetails.Text = $"<strong>From:</strong> {reader["EmailFrom"]}<br/>" +
                                                       $"<strong>To:</strong> {reader["EmailTo"]}<br/>" +
                                                       $"<strong>Subject:</strong> {reader["EmailSubject"]}<br/>" +
                                                       $"<strong>Body:</strong> {reader["EmailBody"]}<br/>" +
                                                       $"<strong>Sent:</strong> {reader["SentDate"]}<br/>" +
                                                       $"<strong>Membership No:</strong> {reader["MembershipNo"]}<br/>" +
                                                       $"<strong>User ID:</strong> {reader["UserID"]}";
                            }
                            else
                            {
                                lblMsg.Text = "Email not found.";
                                lblMsg.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error loading email details: {ex.Message}";
                lblMsg.Visible = true;
            }
        }

        [WebMethod]
        public static string GetEmailDetails(int emailId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT EmailFrom, EmailTo, EmailSubject, EmailBody, SentDate, MembershipNo, UserID FROM EmailsSent WHERE EmailID = @EmailID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmailID", emailId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return $"<div class='card'>" +
                                       "<div class='card-header'>Email Information</div>" +
                                       "<div class='card-body'>" +
                                       $"<strong>From:</strong> {reader["EmailFrom"]}<br/>" +
                                       $"<strong>To:</strong> {reader["EmailTo"]}<br/>" +
                                       $"<strong>Subject:</strong> {reader["EmailSubject"]}<br/>" +
                                       $"<strong>Body:</strong> {reader["EmailBody"]}<br/>" +
                                       $"<strong>Sent:</strong> {reader["SentDate"]}<br/>" +
                                       $"<strong>Membership No:</strong> {reader["MembershipNo"]}<br/>" +
                                       $"<strong>User ID:</strong> {reader["UserID"]}" +
                                       "</div></div>";
                            }
                            else
                            {
                                return "<div class='alert alert-danger'>Email not found.</div>";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"<div class='alert alert-danger'>Error loading email details: {ex.Message}</div>";
            }
        }
    }
}