using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace StackRadius.DBHelper
{
    public sealed class DbHelper
    {
        private DbHelper(IConfiguration configuration)
        {
        }
        public sealed class ConnectionStrings
        {
            public string DBConn { get; set; }
        }

        static DbHelper()
        {

            // Build a config object, using env vars and JSON providers.
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Get values from the config given their key and their target type.
            ConnectionStrings conStr = config.GetSection("ConnectionStrings").Get<ConnectionStrings>();

            if (conStr.DBConn != null)
            {
                // string conString = System.Configuration.ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString.ToString();
                // string providerString = System.Configuration.ConfigurationManager.ConnectionStrings["DbConn"].ProviderName.ToString();
                string conString = conStr.DBConn;
                string providerString = "Npgsql";
                if (!string.IsNullOrEmpty(conString) && !string.IsNullOrEmpty(providerString))
                {
                    SetConnectionString(conString);
                    SetProviderString(providerString);
                }
            }
        }

        internal const string PostgreSQLDatabase = "PostgreSQL";
        internal const string MySQLDatabase = "MySQL";
        internal const string SQLDatabase = "SQL";
        internal const string PostgreSQLProvider = "Npgsql";
        internal const string MySQLProvider = "MySql.Data.MySqlClient";
        internal const string SQLProvider = "System.Data.SqlClient";
        internal const string ODBCProvider = "System.Data.Odbc";

        //Shared db As IDB = GetDbInstance()

        private static IDB db;
        static internal IDB GetDbInstance()
        {
            // get these two from config file or somewhere
            var connectionString = GetConnectionString();
            var providerName = GetProviderString();

            // logic to decide which db is being used
            var driver = GetDbType(providerName);

#if DEBUG1
            BaDbHelperLog.AddDebugFormat("BaDbHelper Provider: {0} ", providerName);
            BaDbHelperLog.AddDebugFormat("BaDbHelper DBType: {0} ", driver);
#endif

            if (providerName == PostgreSQLProvider)
            {
                Db<PostgreSqlClient> dbPostgreSql = new Db<PostgreSqlClient>(connectionString, providerName);
                return dbPostgreSql;
                //return null;
            }
            else if (providerName == MySQLProvider)
            {
                //return new Db<MySqlClient>(connectionString, providerName);
                return null;
            }
            else if (driver == MySQLDatabase && providerName == ODBCProvider)
            {
                //return new Db<MySqlOdbc>(connectionString, providerName);
                return null;
            }
            else if (driver == SQLDatabase && providerName == SQLProvider)
            {
                //return new Db<Microsoft.Data.SqlClient>(connectionString, providerName);
                return null;
            }
            else
            {
                return null;
            }
        }

        private static void SetDB()
        {
            db = GetDbInstance();
        }

        /// <summary>
        /// This method helps to retrieve database type such as MySQL, SQL or any other.
        /// </summary>
        /// <returns>DbType String</returns>
        static internal string GetDbType(string provideName)
        {
            string dbType = null;

            string conString = GetConnectionString();

            if (provideName == PostgreSQLProvider)
            {
                dbType = PostgreSQLDatabase;
            }
            else if (provideName == SQLProvider)
            {
                dbType = SQLDatabase;
            }
            else if (provideName == ODBCProvider && conString.Contains(MySQLDatabase))
            {
                dbType = MySQLDatabase;
            }
            else if (provideName == ODBCProvider && conString.Contains(SQLDatabase))
            {
                dbType = SQLDatabase;
            }

            return dbType;
        }

        /// <summary>
        /// Helps to get active connection string
        /// </summary>
        /// <returns></returns>
        private static string GetConnectionString()
        {
            return _connectionString;
        }
        private static string _connectionString;
        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            if (!string.IsNullOrEmpty(_connectionString) && !string.IsNullOrEmpty(_providerString))
            {
                SetDB();
            }
        }

        /// <summary>
        /// Helps to get database provider based on configured connection
        /// </summary>
        /// <returns></returns>
        private static string GetProviderString()
        {
            return _providerString;
        }

        private static string _providerString;
        public static void SetProviderString(string provider)
        {
            _providerString = provider;
            if (!string.IsNullOrEmpty(_connectionString) && !string.IsNullOrEmpty(_providerString))
            {
                SetDB();
            }
        }

        /// <summary>
        /// Execute DataSet
        /// </summary>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>System.Data.DataSet</returns>
        public static DataSet ExecuteDataSet(string query, CommandType cmdType, List<DbParameter> parameterlist = null)
        {
            return db.ExecuteDataSet(query, cmdType, parameterlist);
        }

        /// <summary>
        /// Execute DataSet using shared connection
        /// </summary>
        /// <param name="baDBConnection">IBaDbConnection object</param>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>System.Data.DataSet</returns>
        public static DataSet ExecuteDataSet(IDbConn baDBConnection, string query, CommandType cmdType, List<DbParameter> parameterlist = null)
        {
            return db.ExecuteDataSet(baDBConnection, query, cmdType, parameterlist);
        }


        public static DataTable ExecuteDataTable()
        {
            // 1. Initialize your DataTable
            DataTable dt = new DataTable();

            // 2. Add columns to the DataTable first
            dt.Columns.Add("ClientId", typeof(int));
            dt.Columns.Add("amcCode", typeof(string));

            // 3. Create a new DataRow based on the updated DataTable schema
            DataRow newRow = dt.NewRow();

            // 4. Assign data values to the specific row columns
            newRow["ClientId"] = 18888;
            newRow["amcCode"] = "J";

            // 5. Add the populated DataRow into the DataTable
            dt.Rows.Add(newRow);

            return dt;
            //return db.ExecuteDataTable();
        }

        /// <summary>
        /// Execute DataTable
        /// </summary>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>System.Data.DataTable</returns>
        public static DataTable ExecuteDataTable(string query, CommandType cmdType, List<DbParameter> parameterlist = null)
        {
            return db.ExecuteDataTable(query, cmdType, parameterlist);
        }

        /// <summary>
        /// Execute DataTable using shared connection
        /// </summary>
        /// <param name="baDBConnection">IBaDbConnection object</param>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>System.Data.DataTable</returns>
        public static DataTable ExecuteDataTable(IDbConn baDBConnection, string query, CommandType cmdType, List<DbParameter> parameterlist = null)
        {
            return db.ExecuteDataTable(baDBConnection, query, cmdType, parameterlist);
        }
        /// <summary>
        /// Execute DataTable using shared connection
        /// </summary>
        /// <param name="baDBConnection">IBaDbConnection object</param>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>System.Data.DataTable</returns>
        public static DataTable ExecuteDataTable(IDbConn baDBConnection, string query, List<DbParameter> parameterlist = null, CommandType cmdType = CommandType.Text)
        {
            return db.ExecuteDataTable(baDBConnection, query, cmdType, parameterlist);
        }

        /// <summary>
        /// Execute NonQuery
        /// </summary>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>integer type value for affected rows</returns>
        public static int ExecuteNonQuery(string query, CommandType cmdType, List<DbParameter> parameterlist = null)
        {
            return db.ExecuteNonQuery(query, cmdType, parameterlist);
        }
        /// <summary>
        /// Execute NonQuery
        /// </summary>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>integer type value for affected rows</returns>
        public static int ExecuteNonQuery(IDbConn baDBConnection, string query, List<DbParameter> parameterlist = null, CommandType cmdType = CommandType.Text)
        {
            return db.ExecuteNonQuery(baDBConnection, query, cmdType, parameterlist);
        }

        /// <summary>
        /// Execute NonQuery using shared connection
        /// </summary>
        /// <param name="baDBConnection">IBaDbConnection object</param>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>integer type value for affected rows</returns>
        public static int ExecuteNonQuery(IDbConn baDBConnection, string query, CommandType cmdType, List<DbParameter> parameterlist = null)
        {
            return db.ExecuteNonQuery(baDBConnection, query, cmdType, parameterlist);
        }

        /// <summary>
        /// Execute Scalar
        /// </summary>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>System.Object</returns>
        public static object ExecuteScalar(string query, CommandType cmdType, List<DbParameter> parameterlist = null)
        {
            return db.ExecuteScalar(query, cmdType, parameterlist);
        }

        /// <summary>
        /// Execute Scalar using shared connection
        /// </summary>
        /// <param name="baDBConnection">IBaDbConnection object</param>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>System.Object</returns>
        public static object ExecuteScalar(IDbConn baDBConnection, string query, List<DbParameter> parameterlist = null, CommandType cmdtype = CommandType.Text)
        {
            return db.ExecuteScalar(baDBConnection, query, cmdtype, parameterlist);
        }
        /// <summary>
        /// Execute Scalar using shared connection
        /// </summary>
        /// <param name="baDBConnection">IBaDbConnection object</param>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>System.Object</returns>
        public static object ExecuteScalar(IDbConn baDBConnection, string query, CommandType cmdType, List<DbParameter> parameterlist = null)
        {
            return db.ExecuteScalar(baDBConnection, query, cmdType, parameterlist);
        }

        /// <summary>
        /// Execute Reader
        /// </summary>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>System.Data.IDataReader</returns>
        [Obsolete("This method should be used only with active / shared connection.")]
        private static IDataReader ExecuteReader(string query, CommandType cmdType, List<DbParameter> parameterlist = null)
        {
            return db.ExecuteReader(query, cmdType, parameterlist);
        }

        /// <summary>
        /// Execute Reader using shared connection
        /// </summary>
        /// <param name="baDBConnection">IBaDbConnection object</param>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>System.Data.IDataReader</returns>
        public static IDataReader ExecuteReader(IDbConn baDBConnection, string query, CommandType cmdType, List<DbParameter> parameterlist = null)
        {
            return db.ExecuteReader(baDBConnection, query, cmdType, parameterlist);
        }

        /// <summary>
        /// Execute Reader using shared connection
        /// </summary>
        /// <param name="baDBConnection">IBaDbConnection object</param>
        /// <param name="query">SQL query as a string</param>
        /// <param name="cmdType">System.Data.CommandType parameter</param>
        /// <param name="parameterlist">Collection of BaDbparameter</param>
        /// <returns>System.Data.IDataReader</returns>
        public static IDataReader ExecuteReader(IDbConn baDBConnection, string query, List<DbParameter> parameterlist = null, CommandType cmdType = CommandType.Text)
        {
            return db.ExecuteReader(baDBConnection, query, cmdType, parameterlist);
        }

        /// <summary>
        /// Returns IBaDbConnection object based on active connection string.
        /// </summary>
        /// <returns>Returns IDbConnection object</returns>
        public static IDbConn GetDBConnection()
        {
            return db.GetDBConnection();
        }

        /// <summary>
        /// Returns IDbCommand object based on IDbConnection
        /// </summary>
        /// <returns>Returns IDbCommand object</returns>
        public static IDbCommand GetDBCommand()
        {
            return db.GetDBCommand();
        }

        /// <summary>
        /// It will parse the comma seperated values in individual parameters
        /// It appends individual parameters into the passed parameter list
        /// Finally, it returns parameter value string that need to replace with actual parameter by formating sql command text.
        /// It is not applicable to stored procedure
        /// </summary>
        /// <param name="parametersValue">Existing parameter value string which contains comma or pipe </param>
        /// <param name="parameterlist">BaDbParameter list which is used in command</param>
        /// <returns>Updates BaDbParameter list collection based on parameter values found in passed parameter string and returns string with parameterValues</returns>
        /// <remarks></remarks>
        public static string ParseParametersForInClause(string parametersValue, List<DbParameter> parameterlist)
        {

            string commaValues = string.Empty;
            int I = 1;

            if ((parametersValue != null))
            {
                string[] strArray = parametersValue.Split(',');

                foreach (string objStr in strArray)
                {
                    //IN clause
                    string parameterName = String.Format("{0}{1}{2}", "@", I.ToString(), Guid.NewGuid().ToString().Substring(0, 8));

                    commaValues = commaValues + (parameterName + ",");
                    parameterlist.Add(new DbParameter(parameterName, objStr, DbType.String));
                    I = I + 1;
                }

                if (commaValues.Length > 1 && commaValues.Contains(","))
                {
                    commaValues = commaValues.Remove(commaValues.Length - 1, 1);
                }
            }

            return commaValues;
        }
    }

    public static class Extension
    {
        /// <summary>
        /// Initialize passed parameters
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterlist"></param>
        public static void Parameterize(this IDbCommand command, List<DbParameter> parameterlist)
        {
            //var parameter = new NpgsqlParameter("@p_clientId", 1);
            //parameter.Direction = ParameterDirection.Input; // Do not use "0"
            //command.Parameters.Add(parameter);

            foreach (DbParameter obj in parameterlist)
            {
                var parameter = new NpgsqlParameter();
                parameter.ParameterName = string.Format("@{0}", obj.Name);
                parameter.Value = obj.Value;
                if (obj.DBDirection == 0) parameter.Direction = ParameterDirection.Input; // Do not use "0"
                parameter.Size = obj.Size;
                if (obj.NpgsqlDbType.HasValue)
                {
                    parameter.NpgsqlDbType = obj.NpgsqlDbType.Value;
                }
                if (obj.DBType.HasValue)
                {
                    parameter.DbType = obj.DBType.Value;
                }
                //if (obj.DBType.HasValue)
                //{
                //    parameter.DbType = obj.DBType.Value;
                //}
                command.Parameters.Add(parameter);
            }
        }

    }
}
