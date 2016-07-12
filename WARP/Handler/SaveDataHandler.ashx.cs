using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace WARP
{
    public class RequestData
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }

    public class FieldErrors
    {
        public string name { get; set; }
        public string status { get; set; }
    }

    /// <summary>
    /// Обрабатывает запросы от DataTables
    /// </summary>
    public class SaveDataHandler : IHttpHandler, IRequiresSessionState
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

            switch (curTable)
            {
                case "Archive":
                    // Сохраняем
                    TableDataArchive tableData = new TableDataArchive();
                    tableData.Init(curBase, curTable, curPage);
                    context.Response.Write(tableData.Process(tableAction, requestRows));

                    //{
                    //"data": [
                    //            {
                    //                 "DT_RowId":   "row_29",
                    //                "first_name": "Fiona",
                    //                "last_name":  "Green",
                    //                "position":   "Chief Operating Officer (COO)",
                    //                "office":     "San Francisco",
                    //                "extn":       "2947",
                    //                "salary":     "850000",
                    //                "start_date": "2010-03-11"
                    //            }
                    //        ]
                    //}

                    ////string w = "0";
                    //foreach (string item in rows.Keys)
                    //{
                    //    w += "," + item;
                    //}

                    //context.Response( Archive.GetJsonData(curBase, curTable, curPage, 0, 0, 500, 0, "asc", w));
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