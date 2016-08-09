using System.Collections.Generic;
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
            int sortColi = int.Parse(context.Request["order[0][column]"]);
            string sortDir = context.Request["order[0][dir]"];
            string search = context.Request["search[value]"];

            // В зависимости от таблицы, используем соответствующий класс
            switch (curTable)
            {
                //case "Archive":
                //    AppPage appPage = new AppPageArchive();
                //    appPage.Master.Init(curBase, curTable, curPage, drawCount, displayStart, displayLength, sortColi, sortDir);
                //    context.Response.Write(appPage.Master.GetJsonData());
                //    break;

                case "Archive":
                    context.Response.Write(Archive.GetJsonData(curBase, curTable, Archive.GetData(curBase, curTable, curPage, displayStart, displayLength, Archive.GetColumnNameByIndex(sortColi), sortDir), drawCount));
                    break;

                

                default:
                    context.Response.Write("you one ugly mother f..");
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