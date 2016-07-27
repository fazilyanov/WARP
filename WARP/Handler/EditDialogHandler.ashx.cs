using System;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace WARP
{
    /// <summary>
    /// Обрабатывает запросы от DataTables
    /// </summary>
    public class EditDialogHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            // База
            string curBase = context.Request["curBase"];

            // Таблица
            string curTable = context.Request["curTable"];

            // Страница
            string curPage = context.Request["curPage"];

            // ID
            string curId = context.Request["curId"];

            //// В зависимости от таблицы, используем соответствующий класс
            //AppPage appPage = null;
            //switch (curTable)
            //{
            //    case "Archive":
            //        appPage = new AppPageArchive();
            //        break;

            //    case "Frm":
            //      //  appPage = new AppPageFrm();
            //        break;

            //    case "User":

            //        break;

            //    default:
            //        break;
            //}
            //appPage.Master.Init(curBase, curTable, curPage, drawCount, displayStart, displayLength, sortCol, sortDir);
            //context.Response.Write(appPage.Master.GetJsonData());
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div class=\"modal-header\">");
            sb.AppendLine("        <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>");
            sb.AppendLine("        <h4 class=\"modal-title\">Modal title</h4>");
            sb.AppendLine("      </div>");
            sb.AppendLine("      <div id=\"EditDialogBody\" class=\"modal-body\">");
            sb.AppendLine("        <p>"+DateTime.Now.ToString()+"</p>");
            sb.AppendLine("        <p>" + curId + "</p>");
            sb.AppendLine("      </div>");
            sb.AppendLine("      <div class=\"modal-footer\">");
            sb.AppendLine("        <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Close</button>");
            sb.AppendLine("        <button type=\"button\" class=\"btn btn-primary\">Save changes</button>");
            sb.AppendLine("      </div>");
            context.Response.Write(sb.ToString());
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