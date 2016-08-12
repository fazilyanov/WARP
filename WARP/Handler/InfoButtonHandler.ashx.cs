using System.Web;
using System.Web.SessionState;

namespace WARP
{
    /// <summary>
    /// Обрабатывает запросы
    /// </summary>
    public class InfoButtonHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            // База
            string curBase = context.Request["curBase"];

            // Таблица
            string curTable = context.Request["curTable"];

            // Страница
            string curPage = context.Request["curPage"];

            // 
            string showVer = context.Request["showVer"] ?? "0";

            // ID
            string curId = context.Request["curId"];

            // В зависимости от таблицы, используем соответствующий класс

            switch (curTable)
            {
                //case "Archive":
                //    AppPage appPage = new AppPageArchive();
                //    appPage.Master.Init(curBase, curTable, curPage);
                //    context.Response.Write(appPage.Master.GenerateJSTableInfoButtonContent(curId));
                //    break;

                case "Archive":
                    if (showVer == "0")
                        context.Response.Write(Archive.GenerateJSTableInfoButtonContent(curBase, curTable, curId));
                    else
                        context.Response.Write(ArchiveVer.GenerateJSTableInfoButtonContent(curBase, curTable, curId));
                    break;

                case "Complect":
                    context.Response.Write(Complect.GenerateJSTableInfoButtonContent(curBase, curTable, curId));
                    break;

                default:
                    break;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}