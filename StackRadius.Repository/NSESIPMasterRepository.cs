using Newtonsoft.Json;
using NpgsqlTypes;
using StackRadius.DBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using DbParameter = StackRadius.DBHelper.DbParameter;

namespace StackRadius.Repository
{
    public class NSESIPMasterRepository
    {
        public string GetAllSIPMaster()
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "nse_sip_master_get_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "ALL", DbType.String));
                dbParam.Add(new DbParameter("p_scheme_code", "", DbType.String));
                dbParam.Add(new DbParameter("p_sip_frequency", "", DbType.String));
                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string GetSIPMasterBy_Scheme_Frequency(string scheme_code, string sip_frequency)
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "nse_sip_master_get_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "BY_SCHEME_FREQUENCY", DbType.String));
                dbParam.Add(new DbParameter("p_scheme_code", scheme_code, DbType.String));
                dbParam.Add(new DbParameter("p_sip_frequency", sip_frequency, DbType.String));
                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string InsertSIPMaster(string jsonData)
        {
            try
            {
                string sqlStr = "nse_sip_master_sp";
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
        public string UpdateSIPMaster(string jsonData)
        {
            try
            {
                string sqlStr = "nse_sip_master_sp";
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

        public string InsertOrUpdateSIPMaster(string mode, string jsonData)
        {
            try
            {
                string sqlStr = "nse_sip_master_sp";
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

        public string DeleteSIPMaster(string scheme_code, string sip_frequency)
        {
            string jsonData = string.Concat("{ \"scheme_code\": \"", scheme_code, "\", \"sip_frequency\": \"", sip_frequency, "\" }");
            try
            {
                string sqlStr = "nse_sip_master_sp";
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
