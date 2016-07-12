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
            int draw = int.Parse(context.Request["draw"]);
            int displayLength = int.Parse(context.Request["length"]);
            int displayStart = int.Parse(context.Request["start"]);
            int sortCol = int.Parse(context.Request["order[0][column]"]);
            string sortDir = context.Request["order[0][dir]"];
            string search = context.Request["search[value]"];

            // В зависимости от таблицы, используем соответствующий класс
            TableData tableData = null;
            switch (curTable)
            {
                case "Archive":
                    tableData = new TableDataArchive();
                    break;
                case "Frm":
                case "User":
                    tableData = new TableData();
                    break;
                default:
                    break;
            }
            tableData.Init(curBase, curTable, curPage, draw, displayStart, displayLength, sortCol, sortDir);
            context.Response.Write(tableData.GetJsonData());
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