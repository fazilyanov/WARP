using System.Web;

namespace WARP
{
    /// <summary>
    /// Обрабатывает запросы от DataTables
    /// </summary>
    public class Handler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int draw = int.Parse(context.Request["draw"]);
            int displayLength = int.Parse(context.Request["length"]);
            int displayStart = int.Parse(context.Request["start"]);
            int sortCol = int.Parse(context.Request["order[0][column]"]);
            string sortDir = context.Request["order[0][dir]"];
            string search = context.Request["search[value]"];

            context.Response.Write(DataTablesTestPage.GetJsonData(draw, displayStart, displayLength, sortCol, sortDir));
            
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