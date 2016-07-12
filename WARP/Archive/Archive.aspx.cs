using System;

namespace WARP
{
    public partial class Archive : System.Web.UI.Page
    {
        public TableDataArchive tableData;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            string curPage = (Page.RouteData.Values["pPage"] ?? "").ToString().Trim();
            if (curPage.Length > 0)
            {
                tableData = new TableDataArchive();
                tableData.Init(Master.curBaseName, "Archive", curPage);
                tableData.BrowserTabTitle = ComFunc.GetArchivePageNameRus(curPage);
                tableData.PageTitle = "Электронный архив | База: " + Master.curBaseNameRus + " | Документы | " + tableData.BrowserTabTitle;
            }
            else
            {
                Response.Write("bad param");
                Response.End();
            }
        }
    }
}