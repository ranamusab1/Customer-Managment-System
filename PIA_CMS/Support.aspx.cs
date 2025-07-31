using System;
using System.Web.UI;

namespace PIA_CMS
{
    public partial class Support : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminUser"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            // lblAdmin is in Site.master, no need to set it here
        }
    }
}