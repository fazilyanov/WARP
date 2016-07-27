using System.Web;
using System.Web.SessionState;

namespace WARP
{
    /// <summary>
    /// Обрабатывает запросы от DataTables
    /// </summary>
    public class GetDataHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            // Получаем параметры от грида
            string curBase = context.Request["curBase"];
            string curTable = context.Request["curTable"];
            string curPage = context.Request["curPage"];
            int drawCount = int.Parse(context.Request["draw"]);
            int displayLength = int.Parse(context.Request["length"]);
            int displayStart = int.Parse(context.Request["start"]);
            int sortCol = int.Parse(context.Request["order[0][column]"]);
            string sortDir = context.Request["order[0][dir]"];
            string search = context.Request["search[value]"];

            // В зависимости от таблицы, используем соответствующий класс
            AppPage appPage = null;
            switch (curTable)
            {
                case "Archive":
                    appPage = new AppPageArchive();
                    break;

                case "Frm":
                  //  appPage = new AppPageFrm();
                    break;

                case "User":

                    break;

                default:
                    break;
            }
            appPage.Master.Init(curBase, curTable, curPage, drawCount, displayStart, displayLength, sortCol, sortDir);
            context.Response.Write(appPage.Master.GetJsonData());
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