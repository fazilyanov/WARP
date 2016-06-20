using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Caching;

namespace WARP
{
    /// <summary>
    /// Общие функции проекта
    /// </summary>
    public class ComFunc
    {
        #region GetInfo

        /// <summary>
        /// Ищет в базе информацию по пользователю
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>DataTable</returns>
        public static DataTable GetUserInfo(string login)
        {
            return GetData("SELECT * FROM [dbo].[User] WHERE Del = 0 AND Login = @login", new SqlParameter("login", login));
        }

        /// <summary>
        /// Возвращает список всех баз
        /// </summary>
        /// <returns>DataTable</returns>
        public static DataTable GetBaseList()
        {
            string key = "BaseList";
            DataTable dt = new DataTable();
            dt = HttpContext.Current.Cache[key] as DataTable;
            if (dt == null)
            {
                dt = GetData("SELECT * FROM Base WHERE Del=0 ORDER BY TabIndex");
                dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };
                HttpContext.Current.Cache.Insert(key, dt, null, DateTime.Now.AddHours(2), Cache.NoSlidingExpiration);
            }
            return dt;
        }

        /// <summary>
        /// Возвращает элемент перечисления ArchivePage по имени
        /// </summary>
        /// <param name="name">Имя</param>
        public static ArchivePage GetArchivePageByName(string name)
        {
            return (ArchivePage)System.Enum.Parse(typeof(ArchivePage), name);
        }

        /// <summary>
        /// Возвращает заголовок страницы, по сокращенному названию
        /// </summary>
        /// <param name="page">Сокращенное название</param>
        public static string GetArchivePageNameRus(ArchivePage archivePage)
        {
            string ret = "";

            switch (archivePage)
            {
                case ArchivePage.Acc:
                    ret = "Бухгалтерские документы";
                    break;

                case ArchivePage.Dog:
                    ret = "Договоры";
                    break;

                case ArchivePage.Ord:
                    ret = "ОРД";
                    break;

                case ArchivePage.Oth:
                    ret = "Прочие документы";
                    break;

                case ArchivePage.Empl:
                    ret = "Документы по личному составу";
                    break;

                case ArchivePage.Ohs:
                    ret = "Документы по охране труда";
                    break;

                case ArchivePage.Tech:
                    ret = "Техническая документация";
                    break;

                case ArchivePage.All:
                    ret = "Поиск документов";
                    break;

                case ArchivePage.Select:
                    ret = "Выбор документа";
                    break;

                case ArchivePage.Bank:
                    ret = "Банковские документы";
                    break;

                case ArchivePage.Norm:
                    ret = "Локальные нормативные документы";
                    break;

                default:
                    break;
            }
            return ret;
        }

        #endregion GetInfo

        #region DataTables

        public static string GenerateHtmlTableColumns(TableData tableData)
        {
            string ret = Environment.NewLine;
            foreach (TableColumn item in tableData.ColumnList)
            {
                ret += "                        <th>" + item.Name + "</th>" + Environment.NewLine;
            }
            return ret;
        }

        public static string GenerateJSTableColumns(TableData tableData)
        {
            string ret = Environment.NewLine;
            foreach (TableColumn item in tableData.ColumnList)
            {
                ret += "                    { \"data\": \"" + item.NameSql + "\", className:\"dt-body-" + item.Align.ToString().ToLower() + "\", \"width\": \"" + item.Width + "px\" }," + Environment.NewLine;
            }
            return ret;
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
        ///
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