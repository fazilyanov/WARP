using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace WARP
{
    /// <summary>
    /// Принимает запросы от карточки (создание/редактирование/удаление)
    /// </summary>
    public class CardSaveDataHandler : IHttpHandler, IRequiresSessionState
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
            Action tableAction = Func.ParseEnum<Action>(context.Request["action"]);

            // Id
            string curId = context.Request["curId"] ?? "0";

            // ID шапки
            string idMaster = context.Request["idMaster"] ?? "0";

            // Редактирование в строке
            string isInline = context.Request["isInline"] ?? "0";

            // Список переданных строк, ключ - ID
            Dictionary<string, List<RequestData>> requestRows = new Dictionary<string, List<RequestData>>();
            if (isInline == "0")
                requestRows.Add(curId, new List<RequestData>());

            // Парсим переданные параметры
            string id = string.Empty;
            foreach (string item in context.Request.Form.AllKeys)
            {
                if (item != "action")
                {
                    if (isInline == "1")
                    {
                        var arr = item.Split('[', ']');
                        id = arr[1];   // .Substring(item.IndexOf('['), item.IndexOf('[') + 1);
                        if (!requestRows.ContainsKey(id)) // Если в списке есть строка для текущего ключа,
                            requestRows.Add(id, new List<RequestData>()); //просто добавляем оставшиеся Имя/Значение поля
                        requestRows[id].Add(new RequestData { FieldName = arr[3], FieldValue = context.Request.Form[item].Trim() });
                    }
                    else
                        requestRows[curId].Add(new RequestData { FieldName = item, FieldValue = context.Request.Form[item].Trim() });
                }
            }

            // Инитим соотвествующий класс
            switch (curTable)
            {
                case "Archive":
                    context.Response.Write(Archive.Process(curBase, curTable, curPage, tableAction, requestRows, context.Request.Files));
                    break;

                case "Complect":
                    context.Response.Write(Complect.Process(curBase, curTable, curPage, tableAction, requestRows, context.Request.Files));
                    break;

                case "ComplectDetail":
                    context.Response.Write(Complect.ProcessDetail(curBase, curTable, curPage, idMaster, tableAction, requestRows, context.Request.Files));
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