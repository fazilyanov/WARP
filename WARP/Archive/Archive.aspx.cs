using System;

namespace WARP
{
    public partial class Archive : System.Web.UI.Page
    {
        public string browserTabTitle;
        public string curPage;
        public string documentTitle;
        public TableDataArchive tableData;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            curPage = (Page.RouteData.Values["pPage"] ?? "").ToString().Trim();
            if (curPage.Length > 0)
            {
                browserTabTitle = ComFunc.GetArchivePageNameRus(curPage);
                documentTitle = "Электронный архив | База: " + Master.curBaseNameRus + " | Документы | " + browserTabTitle;
                tableData = new TableDataArchive();
                tableData.Init(Master.curBaseName, "Archive", curPage);
            }
            else
            {
                Response.Write("bad param");
                Response.End();
            }
        }
    }
}