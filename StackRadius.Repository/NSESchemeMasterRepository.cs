using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using NpgsqlTypes;
using StackRadius.DBHelper;

using DbParameter = StackRadius.DBHelper.DbParameter;

namespace StackRadius.Repository
{
    public class NSESchemeMasterRepository
    {
        public string GetAllSchemeMaster()
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "nse_scheme_master_get_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "ALL", DbType.String));
                dbParam.Add(new DbParameter("p_unique_sr_no", 0, DbType.Int32));
                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string GetSchemeMasterById(int unique_sr_no)
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "nse_scheme_master_get_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "BYID", DbType.String));
                dbParam.Add(new DbParameter("p_unique_sr_no", (Int32?)unique_sr_no, DbType.Int32));
                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string InsertSchemeMaster(string jsonData)
        {
            try
            {
                string sqlStr = "nse_scheme_master_sp";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "INSERT", DbType.String));
                dbParam.Add(new DbParameter("p_data", jsonData, (NpgsqlDbType)NpgsqlTypes.NpgsqlDbType.Jsonb));
                int retval = DbHelper.ExecuteNonQuery(sqlStr, CommandType.StoredProcedure, dbParam);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string UpdateSchemeMaster(string jsonData)
        {
            try
            {
                string sqlStr = "nse_scheme_master_sp";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "UPDATE", DbType.String));
                dbParam.Add(new DbParameter("p_data", jsonData, (NpgsqlDbType)NpgsqlTypes.NpgsqlDbType.Jsonb));
                int retval = DbHelper.ExecuteNonQuery(sqlStr, CommandType.StoredProcedure, dbParam);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string InsertOrUpdateSchemeMaster(string mode, string jsonData)
        {
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
                string sqlStr = "nse_scheme_master_sp";
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
    }
}
