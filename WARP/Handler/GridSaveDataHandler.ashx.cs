using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace WARP
{
    

    /// <summary>
    /// Обрабатывает запросы от DataTables
    /// </summary>
    public class GridSaveDataHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            /*
            // База
            string curBase = context.Request["curBase"];

            // Таблица
            string curTable = context.Request["curTable"];

            // Страница
            string curPage = context.Request["curPage"];

            // Действие
            TableAction tableAction = TableAction.None;
            switch (context.Request["action"])
            {
                case "create":
                    tableAction = TableAction.Create;
                    break;

                case "edit":
                    tableAction = TableAction.Edit;
                    break;

                case "remove":
                    tableAction = TableAction.Remove;
                    break;
            }

            // Список переданных строк, ключ - ID
            Dictionary<string, List<RequestData>> requestRows = new Dictionary<string, List<RequestData>>();

            // Парсим переданные параметры
            string[] buf;
            foreach (string item in context.Request.Form.AllKeys)
            {
                if (item != "action") // Пропускаем параметр «action»
                {
                    buf = item.Replace("data[", "").Replace("]", "").Split('[');
                    if (requestRows.ContainsKey(buf[0])) // Если в списке есть строка для текущего ключа, просто добавляем оставшиеся Имя/Значение поля
                    {
                        requestRows[buf[0]].Add(new RequestData { FieldName = buf[1], FieldValue = context.Request.Form[item].Trim() });
                    }
                    else // Если нет, создаем ее
                    {
                        requestRows.Add(buf[0], new List<RequestData> { new RequestData { FieldName = buf[1], FieldValue = context.Request.Form[item].Trim() } });
                    }
                }
            }

            // Инитим соотвествующий класс
            TableData tableData = null;
            switch (curTable)
            {
                case "Archive":
                    tableData = new TableDataArchive();
                    break;

                case "Frm":
                    tableData = new TableDataFrm();
                    break;

                default:
                    break;
            }
            tableData.Init(curBase, curTable, curPage);
            tableData.Action = tableAction;
            tableData.RequestRows = requestRows;

            // Обрабатываем запрос
            context.Response.Write(tableData.Process());
            */
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