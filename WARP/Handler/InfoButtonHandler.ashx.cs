using System.Text;
using System.Web;
using System.Web.SessionState;

namespace WARP
{
    /// <summary>
    /// Обрабатывает запросы 
    /// </summary>
    public class InfoButtonHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            // База
            string curBase = context.Request["curBase"];

            // Таблица
            string curTable = context.Request["curTable"];

            // Страница
            string curPage = context.Request["curPage"];
            
            // ID
            string curId = context.Request["curId"];
            
            // В зависимости от таблицы, используем соответствующий класс
            AppPage appPage = null;
            switch (curTable)
            {
                case "Archive":
                    appPage = new AppPageArchive();
                    break;

                case "Frm":
                    
                    break;

                case "User":

                    break;

                default:
                    break;
            }

            StringBuilder sb = new StringBuilder();
            appPage.Master.Init(curBase, curTable, curPage);
            context.Response.Write(appPage.Master.GenerateJSTableInfoButtonContent(curId));
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