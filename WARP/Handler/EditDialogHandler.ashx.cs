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
            string curPage = context.Request["curPage"] ?? string.Empty;

            // Действие
            Action tableAction = Func.ParseEnum<Action>(context.Request["action"]);

            // ID Шапки
            string idMaster = context.Request["idMaster"] ?? "0";

            // ID
            string curId = context.Request["curId"];

            //
            string showVer = context.Request["showVer"] ?? "0";

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
                    if (showVer == "0")
                        context.Response.Write(Archive.GenerateEditDialog(curBase, curTable, curPage, tableAction, curId));
                    else
                        context.Response.Write(ArchiveVer.GenerateEditDialog(curBase, curTable, curPage, tableAction, curId));
                    break;

                case "Complect":
                    context.Response.Write(Complect.GenerateEditDialog(curBase, curTable, tableAction, curId));
                    break;

                case "ComplectDetail":
                    context.Response.Write(Complect.GenerateEditDialogDetail(curBase, curTable, tableAction, idMaster, curId));
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