using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackRadius.DBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using DbParameter = StackRadius.DBHelper.DbParameter;

namespace StackRadius.Repository
{
    public class MasterHistoryRepository
    {
        public string GetAllMasterHistory()
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "masterhistorygetall_fn";
                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, null);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string InsertOrUpdateMasterHistory(string JsonData)
        {
            try
            {
                JObject obj = JObject.Parse(JsonData);
                string sqlStr = "masterhistory_sp";
                List<DbParameter> dbParam = new List<DbParameter>();
                string mode = ((string)obj["Mode"] == "Save") ? "INSERT" : "UPDATE";
                dbParam.Add(new DbParameter("mode", mode, DbType.String));
                dbParam.Add(new DbParameter("p_Id", (Int32?)obj["Id"], DbType.Int32));
                dbParam.Add(new DbParameter("p_DownloadOn", string.IsNullOrEmpty(obj["DownloadOn"].ToString()) ? DBNull.Value : (DateTime)obj["DownloadOn"], DbType.DateTime));
                dbParam.Add(new DbParameter("p_Filename", string.IsNullOrEmpty(obj["Filename"].ToString()) ? DBNull.Value : obj["Filename"], DbType.String));
                dbParam.Add(new DbParameter("p_NoOfRecords", (Int32?)obj["Id"], DbType.Int32) );
                dbParam.Add(new DbParameter("p_UpdatedOn", string.IsNullOrEmpty(obj["UpdatedOn"].ToString()) ? DBNull.Value : (DateTime)obj["UpdatedOn"], DbType.DateTime));
                int retval = DbHelper.ExecuteNonQuery(sqlStr, CommandType.StoredProcedure, dbParam);
                return "";
    
            }
            catch //(Exception ex)
            {
                return "";
            }
        }
        public string DeleteMasterHistory(string Jsondata)        
        {
            try
            {
                JObject obj = JObject.Parse(Jsondata);
                string sqlStr = "masterhistory_sp";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "DELETE", DbType.String));
                dbParam.Add(new DbParameter("p_Id", (Int32?)obj["Id"], DbType.Int32));
                dbParam.Add(new DbParameter("p_DownloadOn", DBNull.Value));
                dbParam.Add(new DbParameter("p_Filename", DBNull.Value));
                dbParam.Add(new DbParameter("p_NoOfRecords", DBNull.Value));
                dbParam.Add(new DbParameter("p_UpdatedOn", DBNull.Value));

                Object retval = DbHelper.ExecuteScalar(sqlStr, CommandType.StoredProcedure, dbParam);
                return "";
            }
            catch //(Exception ex)
            {
                //return Common.InsertUpdateErrorLog(ex, this.GetType().Name + "/DeleteProduct");
                return "";

            }
        }      

      
    }
}
