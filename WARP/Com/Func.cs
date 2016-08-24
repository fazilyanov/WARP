using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace WARP
{
    // Общие функции проекта
    public class Func
    {
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

        public static string GetArchivePageNameRus(string name)
        {
            return name != string.Empty ? GetArchivePageData().AsEnumerable().Where(row => row.Field<string>("Name") == name).Select(row => row.Field<string>("NameRus")).Single() : string.Empty;
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

        // Генерит key для id файла
        public static string GetFileKey(string idFile)
        {
            return GetMd5Hash(GetMd5Hash(idFile) + idFile);
        }

        //String to enum
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

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
    }
}