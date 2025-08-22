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
                Session.Clear();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT sno, user_name, NameOfEmployee FROM admin_login WHERE user_name = @Username AND password = @Password AND enable = 1";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int sno = reader.GetInt32(0);
                                Session["AdminUser"] = reader["user_name"].ToString();
                                Session["NameOfEmployee"] = reader["NameOfEmployee"].ToString();
                                Session["AdminId"] = sno.ToString();

                                con.Close();
                                con.Open();
                                string updateQuery = "UPDATE admin_login SET LastLogin = @LastLogin, logincount = ISNULL(logincount, 0) + 1 WHERE sno = @Sno";
                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                                {
                                    updateCmd.Parameters.AddWithValue("@LastLogin", DateTime.Now);
                                    updateCmd.Parameters.AddWithValue("@Sno", sno);
                                    updateCmd.ExecuteNonQuery();
                                }

                                Response.Redirect("Home.aspx");
                            }
                            else
                            {
                                lblMessage.Text = "Invalid username or password.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
            }
        }
    }
}