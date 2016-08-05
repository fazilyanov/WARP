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

            // Действие
            TableAction tableAction = ComFunc.ParseEnum<TableAction>(context.Request["action"]);

            // ID
            string curId = context.Request["curId"];

            // В зависимости от таблицы, используем соответствующий класс
            switch (curTable)
            {
                //case "Archive":
                //    AppPage appPage = new AppPageArchive();
                //    appPage.Master.Init(curBase, curTable, curPage);
                //    appPage.Master.Action = tableAction;
                //    context.Response.Write(appPage.GenerateEditDialog(curId));
                //    break;

                case "Archive":
                    context.Response.Write(Archive.GenerateEditDialog(curBase, curTable,curPage,tableAction,curId));
                    break;

                case "User":

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