using System;
using System.Web;

namespace WARP
{
    public partial class Logon : System.Web.UI.Page
    {
        public string login = "";
        public string password = "";
        public string rememberme = "";
        public string error = "";

        protected void Page_PreRender(object sender, EventArgs e)
        {
            string CookieName = "LoginCookie";
            switch (Request.RequestType)
            {
                case "GET":
                    HttpCookie cookieReq = Request.Cookies[CookieName];

                    if (cookieReq != null)
                    {
                        login = cookieReq["login"] ?? "";
                        password = cookieReq["password"] ?? "";
                    }
                    break;

                case "POST":
                    login = (Request.Form["login"] ?? "").ToString().Trim().ToLower();
                    password = (Request.Form["password"] ?? "").ToString().Trim().ToLower();
                    rememberme = (Request.Form["rememberme"] ?? "").ToString();

                    if (login != "" && password != "" && ComFunc.ExecuteScalarInt("SELECT id FROM [dbo].[User] WHERE [Login]='" + login + "' AND [Password]='" + password + "'") > 0)
                    {
                        Session["UserFormLogin"] = login;
                        if (rememberme != "")
                        {
                            HttpCookie cookie = new HttpCookie(CookieName);
                            cookie["login"] = login;
                            cookie["password"] = password;
                            cookie.Expires = DateTime.Now.AddYears(1);
                            Response.Cookies.Add(cookie);
                            ComFunc.LogIt(2);
                        }
                        Response.Redirect("/");
                    }
                    else
                    {
                        error = "Ошибка авторизации";
                        ComFunc.LogIt(3, login + " / " + password);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}