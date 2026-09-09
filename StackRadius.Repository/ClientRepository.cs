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
    public class ClientRepository
    {
        public string GetClients(string jsonData)
        {
            JObject obj = JObject.Parse(jsonData);
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "client_get_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "ALL", DbType.String));
                dbParam.Add(new DbParameter("p_clientid", 0, DbType.Int32));
                dbParam.Add(new DbParameter("p_record_from", (Int32?)obj["RecordFrom"], DbType.Int32));
                dbParam.Add(new DbParameter("p_record_to", (Int32?)obj["RecordTo"], DbType.Int32));
                dbParam.Add(new DbParameter("p_sort_key", (String)obj["SortKey"] == null ? "clientId" : (String)obj["SortKey"], DbType.String, 100));
                dbParam.Add(new DbParameter("p_sort_dir", (String)obj["SortDir"] == null ? "ASC" : (String)obj["SortDir"], DbType.String, 10));
                dbParam.Add(new DbParameter("p_search_string", string.IsNullOrEmpty(obj["FilterCondition"].ToString()) ? "" : obj["FilterCondition"].ToString(), DbType.String, 100));

                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string GetClientsById(int clientId)
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "client_get_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "BYID", DbType.String));
                dbParam.Add(new DbParameter("p_clientId", (Int32?)clientId, DbType.Int32));
                dbParam.Add(new DbParameter("p_record_from", DBNull.Value));
                dbParam.Add(new DbParameter("p_record_to", DBNull.Value));
                dbParam.Add(new DbParameter("p_sort_key", ""));
                dbParam.Add(new DbParameter("p_sort_dir", ""));
                dbParam.Add(new DbParameter("p_search_string", ""));

                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string InsertOrUpdateClient(string JsonData)
        {
            try
            {
                JObject obj = JObject.Parse(JsonData);
                string sqlStr = "client_sp";
                List<DbParameter> dbParam = new List<DbParameter>();
                string mode = ((string)obj["Mode"] == "Save") ? "INSERT" : "UPDATE";
                dbParam.Add(new DbParameter("mode", mode, DbType.String));
                dbParam.Add(new DbParameter("p_clientid", (Int32?)obj["clientId"], DbType.Int32));
                dbParam.Add(new DbParameter("p_clientname", string.IsNullOrEmpty(obj["clientName"].ToString()) ? DBNull.Value : (string)obj["clientName"], DbType.String));
                dbParam.Add(new DbParameter("p_amccode", string.IsNullOrEmpty(obj["amcCode"].ToString()) ? DBNull.Value : (string)obj["amcCode"], DbType.String));
                dbParam.Add(new DbParameter("p_panno", string.IsNullOrEmpty(obj["panNo"].ToString()) ? DBNull.Value : (string)obj["panNo"], DbType.String));
                dbParam.Add(new DbParameter("p_mobileno", string.IsNullOrEmpty(obj["mobileNo"].ToString()) ? DBNull.Value : (string)obj["mobileNo"], DbType.String));
                dbParam.Add(new DbParameter("p_invemail", string.IsNullOrEmpty(obj["invEmail"].ToString()) ? DBNull.Value : (string)obj["invEmail"], DbType.String));
                int retval = DbHelper.ExecuteNonQuery(sqlStr, CommandType.StoredProcedure, dbParam);
                return "";
            }
            catch //(Exception ex)
            {
                return "";
            }
        }

        public string DeleteClient(string Jsondata)
        {
            try
            {
                JObject obj = JObject.Parse(Jsondata);
                string sqlStr = "client_sp";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "DELETE", DbType.String));
                dbParam.Add(new DbParameter("p_clientid", (Int32?)obj["clientId"], DbType.Int32));
                dbParam.Add(new DbParameter("p_clientname", ""));
                dbParam.Add(new DbParameter("p_amccode", DBNull.Value));
                dbParam.Add(new DbParameter("p_panno", DBNull.Value));
                dbParam.Add(new DbParameter("p_mobileno", DBNull.Value));
                dbParam.Add(new DbParameter("p_invemail", DBNull.Value));
                Object retval = DbHelper.ExecuteScalar(sqlStr, CommandType.StoredProcedure, dbParam);
                return "";
            }
            catch //(Exception ex)
            {
                //return Common.InsertUpdateErrorLog(ex, this.GetType().Name + "/DeleteClient");
                return "";

            }
        }

    }
}
