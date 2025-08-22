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
        private static readonly string DataPath = @"E:\PIA";

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
                System.Diagnostics.Debug.WriteLine("Page_Load: BindForwardAdmins called, dropdown items = " + ddlForwardTo.Items.Count);
            }
        }

        private void BindComplaints()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                string category = Request.QueryString["cat"] ?? "All";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT tkt_no, Category, Subject, Req_Date, fwd_to FROM cms";
                    if (category != "All" || !string.IsNullOrEmpty(ddlFilterBy.SelectedValue))
                    {
                        query += " WHERE ";
                        if (category != "All")
                            query += "Category = @Category";
                        if (!string.IsNullOrEmpty(ddlFilterBy.SelectedValue))
                            query += (category != "All" ? " AND " : "") + $"{ddlFilterBy.SelectedValue} LIKE @FilterValue";
                    }
                    query += " ORDER BY Req_Date DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (category != "All")
                            cmd.Parameters.AddWithValue("@Category", category);
                        if (!string.IsNullOrEmpty(ddlFilterBy.SelectedValue))
                            cmd.Parameters.AddWithValue("@FilterValue", "%" + txtFilter.Text + "%");

                        con.Open();
                        gvComplaints.DataSource = cmd.ExecuteReader();
                        gvComplaints.DataBind();
                        System.Diagnostics.Debug.WriteLine("BindComplaints: Rows bound to grid.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindComplaints Error: " + ex.Message);
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error loading complaints: {ex.Message}');", true);
            }
        }

        private void BindForwardAdmins()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT sno, NameOfEmployee FROM admin_login WHERE enable = 1 AND NameOfEmployee IS NOT NULL AND TRIM(NameOfEmployee) != '' ORDER BY NameOfEmployee";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            System.Diagnostics.Debug.WriteLine("BindForwardAdmins: Query executed, has rows = " + (reader.HasRows ? "Yes" : "No"));
                            ddlForwardTo.Items.Clear();
                            int count = 0;
                            while (reader.Read())
                            {
                                count++;
                                string sno = reader["sno"].ToString();
                                string name = reader["NameOfEmployee"].ToString().Trim();
                                ddlForwardTo.Items.Add(new ListItem(name, sno));
                                System.Diagnostics.Debug.WriteLine($"BindForwardAdmins: Added admin sno={sno}, name={name}");
                            }
                            if (count == 0)
                            {
                                ddlForwardTo.Items.Add(new ListItem("No Admins Available", ""));
                                System.Diagnostics.Debug.WriteLine("BindForwardAdmins: No admins found.");
                            }
                            else
                            {
                                ddlForwardTo.Items.Insert(0, new ListItem("Select Admin", ""));
                            }
                            System.Diagnostics.Debug.WriteLine($"BindForwardAdmins: Total dropdown items = {ddlForwardTo.Items.Count}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindForwardAdmins Error: " + ex.Message);
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error loading admins: {ex.Message}');", true);
                ddlForwardTo.Items.Clear();
                ddlForwardTo.Items.Add(new ListItem("Error Loading Admins", ""));
            }
        }

        [WebMethod]
        public static string BindForwardAdminsAjax()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                StringBuilder options = new StringBuilder();
                options.Append("<option value=''>Select Admin</option>");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT sno, NameOfEmployee FROM admin_login WHERE enable = 1 AND NameOfEmployee IS NOT NULL AND TRIM(NameOfEmployee) != '' ORDER BY NameOfEmployee";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            System.Diagnostics.Debug.WriteLine("BindForwardAdminsAjax: Query executed, has rows = " + (reader.HasRows ? "Yes" : "No"));
                            int count = 0;
                            while (reader.Read())
                            {
                                count++;
                                string sno = reader["sno"].ToString();
                                string name = HttpUtility.HtmlEncode(reader["NameOfEmployee"].ToString().Trim());
                                options.Append($"<option value='{sno}'>{name}</option>");
                                System.Diagnostics.Debug.WriteLine($"BindForwardAdminsAjax: Added admin sno={sno}, name={name}");
                            }
                            if (count == 0)
                            {
                                System.Diagnostics.Debug.WriteLine("BindForwardAdminsAjax: No admins found.");
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine("BindForwardAdminsAjax: Options = " + options.ToString());
                return options.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindForwardAdminsAjax Error: " + ex.Message);
                return "<option value=''>Error loading admins: " + HttpUtility.HtmlEncode(ex.Message) + "</option>";
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
                    int rowIndex = Convert.ToInt32(e.CommandArgument);
                    decimal tkt_no = Convert.ToDecimal(gvComplaints.DataKeys[rowIndex].Value);
                    System.Diagnostics.Debug.WriteLine("gvComplaints_RowCommand: tkt_no = " + tkt_no);
                    ScriptManager.RegisterStartupScript(this, GetType(), "showComplaintDetailsModal", $"showComplaintDetailsModal({tkt_no});", true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("gvComplaints_RowCommand Error: " + ex.Message);
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error opening complaint details: {ex.Message}');", true);
                }
            }
        }

        protected void btnReplySend_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                decimal tkt_no = decimal.Parse(hfComplaintId.Value ?? "0");
                System.Diagnostics.Debug.WriteLine($"btnReplySend_Click: tkt_no={tkt_no}, txtReplyEmail={txtReplyEmail.Text}");

                if (tkt_no == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", "alert('Invalid Ticket No. Please select a valid complaint.');", true);
                    return;
                }

                string ticketNo = "";
                string ffnum = "";
                string email = "";

                // Fetch complaint details to validate email
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT tkt_no, ffnum, email FROM cms WHERE tkt_no = @tkt_no";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@tkt_no", tkt_no);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ticketNo = reader["tkt_no"].ToString();
                                ffnum = reader["ffnum"] != DBNull.Value ? reader["ffnum"].ToString() : "N/A";
                                email = reader["email"] != DBNull.Value ? reader["email"].ToString() : "";
                                System.Diagnostics.Debug.WriteLine($"btnReplySend_Click: Fetched email={email}");
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", "alert('Complaint not found.');", true);
                                return;
                            }
                        }
                    }
                }

                // Validate email
                if (string.IsNullOrEmpty(email))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", "alert('No valid email found for this complaint in the database. Please ensure the complaint has an email address.');", true);
                    return;
                }

                if (string.IsNullOrEmpty(txtReplySubject.Text) || string.IsNullOrEmpty(txtReplyBody.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", "alert('Subject and body cannot be empty.');", true);
                    return;
                }

                // Update sendmaillist
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO sendmaillist (userid, senddate, emlsub, sendto, ffnum) VALUES (@userid, @senddate, @emlsub, @sendto, @ffnum);" +
                                  "UPDATE cms SET cstatus = 'C', UpdateDate = @UpdateDate, UpdateBy = @UpdateBy WHERE tkt_no = @tkt_no";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@userid", Session["AdminUser"]?.ToString() ?? "Unknown");
                        cmd.Parameters.AddWithValue("@senddate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@emlsub", txtReplySubject.Text);
                        cmd.Parameters.AddWithValue("@sendto", email);
                        cmd.Parameters.AddWithValue("@ffnum", ffnum);
                        cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UpdateBy", Session["AdminUser"]?.ToString() ?? "Unknown");
                        cmd.Parameters.AddWithValue("@tkt_no", tkt_no);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine("btnReplySend_Click: sendmaillist and cms updated.");
                    }
                }

                // Append to file
                string filePath = Path.Combine(DataPath, $"{ticketNo}.txt");
                string replyContent = $"\n\n--- Reply by {Session["AdminUser"]} on {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---\nSubject: {txtReplySubject.Text}\nBody: {txtReplyBody.Text}";
                File.AppendAllText(filePath, replyContent);
                System.Diagnostics.Debug.WriteLine($"btnReplySend_Click: Appended to {filePath}");

                ScriptManager.RegisterStartupScript(this, GetType(), "successAlert", "alert('Reply recorded successfully.'); $('#pnlComplaintDetails').modal('hide');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnReplySend_Click Error: " + ex.Message);
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error recording reply: {ex.Message}');", true);
            }
        }

        protected void btnForwardSend_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                decimal tkt_no = decimal.Parse(hfComplaintId.Value ?? "0");
                System.Diagnostics.Debug.WriteLine($"btnForwardSend_Click: tkt_no={tkt_no}, Selected Admin={ddlForwardTo.SelectedValue}, Remarks={txtForwardRemarks.Text}");

                if (tkt_no == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", "alert('Invalid Ticket No. Please select a valid complaint.');", true);
                    return;
                }
                if (string.IsNullOrEmpty(ddlForwardTo.SelectedValue) || ddlForwardTo.SelectedValue == "")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", "alert('Please select an admin to forward to from the dropdown.');", true);
                    return;
                }
                if (string.IsNullOrEmpty(txtForwardRemarks.Text))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", "alert('Please enter forward remarks.');", true);
                    return;
                }

                string ticketNo = "";
                string ffnum = "";
                string subject = "";
                string forwardToAdminId = ddlForwardTo.SelectedValue;
                string forwardToAdminName = ddlForwardTo.SelectedItem.Text;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT tkt_no, ffnum, Subject FROM cms WHERE tkt_no = @tkt_no";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@tkt_no", tkt_no);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ticketNo = reader["tkt_no"].ToString();
                                ffnum = reader["ffnum"] != DBNull.Value ? reader["ffnum"].ToString() : "N/A";
                                subject = reader["Subject"] != DBNull.Value ? reader["Subject"].ToString() : "N/A";
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", "alert('Complaint not found.');", true);
                                return;
                            }
                        }
                    }
                }

                // Update cms with forward details
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string updateQuery = "UPDATE cms SET fwd_to = @fwd_to, fwd_date = @fwd_date, fwd_by = @fwd_by, fwd_remarks = @fwd_remarks WHERE tkt_no = @tkt_no";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@fwd_to", forwardToAdminName);
                        cmd.Parameters.AddWithValue("@fwd_date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@fwd_by", Session["AdminUser"]?.ToString() ?? "Unknown");
                        cmd.Parameters.AddWithValue("@fwd_remarks", txtForwardRemarks.Text);
                        cmd.Parameters.AddWithValue("@tkt_no", tkt_no);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine("btnForwardSend_Click: cms updated.");
                    }
                }

                // Insert into cms_fwd_remarks
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string insertQuery = "INSERT INTO cms_fwd_remarks (fwd_to, fwd_by, fwd_date, remarks, tkt_no) VALUES (@fwd_to, @fwd_by, @fwd_date, @remarks, @tkt_no)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@fwd_to", forwardToAdminName);
                        cmd.Parameters.AddWithValue("@fwd_by", Session["AdminUser"]?.ToString() ?? "Unknown");
                        cmd.Parameters.AddWithValue("@fwd_date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@remarks", txtForwardRemarks.Text);
                        cmd.Parameters.AddWithValue("@tkt_no", (int)tkt_no);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine("btnForwardSend_Click: cms_fwd_remarks inserted.");
                    }
                }

                // Log in sendmaillist (no email sending)
                string forwardToEmail = GetAdminEmail(forwardToAdminId);
                System.Diagnostics.Debug.WriteLine($"btnForwardSend_Click: forwardToAdminId={forwardToAdminId}, forwardToAdminName={forwardToAdminName}, forwardToEmail={forwardToEmail}");

                if (string.IsNullOrEmpty(forwardToEmail))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", "alert('Selected admin does not have a valid email in the database. Please select another admin or update admin email.');", true);
                    return;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string insertQuery = "INSERT INTO sendmaillist (userid, senddate, emlsub, sendto, ffnum) VALUES (@userid, @senddate, @emlsub, @sendto, @ffnum)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@userid", Session["AdminUser"]?.ToString() ?? "Unknown");
                        cmd.Parameters.AddWithValue("@senddate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@emlsub", $"Fwd: {subject}");
                        cmd.Parameters.AddWithValue("@sendto", forwardToEmail);
                        cmd.Parameters.AddWithValue("@ffnum", ffnum);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine("btnForwardSend_Click: sendmaillist inserted.");
                    }
                }

                // Append to file
                string filePath = Path.Combine(DataPath, $"{ticketNo}.txt");
                string forwardContent = $"\n\n--- Forwarded by {Session["AdminUser"]} to {forwardToAdminName} on {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---\nSubject: Fwd: {subject}\nRemarks: {txtForwardRemarks.Text}";
                File.AppendAllText(filePath, forwardContent);
                System.Diagnostics.Debug.WriteLine($"btnForwardSend_Click: Appended to {filePath}");

                ScriptManager.RegisterStartupScript(this, GetType(), "successAlert", "alert('Complaint forwarded successfully to " + forwardToAdminName + ".'); $('#pnlComplaintDetails').modal('hide');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"btnForwardSend_Click Error: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error forwarding complaint: {ex.Message}');", true);
            }
        }

        [WebMethod]
        public static object GetComplaintDetails(decimal tkt_no)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                string ticketNo = "";
                string email = "";
                string subject = "";
                string ffnum = "";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT tkt_no, ffnum, Category, Subject, cstatus, Req_Date, email, Req_By FROM cms WHERE tkt_no = @tkt_no";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@tkt_no", tkt_no);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ticketNo = reader["tkt_no"].ToString();
                                ffnum = reader["ffnum"] != DBNull.Value ? reader["ffnum"].ToString() : "N/A";
                                email = reader["email"] != DBNull.Value ? reader["email"].ToString() : "";
                                subject = reader["Subject"] != DBNull.Value ? reader["Subject"].ToString() : "N/A";
                                System.Diagnostics.Debug.WriteLine($"GetComplaintDetails: tkt_no={tkt_no}, email={email}");
                                return new
                                {
                                    details = $"<div class='card'><div class='card-header'>Complaint Information</div><div class='card-body'>" +
                                              $"<strong>Ticket No:</strong> {reader["tkt_no"]}<br/>" +
                                              $"<strong>Membership No:</strong> {HttpUtility.HtmlEncode(ffnum)}<br/>" +
                                              $"<strong>Category:</strong> {HttpUtility.HtmlEncode(reader["Category"].ToString())}<br/>" +
                                              $"<strong>Subject:</strong> {HttpUtility.HtmlEncode(subject)}<br/>" +
                                              $"<strong>Status:</strong> {(reader["cstatus"].ToString() == "O" ? "Open" : "Closed")}<br/>" +
                                              $"<strong>Date:</strong> {(reader["Req_Date"] != DBNull.Value ? Convert.ToDateTime(reader["Req_Date"]).ToString("yyyy-MM-dd") : "N/A")}<br/>" +
                                              $"<strong>Email:</strong> {HttpUtility.HtmlEncode(email)}<br/>" +
                                              $"<strong>Received From:</strong> {HttpUtility.HtmlEncode(reader["Req_By"].ToString())}</div></div>",
                                    conversation = GetConversation(ticketNo),
                                    attachments = GetAttachments(ticketNo),
                                    forwardHistory = GetForwardHistory((int)tkt_no),
                                    email = email,
                                    replySubject = $"Re: {subject}",
                                    replyBody = $"\n\n--- Original Complaint ---\nSubject: {subject}\nMembership No: {ffnum}"
                                };
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"GetComplaintDetails: Complaint not found for tkt_no={tkt_no}");
                                return new
                                {
                                    details = "<div class='alert alert-danger'>Complaint not found.</div>",
                                    conversation = "",
                                    attachments = "",
                                    forwardHistory = "",
                                    email = "",
                                    replySubject = "",
                                    replyBody = ""
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetComplaintDetails Error: " + ex.Message);
                return new
                {
                    details = $"<div class='alert alert-danger'>Error: {ex.Message}</div>",
                    conversation = "",
                    attachments = "",
                    forwardHistory = "",
                    email = "",
                    replySubject = "",
                    replyBody = ""
                };
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
                    string query = "SELECT EMAIL FROM admin_login WHERE sno = @sno AND enable = 1 AND EMAIL IS NOT NULL AND TRIM(EMAIL) != ''";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@sno", adminId);
                        con.Open();
                        object email = cmd.ExecuteScalar();
                        string result = email != DBNull.Value && email != null ? email.ToString().Trim() : "";
                        System.Diagnostics.Debug.WriteLine($"GetAdminEmail: sno={adminId}, Email={result ?? "NULL"}");
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetAdminEmail Error: " + ex.Message);
                return "";
            }
        }

        private static string GetConversation(string ticketNo)
        {
            try
            {
                string filePath = Path.Combine(DataPath, $"{ticketNo}.txt");
                if (!File.Exists(filePath))
                {
                    return "<div class='alert alert-info'>No conversation found.</div>";
                }

                string content = File.ReadAllText(filePath);
                StringBuilder html = new StringBuilder();
                string[] entries = content.Split(new[] { "\n\n---" }, StringSplitOptions.None);

                foreach (string entry in entries)
                {
                    if (string.IsNullOrWhiteSpace(entry)) continue;

                    string cleanedEntry = entry.TrimStart('-').Trim();
                    string[] lines = cleanedEntry.Split('\n');
                    string header = lines.Length > 0 ? lines[0].Trim() : "Unknown";
                    string remainingContent = string.Join("\n", lines, 1, lines.Length - 1).Trim();

                    if (remainingContent.Contains("<pre") || remainingContent.Contains("<br") || remainingContent.Contains("<b") || remainingContent.Contains("<font"))
                    {
                        html.Append("<div class='conversation-entry'>");
                        html.Append($"<div class='conversation-header'>{HttpUtility.HtmlEncode(header)}</div>");
                        html.Append($"<div class='conversation-body'>{remainingContent}</div>");
                        html.Append("</div>");
                    }
                    else
                    {
                        string subject = "";
                        string body = "";
                        StringBuilder bodyBuilder = new StringBuilder();

                        for (int i = 1; i < lines.Length; i++)
                        {
                            if (lines[i].StartsWith("Subject:"))
                            {
                                subject = lines[i].Substring("Subject:".Length).Trim();
                            }
                            else if (lines[i].StartsWith("Body:") || lines[i].StartsWith("Remarks:"))
                            {
                                bodyBuilder.Append(lines[i].Substring(lines[i].IndexOf(':') + 1).Trim() + "<br/>");
                            }
                            else
                            {
                                bodyBuilder.Append(lines[i].Trim() + "<br/>");
                            }
                        }
                        body = bodyBuilder.ToString().Trim();

                        html.Append("<div class='conversation-entry'>");
                        html.Append($"<div class='conversation-header'>{HttpUtility.HtmlEncode(header)}</div>");
                        if (!string.IsNullOrEmpty(subject))
                            html.Append($"<div class='conversation-subject'>{HttpUtility.HtmlEncode(subject)}</div>");
                        html.Append($"<div class='conversation-body'>{body}</div>");
                        html.Append("</div>");
                    }
                }

                return html.Length > 0 ? html.ToString() : "<div class='alert alert-info'>No conversation found.</div>";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetConversation Error: " + ex.Message);
                return $"<div class='alert alert-danger'>Error loading conversation: {ex.Message}</div>";
            }
        }

        private static string GetAttachments(string ticketNo)
        {
            try
            {
                string folderPath = Path.Combine(DataPath, ticketNo);
                if (Directory.Exists(folderPath))
                {
                    string[] files = Directory.GetFiles(folderPath);
                    StringBuilder html = new StringBuilder();
                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        string relativePath = $"E:/PIA/{ticketNo}/{fileName}";
                        string ext = Path.GetExtension(fileName).ToLower();
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
                return "No attachments found.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetAttachments Error: " + ex.Message);
                return $"Error loading attachments: {ex.Message}";
            }
        }

        private static string GetForwardHistory(int tkt_no)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT fwd_to, fwd_by, fwd_date, remarks FROM cms_fwd_remarks WHERE tkt_no = @tkt_no ORDER BY fwd_date DESC";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@tkt_no", tkt_no);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            StringBuilder html = new StringBuilder("<table class='table table-bordered forward-history-table'><thead><tr><th>Forwarded To</th><th>Forwarded By</th><th>Date</th><th>Remarks</th></tr></thead><tbody>");
                            bool hasRows = false;
                            while (reader.Read())
                            {
                                hasRows = true;
                                html.Append("<tr>");
                                html.Append($"<td>{HttpUtility.HtmlEncode(reader["fwd_to"].ToString())}</td>");
                                html.Append($"<td>{HttpUtility.HtmlEncode(reader["fwd_by"].ToString())}</td>");
                                html.Append($"<td>{(reader["fwd_date"] != DBNull.Value ? Convert.ToDateTime(reader["fwd_date"]).ToString("yyyy-MM-dd HH:mm") : "N/A")}</td>");
                                html.Append($"<td>{HttpUtility.HtmlEncode(reader["remarks"].ToString())}</td>");
                                html.Append("</tr>");
                            }
                            html.Append("</tbody></table>");
                            return hasRows ? html.ToString() : "<div class='alert alert-info'>No forward history found.</div>";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetForwardHistory Error: " + ex.Message);
                return $"<div class='alert alert-danger'>Error loading forward history: {ex.Message}</div>";
            }
        }
    }
}