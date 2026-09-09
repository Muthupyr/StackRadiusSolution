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
    public class NSESWPMasterRepository
    {
        public string GetAllSWPMaster()
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "nse_swp_master_get_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "ALL", DbType.String));
                dbParam.Add(new DbParameter("p_nse_scheme_code", "", DbType.String));
                dbParam.Add(new DbParameter("p_aswp_frequency", "", DbType.String));
                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string GetSWPMasterBy_Scheme_Frequency(string nse_scheme_code, string aswp_frequency)
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "nse_swp_master_get_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "BY_SCHEME_FREQUENCY", DbType.String));
                dbParam.Add(new DbParameter("p_nse_scheme_code", nse_scheme_code, DbType.String));
                dbParam.Add(new DbParameter("p_aswp_frequency", aswp_frequency, DbType.String));
                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string InsertSWPMaster(string jsonData)
        {
            try
            {
                string sqlStr = "nse_swp_master_sp";
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
        public string UpdateSWPMaster(string jsonData)
        {
            try
            {
                string sqlStr = "nse_swp_master_sp";
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

        public string InsertOrUpdateSWPMaster(string mode, string jsonData)
        {
            try
            {
                string sqlStr = "nse_swp_master_sp";
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

        public string DeleteSWPMaster(string nse_scheme_code, string aswp_frequency)
        {
            string jsonData = string.Concat("{ \"nse_scheme_code\": \"", nse_scheme_code, "\", \"aswp_frequency\": \"", aswp_frequency, "\" }");
            try
            {              
                string sqlStr = "nse_swp_master_sp";
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
