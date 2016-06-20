using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace WARP
{
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
            return GetData("SELECT * FROM User WHERE Del = 0 AND Login = @login", new SqlParameter("login", login));
        }

        public static DataTable GetBaseList()
        {
            return GetData("SELECT * FROM Base WHERE Del=0 ORDER BY TabIndex");
        }

        //    string _key = "data_tree__doctree";
        //    DataTable buf = new DataTable();
        //    buf = HttpContext.Current.Cache[_key] as DataTable;
        //        if (buf == null)
        //        {
        //            buf = new DataTable();
        //    SqlConnection conn = new SqlConnection(Properties.Settings.Default.constr);
        //    conn.Open();
        //            string sql = "SELECT a.* FROM [_doctree_pre] a ORDER BY a.pos";
        //    SqlCommand cmd = new SqlCommand(sql, conn);
        //    cmd.CommandTimeout = 600;
        //            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
        //    sqlDataAdapter.Fill(buf);
        //            conn.Close();
        //            buf.PrimaryKey = new DataColumn[] { buf.Columns["id"]
        //};
        //HttpContext.Current.Cache.Insert(_key, buf, null, DateTime.Now.AddHours(2), Cache.NoSlidingExpiration);
        //        }

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
        /// <param name="connstring">Строка подключения</param>
        /// <param name="timeout">Таймаут (По умолчанию - 120)</param>
        /// <returns>Количество строк, "-1" - ошибка</returns>
        /// <remarks>аааааа</remarks>
        public static int ExecuteNonQuery(string query, string connstring, SqlParameter[] sqlParameterArray = null, int timeout = 120)
        {
            int ret = -1;
            SqlCommand cmd;
            SqlConnection conn = new SqlConnection(connstring);
            conn.Open();
            cmd = new SqlCommand(query, conn);
            if (sqlParameterArray != null) cmd.Parameters.AddRange(sqlParameterArray);
            cmd.CommandTimeout = timeout;
            try
            {
                ret = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogError(ex.Message.Trim());
            }
            finally
            {
                conn.Close();
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
        /// <param name="connstring">Строка подключения</param>
        /// <param name="timeout">Таймаут (По умолчанию - 120)</param>
        /// <returns>DataTable.</returns>
        public static DataTable GetData(string query, string connstring, SqlParameter[] sqlParameterArray, int timeout = 120)
        {
            SqlCommand cmd;
            SqlConnection conn = new SqlConnection(connstring);
            conn.Open();
            cmd = new SqlCommand(query, conn);
            if (sqlParameterArray != null) cmd.Parameters.AddRange(sqlParameterArray);
            cmd.CommandTimeout = timeout;
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                sqlDataAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                dt = null;
                LogError(ex.Message.Trim());
            }
            finally
            {
                conn.Close();
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
        /// <param name="connstring">Строка подключения</param>
        /// <param name="timeout">Таймаут (По умолчанию - 120)</param>
        /// <returns>Количество строк, "-1" - ошибка</returns>
        /// <remarks>аааааа</remarks>
        public static object ExecuteScalar(string query, string connstring, int timeout = 120)
        {
            SqlCommand cmd;
            SqlConnection conn = new SqlConnection(connstring);
            object ret = null;
            conn.Open();
            cmd = new SqlCommand(query, conn);
            cmd.CommandTimeout = timeout;
            try
            {
                ret = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                LogError(ex.Message.Trim());
            }
            finally
            {
                conn.Close();
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
        /// <param name="connstring">Строка подключения</param>
        /// <param name="timeout">Таймаут (По умолчанию - 120)</param>
        /// <returns>Количество строк, "-1" - ошибка</returns>
        /// <remarks>аааааа</remarks>
        public static int ExecuteScalarInt(string query, string connstring, int timeout = 120)
        {
            int ret = -1;
            int resInt = 0;
            var res = ComFunc.ExecuteScalar(query, connstring);
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
            ExecuteNonQuery("INSERT INTO [dbo].[Log]([IdUser],[When],[IdLogType],[What]) VALUES ( " + userId + ", GetDate(), " + idLogType + "," + prim + ")");
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
            SqlCommand cmd;
            SqlConnection conn = new SqlConnection(Properties.Settings.Default.ConnectionString);
            try
            {
                conn.Open();
                cmd = new SqlCommand("INSERT INTO [dbo].[Log]([IdUser],[When],[ErrorText]) VALUES ( @IdUser, GetDate(), @ErrorText);", conn);
                int userId = int.Parse((HttpContext.Current.Session["UserId"] ?? "0").ToString());
                SqlParameter[] sqlParameterArray = {
                new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = userId },
                new SqlParameter { ParameterName = "@ErrorText", SqlDbType = SqlDbType.NVarChar, Value = errorText }
            };
                cmd.Parameters.AddRange(sqlParameterArray);

                ret = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SendMailAdmin(ex.Message, "Архив: Неудачная запись в лог ошибок");
            }
            finally
            {
                conn.Close();
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