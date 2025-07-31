using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PIA_CMS
{
    public partial class ViewComplaints : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminUser"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                string category = Request.QueryString["cat"] ?? "All";
                if (ddlCategory.Items.FindByValue(category) != null)
                {
                    ddlCategory.SelectedValue = category;
                }
                BindComplaints("All");
            }
        }

        private void BindComplaints(string status)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT ComplaintID, MembershipNo, RequestDate, Subject, ForwardedTo, Status, Category FROM Complaints WHERE 1=1";
                    if (status != "All")
                        query += " AND Status = @Status";
                    if (ddlCategory.SelectedValue != "All")
                        query += " AND Category = @Category";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (status != "All")
                            cmd.Parameters.AddWithValue("@Status", status == "Open" ? "O" : "C");
                        if (ddlCategory.SelectedValue != "All")
                            cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);

                        con.Open();
                        gvComplaints.DataSource = cmd.ExecuteReader();
                        gvComplaints.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error loading complaints: {ex.Message}";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void lnkAll_Click(object sender, EventArgs e)
        {
            ddlCategory.SelectedValue = "All";
            BindComplaints("All");
        }

        protected void lnkOpen_Click(object sender, EventArgs e)
        {
            BindComplaints("Open");
        }

        protected void lnkClosed_Click(object sender, EventArgs e)
        {
            BindComplaints("Closed");
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindComplaints("All");
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT ComplaintID, MembershipNo, RequestDate, Subject, ForwardedTo, Status, Category FROM Complaints WHERE 1=1";
                    if (ddlCategory.SelectedValue != "All")
                        query += " AND Category = @Category";
                    if (!string.IsNullOrEmpty(ddlFilter.SelectedValue))
                        query += $" AND {ddlFilter.SelectedValue} LIKE @FilterValue";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (ddlCategory.SelectedValue != "All")
                            cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);
                        if (!string.IsNullOrEmpty(ddlFilter.SelectedValue))
                            cmd.Parameters.AddWithValue("@FilterValue", "%" + txtFilter.Text + "%");

                        con.Open();
                        gvComplaints.DataSource = cmd.ExecuteReader();
                        gvComplaints.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error filtering complaints: {ex.Message}";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void gvComplaints_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                int complaintId = Convert.ToInt32(gvComplaints.DataKeys[rowIndex].Value);

                if (e.CommandName == "Reply")
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "SELECT Email, Subject FROM Complaints WHERE ComplaintID = @ComplaintID";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@ComplaintID", complaintId);
                            con.Open();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    hfComplaintID.Value = complaintId.ToString();
                                    txtReplyTo.Text = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";
                                    txtReplyFrom.Text = "your-email@pia.com";
                                    txtReplySubject.Text = reader["Subject"] != DBNull.Value ? $"Re: {reader["Subject"]}" : "Re: Complaint";
                                }
                                else
                                {
                                    lblMsg.Text = $"No data found for ComplaintID: {complaintId}";
                                    lblMsg.ForeColor = System.Drawing.Color.Red;
                                    return;
                                }
                            }
                        }
                    }
                    ScriptManager.RegisterStartupScript(this, GetType(), "showReplyModal", "showReplyModal();", true);
                }
                else if (e.CommandName == "Forward")
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "SELECT Email FROM Admins";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            con.Open();
                            ddlForwardTo.DataSource = cmd.ExecuteReader();
                            ddlForwardTo.DataTextField = "Email";
                            ddlForwardTo.DataValueField = "Email";
                            ddlForwardTo.DataBind();
                        }
                    }
                    hfComplaintID.Value = complaintId.ToString();
                    ddlForwardTo.Items.Insert(0, new ListItem("Select Forward To", ""));
                    ScriptManager.RegisterStartupScript(this, GetType(), "showForwardModal", "showForwardModal();", true);
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error processing command: {ex.Message}";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnSendReply_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtReplyTo.Text) || string.IsNullOrEmpty(txtReplySubject.Text) || string.IsNullOrEmpty(txtReplyBody.Text))
                {
                    lblMsg.Text = "Please fill in all reply fields.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO EmailsSent (UserID, EmailFrom, EmailTo, EmailSubject, EmailBody, SentDate, RefNo, MembershipNo) " +
                                   "VALUES (@UserID, @EmailFrom, @EmailTo, @EmailSubject, @EmailBody, @SentDate, @RefNo, @MembershipNo)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", Session["AdminUser"].ToString());
                        cmd.Parameters.AddWithValue("@EmailFrom", txtReplyFrom.Text);
                        cmd.Parameters.AddWithValue("@EmailTo", txtReplyTo.Text);
                        cmd.Parameters.AddWithValue("@EmailSubject", txtReplySubject.Text);
                        cmd.Parameters.AddWithValue("@EmailBody", txtReplyBody.Text);
                        cmd.Parameters.AddWithValue("@SentDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@RefNo", hfComplaintID.Value);
                        cmd.Parameters.AddWithValue("@MembershipNo", txtReplyTo.Text);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Save email body to file
                string folderPath = Server.MapPath("~/ComplaintBodies/");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                string fileName = $"Complaint_{hfComplaintID.Value}_Reply_{DateTime.Now.Ticks}.txt";
                File.WriteAllText(Path.Combine(folderPath, fileName), txtReplyBody.Text);

                // Send email via SMTP
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(txtReplyFrom.Text);
                    mail.To.Add(txtReplyTo.Text);
                    mail.Subject = txtReplySubject.Text;
                    mail.Body = txtReplyBody.Text;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential("your-email@pia.com", "your-app-password");
                        smtp.Send(mail);
                    }
                }

                lblMsg.Text = "Reply sent successfully!";
                lblMsg.ForeColor = System.Drawing.Color.Green;
                txtReplyBody.Text = "";
                BindComplaints("All");
                ScriptManager.RegisterStartupScript(this, GetType(), "hideReplyModal", "$('#pnlReply').modal('hide');", true);
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error sending reply: {ex.Message}";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnSendForward_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlForwardTo.SelectedValue))
                {
                    lblMsg.Text = "Please select a forward recipient.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Complaints SET ForwardedTo = @ForwardedTo, ForwardedDate = @ForwardedDate, ForwardRemarks = @ForwardRemarks, ForwardedBy = @ForwardedBy WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ForwardedTo", ddlForwardTo.SelectedValue);
                        cmd.Parameters.AddWithValue("@ForwardedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ForwardRemarks", txtForwardRemarks.Text);
                        cmd.Parameters.AddWithValue("@ForwardedBy", Session["AdminUser"].ToString());
                        cmd.Parameters.AddWithValue("@ComplaintID", hfComplaintID.Value);
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            lblMsg.Text = "No complaint found to forward.";
                            lblMsg.ForeColor = System.Drawing.Color.Red;
                            return;
                        }
                    }
                }

                lblMsg.Text = "Complaint forwarded successfully.";
                lblMsg.ForeColor = System.Drawing.Color.Green;
                txtForwardRemarks.Text = "";
                BindComplaints("All");
                ScriptManager.RegisterStartupScript(this, GetType(), "hideForwardModal", "$('#pnlForward').modal('hide');", true);
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error forwarding complaint: {ex.Message}";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}