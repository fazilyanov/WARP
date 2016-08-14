using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI;

namespace WARP
{
   
    // Общие функции проекта
    public class Func
    {
        #region GetInfo

        /// <summary>
        /// Генерит HTML табличку со всеми значениями сессии
        /// </summary>
        public static string GetSessionValues()
        {
            string _ret = "<style type=\"text/css\">table {border: thin solid black;border-collapse:collapse;} td,th{ border: thin solid black;padding:5px;}</style>";
            string _buf = "";

            string[] ignore_value = { };
            long totalSessionBytes = 0;

            _ret += "<b>Активных сессий:</b>  " + (HttpContext.Current.Application["ActiveSession"] ?? 0).ToString() + " <br/><br/>UrlReferrer:";
            _ret += HttpContext.Current.Request.UrlReferrer + "<br/>AbsoluteUri:" + HttpContext.Current.Request.Url.AbsoluteUri + "<br/><br/>";
            _ret += "<br/><br/><table ><caption><b>Переменные сессии</b></caption><tr><th>Ключ</th><th>Значение</th><th>Размер</th></tr>";
            BinaryFormatter b = new BinaryFormatter();
            foreach (string item in HttpContext.Current.Session.Contents)
            {
                var m = new MemoryStream();
                b.Serialize(m, HttpContext.Current.Session[item]);
                totalSessionBytes += m.Length;
                _buf = "";
                //if (HttpContext.Current.Session[item].ToString() == "Table1")
                //{
                //    _buf = ConvertDataTableToHTML((DataTable)HttpContext.Current.Session[item]);
                //}
                //else if (HttpContext.Current.Session[item].ToString().IndexOf("href") > 0)
                //    _buf = "html";
                //else
                _buf = HttpContext.Current.Session[item].ToString();
                _ret += "<tr><td>" + item + "</td><td>" + _buf + "</td><td>" + m.Length + " байт</td></tr>";
            }
            _ret += "<tr><th>Всего</th><td></td><th>" + (int)totalSessionBytes / 1024 + " КБайт</th></tr>";
            _ret += "</table><br/><br/>";

            return _ret;
        }

        #region Base

        /// <summary>
        /// Возвращает список всех баз
        /// </summary>
        /// <returns>DataTable</returns>
        public static DataTable GetBaseData()
        {
            string key = "BaseData";
            DataTable dt = new DataTable();
            dt = HttpContext.Current.Cache[key] as DataTable;
            if (dt == null)
            {
                dt = Db.GetData("SELECT * FROM Base ORDER BY TabIndex");
                dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
                HttpContext.Current.Cache.Insert(key, dt, null, DateTime.Now.AddHours(12), Cache.NoSlidingExpiration);
            }
            return dt;
        }

        public static string GetBaseHtmlList()
        {
            string key = "BaseHtmlList";
            string html = (HttpContext.Current.Cache[key] ?? string.Empty).ToString();
            if (html == string.Empty)
            {
                html = Environment.NewLine;
                foreach (DataRow row in GetBaseData().Rows)
                {
                    html += "                           <li><a href=\"" + (HttpContext.Current.Handler as Page).GetRouteUrl("default", new { pBase = row["Name"] }) + "\">" + row["NameRus"] + "</a></li>" + Environment.NewLine;
                }
                HttpContext.Current.Cache.Insert(key, html, null, DateTime.Now.AddHours(12), Cache.NoSlidingExpiration);
            }
            return html;
        }

        public static string GetBaseNameRus(string name)
        {
            return GetBaseData().AsEnumerable().Where(row => row.Field<string>("Name") == name).Select(row => row.Field<string>("NameRus")).Single();
        }

        public static string GetBaseNameRus(int id)
        {
            return GetBaseData().AsEnumerable().Where(row => row.Field<int>("ID") == id).Select(row => row.Field<string>("NameRus")).Single();
        }

        #endregion Base

        #region ArchivePage

        public static DataTable GetArchivePageData()
        {
            string key = "ArchivePageData";
            DataTable dt = new DataTable();
            dt = HttpContext.Current.Cache[key] as DataTable;
            if (dt == null)
            {
                dt = Db.GetData("SELECT * FROM ArchivePage ORDER BY ID");
                dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
                HttpContext.Current.Cache.Insert(key, dt, null, DateTime.Now.AddHours(12), Cache.NoSlidingExpiration);
            }
            return dt;
        }

        public static string GetArchivePageHtmlList(string curBaseName)
        {
            string key = "ArchivePageHtmlList" + curBaseName;
            string html = (HttpContext.Current.Cache[key] ?? string.Empty).ToString();
            if (html == string.Empty)
            {
                html = Environment.NewLine;
                foreach (DataRow row in GetArchivePageData().Rows)
                {
                    html += "                           <li><a href=\"" + (HttpContext.Current.Handler as Page).GetRouteUrl("archive", new { pBase = curBaseName, pPage = row["Name"] }) + "\">" + row["NameRus"] + "</a></li>" + Environment.NewLine;
                }
                HttpContext.Current.Cache.Insert(key, html, null, DateTime.Now.AddHours(12), Cache.NoSlidingExpiration);
            }
            return html;
        }

        public static string GetArchivePageNameRus(string name)
        {
            return name != string.Empty ? GetArchivePageData().AsEnumerable().Where(row => row.Field<string>("Name") == name).Select(row => row.Field<string>("NameRus")).Single() : string.Empty;
        }

        #endregion ArchivePage

        /// <summary>
        /// Ищет в базе информацию по пользователю
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>DataTable</returns>
        public static DataTable GetUserInfo(string login)
        {
            return Db.GetData("SELECT * FROM [dbo].[User] WHERE Del = 0 AND Login = @login", new SqlParameter("login", login));
        }

        #endregion GetInfo

       

        #region SendMail

        /// <summary>
        /// Отправляет письмо
        /// </summary>
        /// <param name="mm">Сообщение</param>
        public static void SendMail(System.Net.Mail.MailMessage mm)
        {
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(Properties.Settings.Default.MailServerHost, Properties.Settings.Default.MailServerPort);
            client.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.ApplicationMailLogin, Properties.Settings.Default.ApplicationMailPassword);
            try
            {
                client.Send(mm);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Отправляет письмо Админу
        /// </summary>
        /// <param name="subject">Тема</param>
        /// <param name="msg">Сообщение</param>
        public static void SendMailAdmin(string msg, string subject = "Архив")
        {
            System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage();
            mm.From = new System.Net.Mail.MailAddress(Properties.Settings.Default.ApplicationMail);
            mm.To.Add(new System.Net.Mail.MailAddress(Properties.Settings.Default.AdminMail));
            mm.Subject = subject;
            mm.IsBodyHtml = true;
            mm.Body = msg;
            SendMail(mm);
        }

        #endregion SendMail

        //String to enum
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        // Генерит hash строки
        public static string GetMd5Hash(string input)
        {
            StringBuilder sBuilder = new StringBuilder();
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}