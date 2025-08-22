using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PIA_CMS
{
    public partial class ManageUsers : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminUser"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (!IsPostBack)
            {
                BindUsers();
            }
        }

        private void BindUsers()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT sno, user_name, NameOfEmployee, EMAIL, enable FROM admin_login";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        gvUsers.DataSource = cmd.ExecuteReader();
                        gvUsers.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error loading users: {ex.Message}";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
            }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ToggleStatus")
            {
                try
                {
                    int adminId = Convert.ToInt32(e.CommandArgument);
                    string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE admin_login SET enable = CASE WHEN enable = 1 THEN 0 ELSE 1 END WHERE sno = @Sno";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@Sno", adminId);
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    lblMessage.Text = "User status updated successfully.";
                    lblMessage.CssClass = "alert alert-success";
                    lblMessage.Visible = true;
                    BindUsers();
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Error updating user status: {ex.Message}";
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}