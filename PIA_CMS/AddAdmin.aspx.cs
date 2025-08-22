using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace PIA_CMS
{
    public partial class AddAdmin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["AdminUser"] == null)
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        protected void btnAddAdmin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string name = txtName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text.Trim();
                string role = ddlRole.SelectedValue;

                if (string.IsNullOrEmpty(username))
                {
                    lblMessage.Text = "Username is required!";
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Visible = true;
                    return;
                }
                if (string.IsNullOrEmpty(name))
                {
                    lblMessage.Text = "Name is required!";
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Visible = true;
                    return;
                }
                if (string.IsNullOrEmpty(email))
                {
                    lblMessage.Text = "Email is required!";
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Visible = true;
                    return;
                }
                if (!Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                {
                    lblMessage.Text = "Invalid email format!";
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Visible = true;
                    return;
                }
                if (string.IsNullOrEmpty(password))
                {
                    lblMessage.Text = "Password is required!";
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Visible = true;
                    return;
                }
                if (password.Length < 6)
                {
                    lblMessage.Text = "Password must be at least 6 characters long!";
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Visible = true;
                    return;
                }
                if (string.IsNullOrEmpty(role))
                {
                    lblMessage.Text = "Role is required!";
                    lblMessage.CssClass = "alert alert-danger";
                    lblMessage.Visible = true;
                    return;
                }

                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string checkQuery = "SELECT COUNT(*) FROM admin_login WHERE user_name = @Username OR EMAIL = @Email";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            lblMessage.Text = "Username or Email already exists!";
                            lblMessage.CssClass = "alert alert-danger";
                            lblMessage.Visible = true;
                            return;
                        }
                    }

                    string insertQuery = "INSERT INTO admin_login (user_name, NameOfEmployee, EMAIL, password, enable, creation_date) VALUES (@Username, @Name, @Email, @Password, @Enable, @CreationDate)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Enable", 1);
                        cmd.Parameters.AddWithValue("@CreationDate", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = $"User added successfully! Username: {username}";
                lblMessage.CssClass = "alert alert-success";
                lblMessage.Visible = true;
                txtUsername.Text = "";
                txtName.Text = "";
                txtEmail.Text = "";
                txtPassword.Text = "";
                ddlRole.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error adding user: {ex.Message}";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
            }
        }
    }
}