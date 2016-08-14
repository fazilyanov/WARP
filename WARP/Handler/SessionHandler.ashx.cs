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
            string page = string.Empty;
            string act = context.Request["act"] ?? string.Empty;
            string val = string.Empty;
            try
            {
                switch (act)
                {
                    case "setfilter":// Значения Формы Фильтра
                        Dictionary<string, string> filterList = new Dictionary<string, string>();
                        foreach (string key in context.Request.Form.AllKeys)
                        {
                            val = context.Request.Form[key].ToString().Trim();
                            if (val.Length > 0 && key != "page" && key != "act")
                            {
                                filterList.Add(key, val);
                            }
                        }
                        HttpContext.Current.Session[context.Request.Form["page"].ToString() + "UserFilterList"] = filterList;
                        break;

                    case "clearfilter":
                        page = context.Request.Form["page"].ToString();
                        HttpContext.Current.Session[page + "UserFilterList"] = null;
                        break;

                    default:
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex);
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