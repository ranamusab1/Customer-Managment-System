
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.IO;
using System.Text;
using System.Web;
using System.Linq;

namespace PIA_CMS
{
    public partial class ViewComplaints : Page
    {
        private static readonly string DataPath = @"C:\PIA\";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminUser"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (!IsPostBack)
            {
                BindComplaints();
                BindForwardAdmins();
            }
        }

        private void BindComplaints()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
            string category = Request.QueryString["cat"] ?? "All";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ComplaintID, TicketNo, Category, Subject, RequestDate, ForwardedTo FROM Complaints";
                if (category != "All" || !string.IsNullOrEmpty(ddlFilterBy.SelectedValue))
                {
                    query += " WHERE ";
                    if (category != "All")
                        query += "Category = @Category";
                    if (!string.IsNullOrEmpty(ddlFilterBy.SelectedValue))
                        query += (category != "All" ? " AND " : "") + $"{ddlFilterBy.SelectedValue} LIKE @FilterValue";
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (category != "All")
                        cmd.Parameters.AddWithValue("@Category", category);
                    if (!string.IsNullOrEmpty(ddlFilterBy.SelectedValue))
                        cmd.Parameters.AddWithValue("@FilterValue", "%" + txtFilter.Text + "%");

                    con.Open();
                    gvComplaints.DataSource = cmd.ExecuteReader();
                    gvComplaints.DataBind();
                }
            }
        }

        private void BindForwardAdmins()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT AdminID, Name FROM Admins";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ddlForwardTo.DataSource = reader;
                        ddlForwardTo.DataTextField = "Name";
                        ddlForwardTo.DataValueField = "AdminID";
                        ddlForwardTo.DataBind();
                        ddlForwardTo.Items.Insert(0, new ListItem("Select Admin", ""));
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindComplaints();
        }

        protected void gvComplaints_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                try
                {
                    int complaintId = Convert.ToInt32(e.CommandArgument);
                    ScriptManager.RegisterStartupScript(this, GetType(), "showComplaintDetailsModal", $"showComplaintDetailsModal({complaintId});", true);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error opening complaint details: {ex.Message}');", true);
                }
            }
        }

        protected void btnReplySend_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                int complaintId = int.Parse(Request.Form["pnlComplaintDetails$ctl00"] ?? "0");
                string ticketNo = "";
                string membershipNo = "";

                // Get TicketNo and MembershipNo
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT TicketNo, MembershipNo FROM Complaints WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ComplaintID", complaintId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ticketNo = reader["TicketNo"].ToString();
                                membershipNo = reader["MembershipNo"].ToString();
                            }
                        }
                    }
                }

                // Save to EmailsSent
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO EmailsSent (EmailFrom, EmailTo, EmailSubject, EmailBody, SentDate, UserID, MembershipNo) VALUES (@EmailFrom, @EmailTo, @EmailSubject, @EmailBody, @SentDate, @UserID, @MembershipNo)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmailFrom", Session["AdminUser"].ToString() + "@pia.com");
                        cmd.Parameters.AddWithValue("@EmailTo", txtReplyEmail.Text);
                        cmd.Parameters.AddWithValue("@EmailSubject", txtReplySubject.Text);
                        cmd.Parameters.AddWithValue("@EmailBody", txtReplyBody.Text);
                        cmd.Parameters.AddWithValue("@SentDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserID", Session["AdminUser"].ToString());
                        cmd.Parameters.AddWithValue("@MembershipNo", membershipNo);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Append to .txt file
                string filePath = Path.Combine(DataPath, $"{ticketNo}.txt");
                string replyContent = $"\n\n--- Reply by {Session["AdminUser"]} on {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---\nSubject: {txtReplySubject.Text}\nBody: {txtReplyBody.Text}";
                File.AppendAllText(filePath, replyContent);

                ScriptManager.RegisterStartupScript(this, GetType(), "successAlert", "alert('Reply sent successfully.'); $('#pnlComplaintDetails').modal('hide');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error sending reply: {ex.Message}');", true);
            }
        }

        protected void btnForwardSend_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                int complaintId = int.Parse(Request.Form["pnlComplaintDetails$ctl00"] ?? "0");
                string ticketNo = "";
                string membershipNo = "";
                string subject = "";
                string body = "";

                // Get TicketNo, MembershipNo, Subject, Body
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT TicketNo, MembershipNo, Subject, Body FROM Complaints WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ComplaintID", complaintId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ticketNo = reader["TicketNo"].ToString();
                                membershipNo = reader["MembershipNo"].ToString();
                                subject = reader["Subject"].ToString();
                                body = reader["Body"].ToString();
                            }
                        }
                    }
                }

                // Update Complaints table
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string updateQuery = "UPDATE Complaints SET ForwardedTo = @ForwardedTo, ForwardedDate = @ForwardedDate, ForwardRemarks = @ForwardRemarks, ForwardedBy = @ForwardedBy WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@ForwardedTo", ddlForwardTo.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@ForwardedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ForwardRemarks", txtForwardRemarks.Text);
                        cmd.Parameters.AddWithValue("@ForwardedBy", Session["AdminUser"].ToString());
                        cmd.Parameters.AddWithValue("@ComplaintID", complaintId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Save to EmailsSent
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string insertQuery = "INSERT INTO EmailsSent (EmailFrom, EmailTo, EmailSubject, EmailBody, SentDate, UserID, MembershipNo) VALUES (@EmailFrom, @EmailTo, @EmailSubject, @EmailBody, @SentDate, @UserID, @MembershipNo)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmailFrom", Session["AdminUser"].ToString() + "@pia.com");
                        cmd.Parameters.AddWithValue("@EmailTo", txtForwardEmail.Text);
                        cmd.Parameters.AddWithValue("@EmailSubject", $"Fwd: {subject}");
                        cmd.Parameters.AddWithValue("@EmailBody", txtForwardRemarks.Text + $"\n\n--- Forwarded Complaint ---\nSubject: {subject}\nDetails: {body}\nMembership No: {membershipNo}");
                        cmd.Parameters.AddWithValue("@SentDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserID", ddlForwardTo.SelectedValue);
                        cmd.Parameters.AddWithValue("@MembershipNo", membershipNo);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Append to .txt file
                string filePath = Path.Combine(DataPath, $"{ticketNo}.txt");
                string forwardContent = $"\n\n--- Forwarded by {Session["AdminUser"]} to {ddlForwardTo.SelectedItem.Text} on {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---\nRemarks: {txtForwardRemarks.Text}\nSubject: Fwd: {subject}\nBody: {body}";
                File.AppendAllText(filePath, forwardContent);

                ScriptManager.RegisterStartupScript(this, GetType(), "successAlert", "alert('Complaint forwarded successfully.'); $('#pnlComplaintDetails').modal('hide');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error forwarding complaint: {ex.Message}');", true);
            }
        }

        [WebMethod]
        public static object GetComplaintDetails(int complaintId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                string ticketNo = "";
                string email = "";
                string subject = "";
                string body = "";
                string membershipNo = "";

                // Fetch complaint details
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
                                ticketNo = reader["TicketNo"].ToString();
                                membershipNo = reader["MembershipNo"].ToString();
                                email = reader["Email"].ToString();
                                subject = reader["Subject"].ToString();
                                body = reader["Body"].ToString();
                                return new
                                {
                                    details = $"<div class='card'><div class='card-header'>Complaint Information</div><div class='card-body'>" +
                                              $"<strong>Ticket No:</strong> {reader["TicketNo"]}<br/>" +
                                              $"<strong>Membership No:</strong> {reader["MembershipNo"]}<br/>" +
                                              $"<strong>Category:</strong> {reader["Category"]}<br/>" +
                                              $"<strong>Subject:</strong> {reader["Subject"]}<br/>" +
                                              $"<strong>Details:</strong> {reader["Body"]}<br/>" +
                                              $"<strong>Date:</strong> {reader["RequestDate"]}<br/>" +
                                              $"<strong>Email:</strong> {reader["Email"]}<br/>" +
                                              $"<strong>Status:</strong> {(reader["Status"].ToString() == "O" ? "Open" : "Closed")}<br/>" +
                                              $"<strong>Received From:</strong> {reader["ReceivedFrom"]}</div></div>",
                                    conversation = GetConversation(ticketNo),
                                    attachments = GetAttachments(ticketNo),
                                    email = email,
                                    replySubject = $"Re: {subject}",
                                    replyBody = $"\n\n--- Original Complaint ---\nSubject: {subject}\nDetails: {body}\nMembership No: {membershipNo}"
                                };
                            }
                            else
                            {
                                return new { details = "<div class='alert alert-danger'>Complaint not found.</div>", conversation = "", attachments = "", email = "", replySubject = "", replyBody = "" };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetComplaintDetails: {ex.Message}");
                return new { details = $"<div class='alert alert-danger'>Error: {ex.Message}</div>", conversation = "", attachments = "", email = "", replySubject = "", replyBody = "" };
            }
        }

        [WebMethod]
        public static string GetAdminEmail(string adminId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT Email FROM Admins WHERE AdminID = @AdminID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AdminID", adminId);
                        con.Open();
                        object email = cmd.ExecuteScalar();
                        return email?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAdminEmail: {ex.Message}");
                return "";
            }
        }

        private static string GetConversation(string ticketNo)
        {
            try
            {
                string filePath = Path.Combine(DataPath, $"{ticketNo}.txt");
                System.Diagnostics.Debug.WriteLine($"Reading conversation file: {filePath}");
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                return "No conversation found.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetConversation: {ex.Message}");
                return $"Error loading conversation: {ex.Message}";
            }
        }

        private static string GetAttachments(string ticketNo)
        {
            try
            {
                string folderPath = Path.Combine(DataPath, ticketNo);
                System.Diagnostics.Debug.WriteLine($"Checking folder: {folderPath}");
                if (Directory.Exists(folderPath))
                {
                    string[] files = Directory.GetFiles(folderPath);
                    System.Diagnostics.Debug.WriteLine($"Found {files.Length} files in {folderPath}");
                    StringBuilder html = new StringBuilder();
                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        string relativePath = $"/ComplaintData/{ticketNo}/{fileName}";
                        string ext = Path.GetExtension(fileName).ToLower();
                        System.Diagnostics.Debug.WriteLine($"Processing file: {fileName}, Ext: {ext}, Path: {relativePath}");
                        if (new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(ext))
                        {
                            html.Append($"<img src='{relativePath}' class='attachment-img' onclick='showFullSizeAttachment(\"{relativePath}\")' alt='{fileName}' />");
                        }
                        else if (ext == ".pdf")
                        {
                            html.Append($"<div class='attachment-pdf' onclick='showFullSizeAttachment(\"{relativePath}\")'>{fileName}</div>");
                        }
                        else
                        {
                            html.Append($"<a href='{relativePath}' target='_blank' class='btn btn-sm btn-secondary m-1'>{fileName}</a>");
                        }
                    }
                    return html.ToString();
                }
                System.Diagnostics.Debug.WriteLine($"Folder not found: {folderPath}");
                return "No attachments found.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAttachments: {ex.Message}");
                return $"Error loading attachments: {ex.Message}";
            }
        }
    }
}