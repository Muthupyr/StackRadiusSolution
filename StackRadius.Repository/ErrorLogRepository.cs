using Newtonsoft.Json.Linq;
using StackRadius.DBHelper;
using System;
using System.Collections.Generic;
using System.Data;

namespace StackRadius.Repository
{
    public class ErrorLogRepository
    {       
        public string InsertUpdateErrorLog(string jsonData)
        {
            try
            {
                JObject obj = JObject.Parse(jsonData);
                string sqlStr = "ErrorLog_sp";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("Mode", "INSERT", DbType.String));
                dbParam.Add(new DbParameter("Id", (Int32?)obj["Id"], DbType.Int32));
                dbParam.Add(new DbParameter("UserName", (String)obj["UserName"], DbType.String, 50));
                dbParam.Add(new DbParameter("DateTime", (DateTime?)obj["DateTime"], DbType.DateTime));
                dbParam.Add(new DbParameter("ErrorLocation", (String)obj["ErrorLocation"], DbType.String));
                dbParam.Add(new DbParameter("ErrorType", (String)obj["ErrorType"], DbType.String));
                dbParam.Add(new DbParameter("ErrorDescription", (String)obj["ErrorDescription"], DbType.String));
                DbHelper.ExecuteNonQuery(sqlStr, CommandType.StoredProcedure, dbParam);
                return "";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
