using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PIA_CMS
{
    public partial class ViewEmails : Page
    {
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
            string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT EmailID, UserID, EmailFrom, EmailTo, EmailSubject, SentDate, MembershipNo FROM EmailsSent";
                if (!string.IsNullOrEmpty(ddlFilterBy.SelectedValue))
                    query += $" WHERE {ddlFilterBy.SelectedValue} LIKE @FilterValue";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (!string.IsNullOrEmpty(ddlFilterBy.SelectedValue))
                        cmd.Parameters.AddWithValue("@FilterValue", "%" + txtFilter.Text + "%");

                    con.Open();
                    gvEmails.DataSource = cmd.ExecuteReader();
                    gvEmails.DataBind();
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindEmails();
        }

        protected void gvEmails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                try
                {
                    int emailId = Convert.ToInt32(e.CommandArgument);
                    ScriptManager.RegisterStartupScript(this, GetType(), "showEmailDetailsModal", $"showEmailDetailsModal({emailId});", true);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "errorAlert", $"alert('Error opening email details: {ex.Message}');", true);
                }
            }
        }
    }
}