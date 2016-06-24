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
            string curBase = context.Request["curBase"];
            string curTable = context.Request["curTable"];
            string curPage = context.Request["curPage"];
            int draw = int.Parse(context.Request["draw"]);
            int displayLength = int.Parse(context.Request["length"]);
            int displayStart = int.Parse(context.Request["start"]);
            int sortCol = int.Parse(context.Request["order[0][column]"]);
            string sortDir = context.Request["order[0][dir]"];
            string search = context.Request["search[value]"];
            switch (curTable)
            {
                case "Archive":
                    context.Response.Write(Archive.GetJsonData(curBase, curTable, curPage, draw, displayStart, displayLength, sortCol, sortDir));
                    break;
                case "Frm":
                    context.Response.Write(Frm.GetJsonData(curBase, curTable, curPage, draw, displayStart, displayLength, sortCol, sortDir));
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