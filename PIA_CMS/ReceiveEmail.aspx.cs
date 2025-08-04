using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace PIA_CMS
{
    public partial class ReceiveEmail : System.Web.UI.Page
    {
        private static readonly string DataPath = @"C:\PIA\";

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
                }
            }
        }

        private void SaveEmailToFile(string ticketNo, string emailFrom, string emailSubject, string emailBody)
        {
            try
            {
                // Ensure directory exists
                if (!Directory.Exists(DataPath))
                    Directory.CreateDirectory(DataPath);

                // Save email body to .txt file
                string filePath = Path.Combine(DataPath, $"{ticketNo}.txt");
                string content = $"--- Email from {emailFrom} on {DateTime.Now:yyyy-MM-dd HH:mm:ss} ---\nSubject: {emailSubject}\nBody: {emailBody}";
                File.WriteAllText(filePath, content);

                // Create folder for attachments
                string folderPath = Path.Combine(DataPath, ticketNo);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Save attachments
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
                Response.Write($"Error saving email: {ex.Message}");
            }
        }

        private void SaveToDatabase(string ticketNo, string emailFrom, string emailSubject, string emailBody, string membershipNo)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Complaints (TicketNo, MembershipNo, RequestDate, Category, Subject, Status, Email, ReceivedFrom, Body) " +
                                   "VALUES (@TicketNo, @MembershipNo, @RequestDate, @Category, @Subject, @Status, @Email, @ReceivedFrom, @Body)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@TicketNo", ticketNo);
                        cmd.Parameters.AddWithValue("@MembershipNo", membershipNo ?? "");
                        cmd.Parameters.AddWithValue("@RequestDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Category", "Service Center");
                        cmd.Parameters.AddWithValue("@Subject", emailSubject);
                        cmd.Parameters.AddWithValue("@Status", "O");
                        cmd.Parameters.AddWithValue("@Email", emailFrom);
                        cmd.Parameters.AddWithValue("@ReceivedFrom", "Customer");
                        cmd.Parameters.AddWithValue("@Body", emailBody);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write($"Error saving to database: {ex.Message}");
            }
        }
    }
}