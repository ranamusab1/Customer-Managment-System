using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Web.UI;

namespace PIA_CMS
{
    public partial class SendEmail : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminUser"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            // lblAdmin is in Site.master, no need to set it here
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO EmailsSent (UserID, EmailFrom, EmailTo, EmailSubject, EmailBody, SentDate, MembershipNo) " +
                                   "VALUES (@UserID, @EmailFrom, @EmailTo, @EmailSubject, @EmailBody, @SentDate, @MembershipNo)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", Session["AdminUser"].ToString());
                        cmd.Parameters.AddWithValue("@EmailFrom", "your-email@pia.com");
                        cmd.Parameters.AddWithValue("@EmailTo", txtTo.Text);
                        cmd.Parameters.AddWithValue("@EmailSubject", txtSubject.Text);
                        cmd.Parameters.AddWithValue("@EmailBody", txtBody.Text);
                        cmd.Parameters.AddWithValue("@SentDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@MembershipNo", txtMembershipId.Text);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Send email via SMTP
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("your-email@pia.com");
                mail.To.Add(txtTo.Text);
                if (!string.IsNullOrEmpty(txtBCC.Text))
                    mail.Bcc.Add(txtBCC.Text);
                mail.Subject = txtSubject.Text;
                mail.Body = txtBody.Text;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential("your-email@pia.com", "your-app-password");
                smtp.Send(mail);

                lblMsg.Text = "Email sent successfully!";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
            }
        }
    }
}