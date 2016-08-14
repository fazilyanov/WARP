using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WARP
{
    public class Db
    {
       

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
                Log.SqlError(ex, sqlCommand.CommandText, sqlParameterArray);
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
                Log.SqlError(ex, sqlCommand.CommandText, sqlParameterArray);
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
                Log.SqlError(ex, sqlCommand.CommandText, sqlParameterArray);
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
                Log.SqlError(ex, sqlCommand.CommandText, null);
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
            var res = ExecuteScalar(query, connString);
            if (int.TryParse((res is DBNull || res == null ? "-1" : res.ToString()), out resInt))
                ret = resInt;
            return ret;
        }
        
    }
}