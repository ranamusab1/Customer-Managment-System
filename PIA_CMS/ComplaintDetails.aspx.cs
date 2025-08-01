using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.Services;

namespace PIA_CMS
{
    public partial class ComplaintDetails : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminUser"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (!IsPostBack)
            {
                if (Request.QueryString["ComplaintID"] != null)
                {
                    hfComplaintID.Value = Request.QueryString["ComplaintID"];
                    LoadComplaintDetails();
                }
                else
                {
                    lblMsg.Text = "No complaint selected.";
                    lblMsg.Visible = true;
                }
            }
        }

        private void LoadComplaintDetails()
        {
            try
            {
                int complaintId;
                if (!int.TryParse(hfComplaintID.Value, out complaintId))
                {
                    lblMsg.Text = "Invalid complaint ID.";
                    lblMsg.Visible = true;
                    return;
                }

                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT TicketNo, MembershipNo, Category, Subject, Body, RequestDate, Email, Status, ReceivedFrom FROM Complaints WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ComplaintID", complaintId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblComplaintDetails.Text = $"<strong>Ticket No:</strong> {reader["TicketNo"]}<br/>" +
                                                          $"<strong>Membership No:</strong> {reader["MembershipNo"]}<br/>" +
                                                          $"<strong>Category:</strong> {reader["Category"]}<br/>" +
                                                          $"<strong>Subject:</strong> {reader["Subject"]}<br/>" +
                                                          $"<strong>Details:</strong> {reader["Body"]}<br/>" +
                                                          $"<strong>Date:</strong> {reader["RequestDate"]}<br/>" +
                                                          $"<strong>Email:</strong> {reader["Email"]}<br/>" +
                                                          $"<strong>Status:</strong> {(reader["Status"].ToString() == "O" ? "Open" : "Closed")}<br/>" +
                                                          $"<strong>Received From:</strong> {reader["ReceivedFrom"]}";
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
                lblMsg.Text = $"Error loading complaint details: {ex.Message}";
                lblMsg.Visible = true;
            }
        }

        [WebMethod]
        public static string GetComplaintDetails(int complaintId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT TicketNo, MembershipNo, Category, Subject, Body, RequestDate, Email, Status, ReceivedFrom FROM Complaints WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ComplaintID", complaintId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return $"<div class='card'>" +
                                       "<div class='card-header'>Complaint Information</div>" +
                                       "<div class='card-body'>" +
                                       $"<strong>Ticket No:</strong> {reader["TicketNo"]}<br/>" +
                                       $"<strong>Membership No:</strong> {reader["MembershipNo"]}<br/>" +
                                       $"<strong>Category:</strong> {reader["Category"]}<br/>" +
                                       $"<strong>Subject:</strong> {reader["Subject"]}<br/>" +
                                       $"<strong>Details:</strong> {reader["Body"]}<br/>" +
                                       $"<strong>Date:</strong> {reader["RequestDate"]}<br/>" +
                                       $"<strong>Email:</strong> {reader["Email"]}<br/>" +
                                       $"<strong>Status:</strong> {(reader["Status"].ToString() == "O" ? "Open" : "Closed")}<br/>" +
                                       $"<strong>Received From:</strong> {reader["ReceivedFrom"]}" +
                                       "</div></div>";
                            }
                            else
                            {
                                return "<div class='alert alert-danger'>Complaint not found.</div>";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"<div class='alert alert-danger'>Error loading complaint details: {ex.Message}</div>";
            }
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            try
            {
                int complaintId = int.Parse(hfComplaintID.Value);
                Response.Redirect($"ReplyComplaint.aspx?complaintId={complaintId}");
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error redirecting to reply: {ex.Message}";
                lblMsg.Visible = true;
            }
        }

        protected void btnForward_Click(object sender, EventArgs e)
        {
            try
            {
                int complaintId = int.Parse(hfComplaintID.Value);
                Response.Redirect($"ViewComplaints.aspx?forwardComplaintId={complaintId}");
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error redirecting to forward: {ex.Message}";
                lblMsg.Visible = true;
            }
        }
    }
}