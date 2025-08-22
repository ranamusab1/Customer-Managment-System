using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace PIA_CMS
{
    public partial class ReceiveEmail : Page
    {
        private static readonly string DataPath = @"C:\PIA";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ticketNo = Request.QueryString["ticketNo"];
                string emailFrom = Request.QueryString["emailFrom"];
                string emailSubject = Request.QueryString["emailSubject"];
                string emailBody = Request.QueryString["emailBody"];
                string membershipNo = Request.QueryString["membershipNo"];

                if (!string.IsNullOrEmpty(ticketNo))
                {
                    SaveEmailToFile(ticketNo, emailFrom, emailSubject, emailBody);
                    SaveToDatabase(ticketNo, emailFrom, emailSubject, emailBody, membershipNo);
                    lblMsg.Text = "Email received and saved successfully.";
                    lblMsg.CssClass = "alert alert-success";
                    lblMsg.Visible = true;
                }
            }
        }

        private void SaveEmailToFile(string ticketNo, string emailFrom, string emailSubject, string emailBody)
        {
            try
            {
                if (!Directory.Exists(DataPath))
                    Directory.CreateDirectory(DataPath);

                string filePath = Path.Combine(DataPath, $"{ticketNo}.txt");
                string content = $"--- Email from {emailFrom} on {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---\nSubject: {emailSubject}\nBody: {emailBody}";
                File.WriteAllText(filePath, content);

                string folderPath = Path.Combine(DataPath, ticketNo);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];
                        if (file != null && file.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string savePath = Path.Combine(folderPath, fileName);
                            file.SaveAs(savePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error saving email: {ex.Message}";
                lblMsg.CssClass = "alert alert-danger";
                lblMsg.Visible = true;
            }
        }

        private void SaveToDatabase(string ticketNo, string emailFrom, string emailSubject, string emailBody, string membershipNo)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO cms (tkt_no, ffnum, Req_Date, Category, Subject, cstatus, email, Req_By) " +
                                   "VALUES (@Tkt_no, @Ffnum, @Req_Date, @Category, @Subject, @Cstatus, @Email, @Req_By)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Tkt_no", ticketNo);
                        cmd.Parameters.AddWithValue("@Ffnum", membershipNo ?? "");
                        cmd.Parameters.AddWithValue("@Req_Date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Category", "Service Center (Emails)");
                        cmd.Parameters.AddWithValue("@Subject", emailSubject);
                        cmd.Parameters.AddWithValue("@Cstatus", "O");
                        cmd.Parameters.AddWithValue("@Email", emailFrom);
                        cmd.Parameters.AddWithValue("@Req_By", "Customer");
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error saving to database: {ex.Message}";
                lblMsg.CssClass = "alert alert-danger";
                lblMsg.Visible = true;
            }
        }
    }
}