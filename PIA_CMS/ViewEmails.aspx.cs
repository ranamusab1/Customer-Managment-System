using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.IO;
using System.Text;
using System.Web;

namespace PIA_CMS
{
    public partial class ViewEmails : Page
    {
        private static readonly string EmailPath = @"C:\Email";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminUser"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (!IsPostBack)
            {
                BindEmails();
            }
        }

        private void BindEmails()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT sno, userid, senddate, emlsub, sendto, ffnum FROM sendmaillist";
                    if (!string.IsNullOrEmpty(ddlFilterBy.SelectedValue))
                    {
                        query += " WHERE ";
                        if (ddlFilterBy.SelectedValue == "senddate")
                        {
                            query += "CONVERT(VARCHAR, senddate, 120) LIKE @FilterValue";
                        }
                        else
                        {
                            query += $"{ddlFilterBy.SelectedValue} LIKE @FilterValue";
                        }
                    }
                    query += " ORDER BY senddate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (!string.IsNullOrEmpty(ddlFilterBy.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@FilterValue", "%" + txtFilter.Text + "%");
                        }
                        con.Open();
                        gvEmails.DataSource = cmd.ExecuteReader();
                        gvEmails.DataBind();
                        System.Diagnostics.Debug.WriteLine($"BindEmails: Query executed with filter={ddlFilterBy.SelectedValue}, value={txtFilter.Text}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BindEmails Error: " + ex.Message);
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error loading emails: {ex.Message}');", true);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlFilterBy.SelectedValue) && !string.IsNullOrEmpty(txtFilter.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", "alert('Please select a filter type before searching.');", true);
                return;
            }
            BindEmails();
        }

        protected void gvEmails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);
                    int emailId = Convert.ToInt32(gvEmails.DataKeys[rowIndex].Value);
                    System.Diagnostics.Debug.WriteLine($"gvEmails_RowCommand: sno={emailId}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "showModal", $"showEmailDetailsModal({emailId});", true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("gvEmails_RowCommand Error: " + ex.Message);
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error opening email details: {ex.Message}');", true);
                }
            }
        }

        [WebMethod]
        public static object GetEmailDetails(int sno)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                string userid = "N/A";
                string senddate = "N/A";
                string emlsub = "N/A";
                string sendto = "N/A";
                string ffnum = "N/A";

                // Fetch email metadata from sendmaillist
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT userid, senddate, emlsub, sendto, ffnum FROM sendmaillist WHERE sno = @sno";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@sno", sno);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userid = reader["userid"] != DBNull.Value ? reader["userid"].ToString() : "N/A";
                                senddate = reader["senddate"] != DBNull.Value ? Convert.ToDateTime(reader["senddate"]).ToString("yyyy-MM-dd HH:mm") : "N/A";
                                emlsub = reader["emlsub"] != DBNull.Value ? reader["emlsub"].ToString() : "N/A";
                                sendto = reader["sendto"] != DBNull.Value ? reader["sendto"].ToString() : "N/A";
                                ffnum = reader["ffnum"] != DBNull.Value ? reader["ffnum"].ToString() : "N/A";
                                System.Diagnostics.Debug.WriteLine($"GetEmailDetails: sno={sno}, userid={userid}, sendto={sendto}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"GetEmailDetails: Email not found for sno={sno}");
                                return new
                                {
                                    details = "<div class='alert alert-danger'>Email not found.</div>",
                                    content = ""
                                };
                            }
                        }
                    }
                }

                // Fetch email content from file
                string emailContent = GetEmailContent(sno);

                // Return metadata and content
                return new
                {
                    details = $"<div class='card'>" +
                              "<div class='card-header'>Email Information</div>" +
                              "<div class='card-body'>" +
                              $"<strong>Sent By:</strong> {HttpUtility.HtmlEncode(userid)}<br/>" +
                              $"<strong>Sent Date:</strong> {HttpUtility.HtmlEncode(senddate)}<br/>" +
                              $"<strong>Subject:</strong> {HttpUtility.HtmlEncode(emlsub)}<br/>" +
                              $"<strong>Sent To:</strong> {HttpUtility.HtmlEncode(sendto)}<br/>" +
                              $"<strong>Membership No:</strong> {HttpUtility.HtmlEncode(ffnum)}" +
                              "</div></div>",
                    content = emailContent
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetEmailDetails Error: sno={sno}, Error={ex.Message}");
                return new
                {
                    details = $"<div class='alert alert-danger'>Error loading email details: {ex.Message}</div>",
                    content = ""
                };
            }
        }

        private static string GetEmailContent(int sno)
        {
            try
            {
                string filePath = Path.Combine(EmailPath, $"{sno}.txt");
                System.Diagnostics.Debug.WriteLine($"GetEmailContent: Attempting to read file={filePath}");
                if (!File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine($"GetEmailContent: File not found for sno={sno}");
                    return "<div class='alert alert-info'>No email content found.</div>";
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

                System.Diagnostics.Debug.WriteLine($"GetEmailContent: Successfully read content for sno={sno}");
                return html.Length > 0 ? html.ToString() : "<div class='alert alert-info'>No email content found.</div>";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetEmailContent Error: sno={sno}, Error={ex.Message}");
                return $"<div class='alert alert-danger'>Error loading email content: {ex.Message}</div>";
            }
        }
    }
}