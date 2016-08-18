using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

/// scio me nihil scire
/// 19.06.2016
/// Artur Fazilyanov
namespace WARP
{
    public partial class Site : System.Web.UI.MasterPage
    {
        /// <summary>
        /// Сокращенное имя текущей базы, переданное в параметре адресной строки
        /// </summary>
        public string curBase;

        /// <summary>
        /// Полное наименование текущей базы / организации
        /// </summary>
        public string curBaseNameRus;

        /// <summary>
        /// Страница или мод
        /// </summary>
        public string curPage;

        private DataTable GetUserInfo(string login)
        {
            return Db.GetData("SELECT * FROM [dbo].[User] WHERE Del = 0 AND Login = @login", new SqlParameter("login", login));
        }

        /// <summary>
        /// Возвращает список всех баз
        /// </summary>
        /// <returns>DataTable</returns>
        private DataTable GetBaseData()
        {
            string key = "BaseData";
            DataTable dt = new DataTable();
            dt = Cache[key] as DataTable;
            if (dt == null)
            {
                dt = Db.GetData("SELECT * FROM Base ORDER BY TabIndex");
                dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
                Cache.Insert(key, dt, null, DateTime.Now.AddHours(12), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return dt;
        }

        private string GetBaseNameRus(string name)
        {
            return GetBaseData().AsEnumerable().Where(row => row.Field<string>("Name") == name).Select(row => row.Field<string>("NameRus")).Single();
        }

        public string GetBaseHtmlList()
        {
            string key = "BaseHtmlList";
            string html = (Cache[key] ?? string.Empty).ToString();
            if (html == string.Empty)
            {
                html = Environment.NewLine;
                foreach (DataRow row in GetBaseData().Rows)
                {
                    html += "                           <li><a href=\"" + Page.GetRouteUrl("default", new { pBase = row["Name"] }) + "\">" + row["NameRus"] + "</a></li>" + Environment.NewLine;
                }
                Cache.Insert(key, html, null, DateTime.Now.AddHours(12), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return html;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["UserFormLogin"] = "a.fazilyanov";

            curBase = (Page.RouteData.Values["pBase"] ?? string.Empty).ToString();
            curBaseNameRus = curBase != string.Empty ? GetBaseNameRus(curBase) : curBase;
            curPage = (Page.RouteData.Values["pPage"] ?? string.Empty).ToString();

            // Если пользователь еще не авторизировался (любым из способов)
            if (Session["UserLogin"] == null)
            {
                string Login = string.Empty;
                // Если в Session["UserFormLogin"] есть логин, значит пользователь успешно
                // авторизовался через форму
                if ((Session["UserFormLogin"] ?? "").ToString() != "")
                {
                    Login = Session["UserFormLogin"].ToString();
                }
                else // Если нет, попробуем выполнить windows авторизацию
                {
                    Login = Context.User.Identity.Name.Trim().ToLower(); // Достаем windows логин
                    Login = Login.Substring(Login.LastIndexOf('\\') + 1); // Чистим
                }
                if (Login.Length > 0)
                {
                    DataTable dt = GetUserInfo(Login); // Ищем в базе

                    if (dt == null || dt.Rows.Count == 0) // если нет такого, просим залогиниться вручную
                    {
                        Response.Redirect("/Site/Logon.aspx");
                    }
                    else // Раз есть - считываем данные в сессию
                    {
                        Session["UserId"] = dt.Rows[0]["ID"];
                        Session["UserLogin"] = dt.Rows[0]["Login"];
                        Session["UserName"] = dt.Rows[0]["Name"];
                        //
                        Log.LogIt(1);
                    }
                }
                else
                {
                    Response.Redirect("/Site/Logon.aspx");
                }
            }
        }
    }
}