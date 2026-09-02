using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using NpgsqlTypes;
using StackRadius.DBHelper;

using DbParameter = StackRadius.DBHelper.DbParameter;

namespace StackRadius.Repository
{
    public class SchemeMasterRepository
    {
        public string GetAllSchemeMaster()
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "nse_scheme_master_getall_fn";
                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, null);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string InsertOrUpdateSchemeMaster(string mode, string jsonData)
        {
            jsonData = RemoveViewPropertiesFromJson(jsonData);
            try
            {
                string sqlStr = "nse_scheme_master_sp";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", mode, DbType.String));
                dbParam.Add(new DbParameter("p_data", jsonData, (NpgsqlDbType)NpgsqlTypes.NpgsqlDbType.Jsonb));
                int retval = DbHelper.ExecuteNonQuery(sqlStr, CommandType.StoredProcedure, dbParam);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeleteSchemeMaster(int unique_sr_no)
        {
            string jsonData = string.Concat("{ \"unique_sr_no\": \"", unique_sr_no.ToString(), "\"}");
            try
            {
                string sqlStr = "clientprofile_sp";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "DELETE", DbType.String));
                dbParam.Add(new DbParameter("p_data", jsonData, (NpgsqlDbType)NpgsqlTypes.NpgsqlDbType.Jsonb));
                Object retval = DbHelper.ExecuteScalar(sqlStr, CommandType.StoredProcedure, dbParam);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string RemoveViewPropertiesFromJson(string jsonData)
        {
            jsonData = Common.Common.RemovePropertyFromJson(jsonData, "Mode");
            return jsonData;
        }
    }

}
