using System;
using System.Web;

namespace ArchNet
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
                    login = (Request.Form["login"] ?? "").ToString().Trim();
                    password = (Request.Form["password"] ?? "").ToString().Trim();
                    rememberme = (Request.Form["rememberme"] ?? "").ToString();

                    if (login != "" && password != "" && faFunc.ExecuteScalarInt("SELECT id FROM [dbo].[_user] where [login]='" + login + "' AND [password]='" + password + "'") > 0)
                    {
                        Session["UserFormLogin"] = login;
                        if (rememberme != "")
                        {
                            HttpCookie cookie = new HttpCookie(CookieName);
                            cookie["login"] = login;
                            cookie["password"] = password;
                            cookie.Expires = DateTime.Now.AddYears(1);
                            Response.Cookies.Add(cookie);

                        }
                        Response.Redirect("/");
                    }
                    else
                    {
                        error = "Ошибка";
                    }
                    break;

                default:
                    break;
            }
        }
    }
}