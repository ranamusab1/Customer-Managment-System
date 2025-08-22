using System;
using System.Web.UI;

namespace PIA_CMS
{
    public partial class Home : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminUser"] == null)
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}