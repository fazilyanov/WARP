using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace WARP
{
    /// <summary>
    /// Обрабатывает запросы от Typeahead
    /// </summary>
    public class TypeaheadHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string query = context.Request.QueryString["q"].Trim();
            string curTable = context.Request.QueryString["t"];
            string curBase = context.Request.QueryString["b"];

            context.Response.ContentType = "application/json";
            int bufInt = 0;
            string sqlQuery = string.Empty;
            switch (curTable)
            {
                case "Archive":
                    sqlQuery = "SELECT TOP 30 Id, DocNum as Name FROM [" + curBase + curTable + "] WHERE Active=1 AND Del=0 AND DocNum LIKE'%" + query + "%' " + (int.TryParse(query, out bufInt) ? " OR Id=" + query : "") + " ORDER by DocNum";
                    break;

                default:
                    sqlQuery = "SELECT TOP 30 Id, Name FROM [" + curTable + "] WHERE Del=0 " + (query.Length > 0 ? "AND Name LIKE'%" + query + "%' " : "") + " ORDER by Name";
                    break;
            }
            DataTable dt = Db.GetData(sqlQuery);
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            if (curTable == "User")
            {
                row = new Dictionary<string, object>();
                row.Add("ID", HttpContext.Current.Session["UserId"].ToString());
                row.Add("Name", "Я");
                data.Add(row);
            }
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                row.Add("ID", dr["ID"].ToString());
                row.Add("Name", dr["Name"].ToString());
                data.Add(row);
            }

            JavaScriptSerializer js = new JavaScriptSerializer();

            context.Response.Write(js.Serialize(data));
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