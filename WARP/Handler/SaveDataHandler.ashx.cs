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

    /// <summary>
    /// Обрабатывает запросы от DataTables
    /// </summary>
    public class SaveDataHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string curBase = context.Request["curBase"];
            string curTable = context.Request["curTable"];
            string curPage = context.Request["curPage"];
            string action = context.Request["action"];

            Dictionary<string, List<RequestData>> rows = new Dictionary<string, List<RequestData>>();
            string[] buf;
            foreach (string item in context.Request.Form.AllKeys)
            {
                if (item != "action")
                {
                    buf = item.Replace("data[", "").Replace("]", "").Split('[');
                    if (rows.ContainsKey(buf[0]))
                    {
                        rows[buf[0]].Add(new RequestData { FieldName = buf[1], FieldValue = context.Request.Form[item] });
                    }
                    else
                    {
                        rows.Add(buf[0], new List<RequestData> { new RequestData { FieldName = buf[1], FieldValue = context.Request.Form[item] } });
                    }
                }
            }

            switch (curTable)
            {
                case "Archive":
                    Archive.SaveData(curBase, curTable, curPage, action, rows);
                    string w = "0";
                    foreach (string item in rows.Keys)
                    {
                        w += "," + item;
                    }

                    context.Response.Write( Archive.GetJsonData(curBase, curTable, curPage, 0, 0, 500, 0, "asc", w));
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