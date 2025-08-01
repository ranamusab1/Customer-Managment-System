using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace PIA_CMS
{
    public partial class ViewComplaints : Page
    {
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
                string query = "SELECT ComplaintID, TicketNo, Category, Subject, RequestDate, MembershipNo, Email FROM Complaints";
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

        protected void btnForwardSend_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Update Complaints table with forward details
                    string updateQuery = "UPDATE Complaints SET ForwardedTo = @ForwardedTo, ForwardedDate = @ForwardedDate, ForwardRemarks = @ForwardRemarks, ForwardedBy = @ForwardedBy WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@ForwardedTo", ddlForwardTo.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@ForwardedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ForwardRemarks", txtForwardRemarks.Text);
                        cmd.Parameters.AddWithValue("@ForwardedBy", Session["AdminUser"].ToString());
                        cmd.Parameters.AddWithValue("@ComplaintID", Request.Form["pnlForwardComplaint$ctl00"] ?? "0");
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    // Insert into EmailsSent
                    string insertQuery = "INSERT INTO EmailsSent (EmailFrom, EmailTo, EmailSubject, EmailBody, SentDate, UserID, MembershipNo) VALUES (@EmailFrom, @EmailTo, @EmailSubject, @EmailBody, @SentDate, @UserID, @MembershipNo)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmailFrom", Session["AdminUser"].ToString() + "@pia.com");
                        cmd.Parameters.AddWithValue("@EmailTo", txtForwardEmail.Text);
                        cmd.Parameters.AddWithValue("@EmailSubject", txtForwardSubject.Text);
                        cmd.Parameters.AddWithValue("@EmailBody", txtForwardRemarks.Text + "\n\n" + txtForwardBody.Text);
                        cmd.Parameters.AddWithValue("@SentDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserID", ddlForwardTo.SelectedValue);
                        cmd.Parameters.AddWithValue("@MembershipNo", "");
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "successAlert", "alert('Complaint forwarded successfully.'); $('#pnlForwardComplaint').modal('hide');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error forwarding complaint: {ex.Message}');", true);
            }
        }

        [WebMethod]
        public static object GetForwardData(int complaintId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT Subject, Body, MembershipNo FROM Complaints WHERE ComplaintID = @ComplaintID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ComplaintID", complaintId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new
                                {
                                    subject = $"Fwd: {reader["Subject"]}",
                                    body = $"--- Forwarded Complaint ---\nSubject: {reader["Subject"]}\nDetails: {reader["Body"]}\nMembership No: {reader["MembershipNo"]}"
                                };
                            }
                            else
                            {
                                return new { subject = "", body = "<div class='alert alert-danger'>Complaint not found.</div>" };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new { subject = "", body = $"<div class='alert alert-danger'>Error: {ex.Message}</div>" };
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
                return "";
            }
        }
    }
}