using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace PIA_CMS
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.Text = "";
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Please enter both username and password.";
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["dbConn"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                lblMessage.Text = "Database connection string is missing. Please contact the administrator.";
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM Admins WHERE Username = @Username AND Password = @Password";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        if (count == 1)
                        {
                            Session["AdminUser"] = username;
                            Response.Redirect("Home.aspx"); // Change to "DepartmentDashboard.aspx" if it exists
                        }
                        else
                        {
                            lblMessage.Text = "Invalid username or password.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error connecting to database: {ex.Message}";
            }
        }
    }
}