using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Caching;
using System.Web.UI;

namespace WARP
{
    /// <summary>
    /// Общие функции проекта
    /// </summary>
    public class ComFunc
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
                dt = GetData("SELECT * FROM Base ORDER BY TabIndex");
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
                dt = GetData("SELECT * FROM ArchivePage ORDER BY ID");
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
            return GetData("SELECT * FROM [dbo].[User] WHERE Del = 0 AND Login = @login", new SqlParameter("login", login));
        }

        #endregion GetInfo

        #region DataTables

        public static List<Dictionary<string, object>> GetFormatData(TableData tableData, DataTable dt)
        {
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            CultureInfo ruRu = CultureInfo.CreateSpecificCulture("ru-RU");
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();

                foreach (TableColumn column in tableData.ColumnList)
                {
                    switch (column.Type)
                    {
                        case TableColumnType.Integer:
                            row.Add(column.NameSql, Convert.ToInt32(dr[column.NameSql]));
                            break;

                        case TableColumnType.Money:
                            row.Add(column.NameSql, String.Format(ruRu, "{0:0,0.00}", Convert.ToDecimal(dr[column.NameSql])));
                            break;

                        case TableColumnType.DateTime:
                            row.Add(column.NameSql, ((DateTime)dr[column.NameSql]).ToString("dd.MM.yyyy HH:mm:ss"));
                            break;

                        case TableColumnType.Date:
                            row.Add(column.NameSql, ((DateTime)dr[column.NameSql]).ToString("dd.MM.yyyy"));
                            break;

                        case TableColumnType.String:
                        default:
                            row.Add(column.NameSql, dr[column.NameSql].ToString());
                            break;
                    }
                }
                data.Add(row);
            }

            return data;
        }

        #endregion DataTables

        #region SqlData

        /// <summary>
        /// Выполняет запрос
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <returns>Если вернул "-1" значит ошибка</returns>
        /// <overloads>Cтрока подключения и таймаут по умолчанию</overloads>
        public static int ExecuteNonQuery(string query)
        {
            return ExecuteNonQuery(query, Properties.Settings.Default.ConnectionString);
        }

        /// <summary>
        /// Выполняет запрос
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <returns>Если вернул "-1" значит ошибка</returns>
        /// <overloads>Cтрока подключения и таймаут по умолчанию</overloads>
        public static int ExecuteNonQuery(string query, SqlParameter[] sqlParameterArray)
        {
            return ExecuteNonQuery(query, Properties.Settings.Default.ConnectionString, sqlParameterArray);
        }

        /// <summary>
        /// Выполняет запрос
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <returns>Если вернул "-1" значит ошибка</returns>
        /// <overloads>Cтрока подключения и таймаут по умолчанию</overloads>
        public static int ExecuteNonQuery(string query, SqlParameter sqlParameter)
        {
            return ExecuteNonQuery(query, Properties.Settings.Default.ConnectionString, new SqlParameter[] { sqlParameter });
        }

        /// <summary>
        /// Выполняет запрос
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <param name="connString">Строка подключения</param>
        /// <param name="timeout">Таймаут (По умолчанию - 120)</param>
        /// <returns>Количество строк, "-1" - ошибка</returns>
        /// <remarks>аааааа</remarks>
        public static int ExecuteNonQuery(string query, string connString, SqlParameter[] sqlParameterArray = null, int timeout = 120)
        {
            int ret = -1;
            SqlCommand sqlCommand;
            SqlConnection sqlConnection = new SqlConnection(connString);
            sqlConnection.Open();
            sqlCommand = new SqlCommand(query, sqlConnection);
            if (sqlParameterArray != null) sqlCommand.Parameters.AddRange(sqlParameterArray);
            sqlCommand.CommandTimeout = timeout;
            try
            {
                ret = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogSqlError(ex.Message.Trim(), sqlCommand.CommandText, sqlParameterArray);
            }
            finally
            {
                sqlConnection.Close();
            }
            return ret;
        }

        /// <summary>
        /// Выполняет запрос
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <param name="connString">Строка подключения</param>
        /// <param name="timeout">Таймаут (По умолчанию - 120)</param>
        /// <returns>Количество строк, "-1" - ошибка</returns>
        /// <remarks>аааааа</remarks>
        public static int ExecuteNonQueryTransaction(string query, SqlParameter[] sqlParameterArray = null, int timeout = 120)
        {
            int ret = -1;
            SqlCommand sqlCommand;
            SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            sqlConnection.Open();

            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            sqlCommand = new SqlCommand(query, sqlConnection, sqlTransaction);
            try
            {
                if (sqlParameterArray != null) sqlCommand.Parameters.AddRange(sqlParameterArray);
                sqlCommand.CommandTimeout = timeout;
                ret = sqlCommand.ExecuteNonQuery();
                sqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                LogSqlError(ex.Message.Trim(), sqlCommand.CommandText, sqlParameterArray);
                
            }
            finally
            {
                sqlConnection.Close();
            }
            return ret;
        }

        /// <summary>
        /// Возвращает таблицу - результат запроса, строка подключения и таймаут по умолчанию
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <returns>DataTable.</returns>
        public static DataTable GetData(string query)
        {
            return GetData(query, Properties.Settings.Default.ConnectionString, null);
        }

        /// <summary>
        /// Возвращает таблицу - результат запроса, строка подключения и таймаут по умолчанию
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <returns>DataTable.</returns>
        public static DataTable GetData(string query, SqlParameter sqlParameter)
        {
            return GetData(query, Properties.Settings.Default.ConnectionString, new SqlParameter[] { sqlParameter });
        }

        /// <summary>
        /// Возвращает таблицу - результат запроса, строка подключения и таймаут по умолчанию
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <returns>DataTable.</returns>
        public static DataTable GetData(string query, SqlParameter[] sqlParameterArray)
        {
            return GetData(query, Properties.Settings.Default.ConnectionString, sqlParameterArray);
        }

        /// <summary>
        /// Возвращает таблицу - результат запроса
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <param name="connString">Строка подключения</param>
        /// <param name="timeout">Таймаут (По умолчанию - 120)</param>
        /// <returns>DataTable.</returns>
        public static DataTable GetData(string query, string connString, SqlParameter[] sqlParameterArray, int timeout = 120)
        {
            SqlCommand sqlCommand;
            SqlConnection sqlConnection = new SqlConnection(connString);
            sqlConnection.Open();
            sqlCommand = new SqlCommand(query, sqlConnection);
            if (sqlParameterArray != null) sqlCommand.Parameters.AddRange(sqlParameterArray);
            sqlCommand.CommandTimeout = timeout;
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                dt = null;
                LogSqlError(ex.Message.Trim(), sqlCommand.CommandText, sqlParameterArray);
            }
            finally
            {
                sqlConnection.Close();
            }
            return dt;
        }

        /// <summary>
        /// Выполняет скалярный запрос
        /// </summary>
        /// <param name="query">SQL запрос</param>
        public static object ExecuteScalar(string query)
        {
            return ExecuteScalar(query, Properties.Settings.Default.ConnectionString);
        }

        /// <summary>
        /// Выполняет скалярный запрос
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <param name="connString">Строка подключения</param>
        /// <param name="timeout">Таймаут (По умолчанию - 120)</param>
        /// <returns>Количество строк, "-1" - ошибка</returns>
        /// <remarks>аааааа</remarks>
        public static object ExecuteScalar(string query, string connString, int timeout = 120)
        {
            SqlCommand sqlCommand;
            SqlConnection sqlConnection = new SqlConnection(connString);
            object ret = null;
            sqlConnection.Open();
            sqlCommand = new SqlCommand(query, sqlConnection);
            sqlCommand.CommandTimeout = timeout;
            try
            {
                ret = sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                LogSqlError(ex.Message.Trim(), sqlCommand.CommandText, null);
            }
            finally
            {
                sqlConnection.Close();
            }
            return ret;
        }

        /// <summary>
        /// Выполняет скалярный запрос
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <returns>Если вернул "-1" значит ошибка</returns>
        /// <overloads>Cтрока подключения и таймаут по умолчанию</overloads>
        public static int ExecuteScalarInt(string query)
        {
            return ExecuteScalarInt(query, Properties.Settings.Default.ConnectionString);
        }

        /// <summary>
        /// Выполняет скалярный запрос
        /// </summary>
        /// <param name="query">SQL запрос</param>
        /// <param name="connString">Строка подключения</param>
        /// <param name="timeout">Таймаут (По умолчанию - 120)</param>
        /// <returns>Количество строк, "-1" - ошибка</returns>
        /// <remarks>аааааа</remarks>
        public static int ExecuteScalarInt(string query, string connString, int timeout = 120)
        {
            int ret = -1;
            int resInt = 0;
            var res = ComFunc.ExecuteScalar(query, connString);
            if (int.TryParse((res is DBNull || res == null ? "-1" : res.ToString()), out resInt))
                ret = resInt;
            return ret;
        }

        #endregion SqlData

        #region Log

        /// <summary>
        /// Добавляет запись в таблицу Log
        /// </summary>
        /// <param name="idLogType">
        /// 1-Авторизация Windows,
        /// 2-Авторизация Form,
        /// 3-Ошибка авторизации Form
        /// <param name="prim">Примечание</param>
        public static void LogIt(int idLogType, string prim = null)
        {
            int userId = int.Parse((HttpContext.Current.Session["UserId"] ?? "0").ToString());
            prim = prim == null ? "NULL" : "'" + prim + "'";
            ExecuteNonQuery("INSERT INTO [dbo].[Log]([IdUser],[When],[IdLogType],[Prim]) VALUES ( " + userId + ", GetDate(), " + idLogType + "," + prim + ")");
        }

        /// <summary>
        ///  Логирует SQL ошибки
        /// </summary>
        /// <param name="errorText">Сообщение исключения</param>
        /// <param name="sqlQuery">Запрос в котором произошла ошибка</param>
        public static void LogSqlError(string errorText, string sqlQuery, SqlParameter[] sqlParameterArray)
        {
            string paramList = "";
            if (sqlParameterArray != null)
            {
                foreach (SqlParameter item in sqlParameterArray)
                {
                    paramList += item.ParameterName + " = " + item.Value.ToString() + Environment.NewLine;
                }
            }
            LogError(
                "Ошибка:" + Environment.NewLine + errorText + Environment.NewLine + Environment.NewLine +
                "SQL:" + Environment.NewLine + sqlQuery + Environment.NewLine + "Params:" + Environment.NewLine + paramList);
        }

        /// <summary>
        /// Добавляет запись в таблицу LogError
        /// </summary>
        /// <param name="errorText">Примечание</param>
        /// <remarks>Не используем готовые функции для запроса к БД - зациклитится,
        /// если проблемы с БД отправляем письмо, если и письмо отправить не получилось,
        /// тогда уж хз
        /// </remarks>
        public static void LogError(string errorText)
        {
            int ret = -1;
            SqlCommand sqlCommand;
            SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            try
            {
                sqlConnection.Open();
                sqlCommand = new SqlCommand("INSERT INTO [dbo].[LogError]([IdUser],[When],[ErrorText]) VALUES ( @IdUser, GetDate(), @ErrorText);", sqlConnection);
                int userId = int.Parse((HttpContext.Current.Session["UserId"] ?? "0").ToString());
                SqlParameter[] sqlParameterArray = {
                    new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = userId },
                    new SqlParameter { ParameterName = "@ErrorText", SqlDbType = SqlDbType.NVarChar, Value = errorText }
                };
                sqlCommand.Parameters.AddRange(sqlParameterArray);
                ret = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SendMailAdmin(ex.Message + Environment.NewLine + errorText, "Архив: Неудачная запись в лог ошибок");
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        #endregion Log

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
    }
}