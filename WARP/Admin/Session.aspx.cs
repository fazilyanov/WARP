using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace WARP
{
    public partial class Session : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Write(ComFunc.GetSessionValues());
            Response.End();
        }
        
    }
}