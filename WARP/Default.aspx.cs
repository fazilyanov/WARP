using System;
using System.Data;

namespace WARP
{
    public partial class Default : System.Web.UI.Page
    {
        public string htmlBaseList = string.Empty;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            DataTable dt = ComFunc.GetBaseList();
            foreach (DataRow row in dt.Rows)
            {
                htmlBaseList += "<a href=\"" + GetRouteUrl("archive", new { pBase = row["Name"], pPage = ArchivePage.All.ToString() }) + "\" class=\"list-group-item\">" + row["NameRus"] + "</a>" + Environment.NewLine;
            }
        }
    }
}