using System.Data;
using System.Collections.Generic;

namespace StackRadius.DBHelper
{
    public interface IDB
    {

        IDbConn GetDBConnection();

        IDbCommand GetDBCommand();
        DataTable ExecuteDataTable(string query, CommandType cmdType, List<DbParameter> parameterlist);
        DataTable ExecuteDataTable(IDbConn dbConnection, string query, CommandType cmdType, List<DbParameter> parameterlist);

        int ExecuteNonQuery(string query, CommandType cmdType, List<DbParameter> parameterlist);

        int ExecuteNonQuery(IDbConn dbConnection, string query, CommandType cmdType, List<DbParameter> parameterlist);

        object ExecuteScalar(string query, CommandType cmdType, List<DbParameter> parameterlist);

        IDataReader ExecuteReader(string query, CommandType cmdType, List<DbParameter> parameterlist);

        DataSet ExecuteDataSet(IDbConn dbConnection, string query, CommandType cmdType, List<DbParameter> parameterlist);

        DataSet ExecuteDataSet(string query, CommandType cmdType, List<DbParameter> parameterlist);

        object ExecuteScalar(IDbConn dbConnection, string query, CommandType cmdType, List<DbParameter> parameterlist);

        IDataReader ExecuteReader(IDbConn dbConnection, string query, CommandType cmdType, List<DbParameter> parameterlist);

    }
}
