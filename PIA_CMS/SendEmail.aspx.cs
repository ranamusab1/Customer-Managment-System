using System;
using System.Configuration;
using System.Data.SqlClient;
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
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO sendmaillist (userid, senddate, emlsub, sendto, cc, ffnum) VALUES (@UserID, @Senddate, @Emlsub, @Sendto, @CC, @Ffnum)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", Session["AdminUser"].ToString());
                        cmd.Parameters.AddWithValue("@Senddate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Emlsub", txtSubject.Text);
                        cmd.Parameters.AddWithValue("@Sendto", txtTo.Text);
                        cmd.Parameters.AddWithValue("@CC", txtBCC.Text);
                        cmd.Parameters.AddWithValue("@Ffnum", txtMembershipId.Text);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                lblMsg.Text = "Email sent successfully.";
                lblMsg.CssClass = "alert alert-success";
                lblMsg.Visible = true;
                txtMembershipId.Text = "";
                txtTo.Text = "";
                txtBCC.Text = "";
                txtSubject.Text = "";
                txtBody.Text = "";
            }
            catch (Exception ex)
            {
                lblMsg.Text = $"Error sending email: {ex.Message}";
                lblMsg.CssClass = "alert alert-danger";
                lblMsg.Visible = true;
            }
        }
    }
}