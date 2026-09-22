using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackRadius.DBHelper;
using StackRadius.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using DbParameter = StackRadius.DBHelper.DbParameter;

namespace StackRadius.Repository
{
    public class DashboardRepository
    {
        public string GetDahboard(string jsonData)
        {
            JObject obj = JObject.Parse(jsonData);
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "dashboard_get_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "ALL", DbType.String));

                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }
    }
}
