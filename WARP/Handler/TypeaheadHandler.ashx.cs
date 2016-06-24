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
            string query = context.Request.QueryString["q"];
            string table = context.Request.QueryString["t"];

            context.Response.ContentType = "application/json";

            string sqlQuery = string.Empty;
            switch (table)
            {
                default:
                    sqlQuery = "SELECT TOP 15 ID, Name FROM " + table + " WHERE Name LIKE'" + query + "%'";
                    break;
            }
            DataTable dt = ComFunc.GetData(sqlQuery);
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

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