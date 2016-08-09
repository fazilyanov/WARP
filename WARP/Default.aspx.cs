using System;
using System.Data;

namespace WARP
{
    public partial class Default : System.Web.UI.Page
    {
      

        protected void Page_PreRender(object sender, EventArgs e)
        {
            //DataTable dt = ComFunc.GetBaseData();
            //foreach (DataRow row in dt.Rows)
            //{
           //     htmlBaseList += "<a href=\"" + GetRouteUrl("archive", new { pBase = row["Name"], pPage = ArchivePage.All.ToString() }) + "\" class=\"list-group-item\">" + row["NameRus"] + "</a>" + Environment.NewLine;
            //}
        }
    }
}