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
    public partial class ComplaintDetails : Page
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
                string complaintId = Request.QueryString["ComplaintID"];
                if (string.IsNullOrEmpty(complaintId) || !int.TryParse(complaintId, out int id))
                {
                    lblMsg.Text = "Invalid or missing Complaint ID in URL.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                LoadComplaintDetails(id);
            }
        }

        private void LoadComplaintDetails(int complaintId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Query Complaints table
                    string query = "SELECT TicketNo, MembershipNo, RequestDate, Subject, Body, Category, Status, ForwardedTo, Email FROM Complaints WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ComplaintID", complaintId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblComplaintDetails.Text = $"Ticket No: {(reader["TicketNo"] != DBNull.Value ? reader["TicketNo"] : "N/A")}<br/>" +
                                                          $"Membership No: {(reader["MembershipNo"] != DBNull.Value ? reader["MembershipNo"] : "N/A")}<br/>" +
                                                          $"Request Date: {(reader["RequestDate"] != DBNull.Value ? Convert.ToDateTime(reader["RequestDate"]).ToString("yyyy-MM-dd") : "N/A")}<br/>" +
                                                          $"Subject: {(reader["Subject"] != DBNull.Value ? reader["Subject"] : "N/A")}<br/>" +
                                                          $"Details: {(reader["Body"] != DBNull.Value ? reader["Body"] : "No details available")}<br/>" +
                                                          $"Category: {(reader["Category"] != DBNull.Value ? reader["Category"] : "N/A")}<br/>" +
                                                          $"Status: {(reader["Status"] != DBNull.Value ? (reader["Status"].ToString() == "O" ? "Open" : "Closed") : "N/A")}<br/>" +
                                                          $"Forwarded To: {(reader["ForwardedTo"] != DBNull.Value ? reader["ForwardedTo"] : "Not forwarded")}";
                                hfComplaintID.Value = complaintId.ToString();
                                txtReplyTo.Text = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";
                                txtReplyFrom.Text = "your-email@pia.com";
                                txtReplySubject.Text = reader["Subject"] != DBNull.Value ? $"Re: {reader["Subject"]}" : "Re: Complaint";
                            }
                            else
                            {
                                lblComplaintDetails.Text = "Complaint not found.";
                                lblMsg.Text = $"No data found for ComplaintID: {complaintId}";
                                lblMsg.ForeColor = System.Drawing.Color.Red;
                                return;
                            }
                        }
                    }

                    // Query EmailsSent table
                    query = "SELECT EmailSubject, EmailBody, SentDate FROM EmailsSent WHERE RefNo = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ComplaintID", complaintId.ToString());
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            string emailDetails = "";
                            while (reader.Read())
                            {
                                emailDetails += $"Subject: {(reader["EmailSubject"] != DBNull.Value ? reader["EmailSubject"] : "N/A")}<br/>" +
                                                $"Body: {(reader["EmailBody"] != DBNull.Value ? reader["EmailBody"] : "N/A")}<br/>" +
                                                $"Sent: {(reader["SentDate"] != DBNull.Value ? Convert.ToDateTime(reader["SentDate"]).ToString("yyyy-MM-dd HH:mm:ss") : "N/A")}<br/><hr/>";
                            }
                            lblEmailDetails.Text = string.IsNullOrEmpty(emailDetails) ? "No emails sent for this complaint." : emailDetails;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error loading complaint details: {ex.Message}";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnMoveCategory_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlMoveCategory.SelectedValue))
                {
                    lblMsg.Text = "Please select a category to move the complaint.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Complaints SET Category = @Category WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Category", ddlMoveCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@ComplaintID", hfComplaintID.Value);
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            lblMsg.Text = "No complaint found to update category.";
                            lblMsg.ForeColor = System.Drawing.Color.Red;
                            return;
                        }
                    }
                }
                lblMsg.Text = "Category updated successfully.";
                lblMsg.ForeColor = System.Drawing.Color.Green;
                LoadComplaintDetails(Convert.ToInt32(hfComplaintID.Value));
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error updating category: {ex.Message}";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(hfComplaintID.Value))
                {
                    lblMsg.Text = "No complaint selected for reply.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                // Ensure ScriptManager is present
                if (ScriptManager.GetCurrent(this) != null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showReplyModal", "showReplyModal();", true);
                }
                else
                {
                    lblMsg.Text = "ScriptManager is missing on the page.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error opening reply modal: {ex.Message}";
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
                LoadComplaintDetails(Convert.ToInt32(hfComplaintID.Value));
                if (ScriptManager.GetCurrent(this) != null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "hideReplyModal", "$('#pnlReply').modal('hide');", true);
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error sending reply: {ex.Message}";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnForward_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(hfComplaintID.Value))
                {
                    lblMsg.Text = "No complaint selected for forwarding.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT Email FROM Admins";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlForwardTo.DataSource = reader;
                            ddlForwardTo.DataTextField = "Email";
                            ddlForwardTo.DataValueField = "Email";
                            ddlForwardTo.DataBind();
                        }
                    }
                }
                ddlForwardTo.Items.Insert(0, new ListItem("Select Forward To", ""));
                if (ScriptManager.GetCurrent(this) != null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showForwardModal", "showForwardModal();", true);
                }
                else
                {
                    lblMsg.Text = "ScriptManager is missing on the page.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error loading forward recipients: {ex.Message}";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnSendForward_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlForwardTo.SelectedValue))
                {
                    lblMsg.Text = "Please select a recipient to forward the complaint.";
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
                LoadComplaintDetails(Convert.ToInt32(hfComplaintID.Value));
                if (ScriptManager.GetCurrent(this) != null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "hideForwardModal", "$('#pnlForward').modal('hide');", true);
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error forwarding complaint: {ex.Message}";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnMarkSolved_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(hfComplaintID.Value))
                {
                    lblMsg.Text = "No complaint selected to mark as solved.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Complaints SET Status = 'C', UpdateDate = @UpdateDate, UpdatedBy = @UpdatedBy WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UpdateDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UpdatedBy", Session["AdminUser"].ToString());
                        cmd.Parameters.AddWithValue("@ComplaintID", hfComplaintID.Value);
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            lblMsg.Text = "No complaint found to mark as solved.";
                            lblMsg.ForeColor = System.Drawing.Color.Red;
                            return;
                        }
                    }
                }
                lblMsg.Text = "Complaint marked as solved.";
                lblMsg.ForeColor = System.Drawing.Color.Green;
                LoadComplaintDetails(Convert.ToInt32(hfComplaintID.Value));
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error marking complaint as solved: {ex.Message}";
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}