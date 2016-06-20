using System;

namespace WARP
{
    public partial class Pinger : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Write((Session["user_login"] ?? "none").ToString());
            Response.End();
        }
    }
}