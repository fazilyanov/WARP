using System;
using System.Data;

/// scio me nihil scire
/// 19.06.2016
/// Artur Fazilyanov
namespace WARP
{
    public partial class Site : System.Web.UI.MasterPage
    {
        public string curBaseName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["UserFormLogin"] = "a.fazilyanov";

            // Сокращенное имя текущей базы, переданное в параметре адресной строки
            curBaseName = (Page.RouteData.Values["pBase"] ?? "").ToString();

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
                    DataTable dt = ComFunc.GetUserInfo(Login); // Ищем в базе

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
                        ComFunc.LogIt(1);
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