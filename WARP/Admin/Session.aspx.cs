using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WARP
{
    public partial class Session : System.Web.UI.Page
    {
        /// <summary>
        /// Генерит HTML табличку со всеми значениями сессии
        /// </summary>
        private string GetSessionValues()
        {
            string _ret = "<style type=\"text/css\">table {border: thin solid black;border-collapse:collapse;} td,th{ border: thin solid black;padding:5px;}</style>";
            string _buf = "";

            string[] ignore_value = { };
            long totalSessionBytes = 0;

            _ret += "<b>Активных сессий:</b>  " + (Application["ActiveSession"] ?? 0).ToString() + " <br/><br/>UrlReferrer:";
            _ret += Request.UrlReferrer + "<br/>AbsoluteUri:" + Request.Url.AbsoluteUri + "<br/><br/>";
            _ret += "<br/><br/><table ><caption><b>Переменные сессии</b></caption><tr><th>Ключ</th><th>Значение</th><th>Размер</th></tr>";
            BinaryFormatter b = new BinaryFormatter();
            foreach (string item in Session.Contents)
            {
                var m = new MemoryStream();
                b.Serialize(m, Session[item]);
                totalSessionBytes += m.Length;
                _buf = "";
                //if (HttpContext.Current.Session[item].ToString() == "Table1")
                //{
                //    _buf = ConvertDataTableToHTML((DataTable)HttpContext.Current.Session[item]);
                //}
                //else if (HttpContext.Current.Session[item].ToString().IndexOf("href") > 0)
                //    _buf = "html";
                //else
                _buf = Session[item].ToString();
                _ret += "<tr><td>" + item + "</td><td>" + _buf + "</td><td>" + m.Length + " байт</td></tr>";
            }
            _ret += "<tr><th>Всего</th><td></td><th>" + (int)totalSessionBytes / 1024 + " КБайт</th></tr>";
            _ret += "</table><br/><br/>";

            return _ret;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Write(GetSessionValues());
            Response.End();
        }
        
    }
}