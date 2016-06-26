using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace WARP
{
    /// <summary>
    /// Обрабатывает записывает в сессию
    /// </summary>
    public class SessionHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                Dictionary<string, string> filterList = new Dictionary<string, string>();

                foreach (string key in context.Request.Form.AllKeys)
                {
                    string val = context.Request.Form[key].ToString().Trim();
                    if (val.Length > 0)
                    {
                        filterList.Add(key, val);
                    }
                }
                string page = context.Request.Form["page"].ToString();
                HttpContext.Current.Session[page + "UserFilterList"] = filterList;
            }
            catch (System.Exception ex)
            {
                ComFunc.LogError(ex.Message + "\r\n" + ex.StackTrace);
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