using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace WARP
{
    public class Log
    {
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
            string userId = (HttpContext.Current.Session["UserId"] ?? "0").ToString();
            Db.ExecuteNonQuery("INSERT INTO [dbo].[Log]([IdUser],[When],[IdLogType],[Prim]) VALUES ( " + userId + ", GetDate(), " + idLogType + "," + (prim ?? "''") + ")");
        }

        /// <summary>
        ///  Логирует SQL ошибки
        /// </summary>
        /// <param name="errorText">Сообщение исключения</param>
        /// <param name="sqlQuery">Запрос в котором произошла ошибка</param>
        public static void SqlError(Exception ex, string sqlQuery, SqlParameter[] sqlParameterArray)
        {
            string paramList = "";
            if (sqlParameterArray != null)
            {
                foreach (SqlParameter item in sqlParameterArray)
                {
                    paramList += item.ParameterName + " = " + item.Value.ToString() + Environment.NewLine;
                }
            }
            Error(ex, Environment.NewLine + Environment.NewLine + "SQL:" + Environment.NewLine + sqlQuery + Environment.NewLine + "Params:" + Environment.NewLine + paramList);
        }

        /// <summary>
        /// Добавляет запись в таблицу LogError
        /// </summary>
        /// <param name="errorText">Примечание</param>
        /// <remarks>Не используем готовые функции для запроса к БД - зациклитится,
        /// если проблемы с БД отправляем письмо, если и письмо отправить не получилось,
        /// тогда уж хз
        /// </remarks>
        public static void Error(Exception ex, string extra = "")
        {
            int ret = -1;
            SqlCommand sqlCommand;
            string errorText = "Ошибка:" + ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace + extra;
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

                // Последнюю ошибку сохраняем в сессии для вывода
                HttpContext.Current.Session["LastError"] = errorText;
            }
            catch (Exception ex2)
            {
                Func.SendMailAdmin(ex2.Message + Environment.NewLine + errorText, "Архив: Неудачная запись в лог ошибок");
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}