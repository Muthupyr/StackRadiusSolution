using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NpgsqlTypes;
using StackRadius.DBHelper;
using DbParameter = StackRadius.DBHelper.DbParameter;

namespace StackRadius.Repository
{
    public class ClientProfileRepository
    {
        public string GetAllClientProfiles()
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "clientprofilegetall_fn";
                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, null);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string GetClientProfileById(string client_code)
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "clientprofilegetbyid_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("p_client_code", client_code, DbType.String));
                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string InsertOrUpdateClientProfile(string mode, string jsonData)
        {
            jsonData = RemoveViewPropertiesFromJson(jsonData);
            try
            {
                string sqlStr = "clientprofile_sp";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", mode, DbType.String));
                dbParam.Add(new DbParameter("p_data", jsonData, (NpgsqlDbType) NpgsqlTypes.NpgsqlDbType.Jsonb));
                int retval = DbHelper.ExecuteNonQuery(sqlStr, CommandType.StoredProcedure, dbParam);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }         
        }

        public string DeleteClientProfile(string client_code)
        {
            string jsonData = string.Concat("{ \"client_code\": \"" , client_code , "\"}");

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

        public string GetClientProfiles(string jsondata)
        {
            DataTable dtTable = new DataTable();
            try
            {
                JObject obj = JObject.Parse(jsondata);
                string sqlStr = "clientprofileget_sp";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("Mode", "SELECT", DbType.String));

                // Searchable / filter columns. Add more here if the search proc supports them.
                dbParam.Add(new DbParameter("member_code", (string)obj["member_code"], DbType.String));
                dbParam.Add(new DbParameter("client_code", (string)obj["client_code"], DbType.String));
                dbParam.Add(new DbParameter("ucc_status", (string)obj["ucc_status"], DbType.String));
                dbParam.Add(new DbParameter("primary_holder_name", (string)obj["primary_holder_name"], DbType.String));
                dbParam.Add(new DbParameter("primary_holder_pan", (string)obj["primary_holder_pan"], DbType.String));
                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch //(Exception ex)
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        private string RemoveViewPropertiesFromJson(string jsonData)
        {
            jsonData = Common.Common.RemovePropertyFromJson(jsonData, "Mode");
            jsonData = Common.Common.RemovePropertyFromJson(jsonData, "AMCCodeList");
            jsonData = Common.Common.RemovePropertyFromJson(jsonData, "ClientList");
            return jsonData;
        }
    }
}


//Method 1: The JSONB Object Approach 
//This approach passes a single JSON map of keys and values.
//It is highly flexible because you do not have to map
//out 150 strict parameters in the procedure signature.
//sqlCREATE OR REPLACE PROCEDURE insert_large_table_json(p_data JSONB)
//LANGUAGE plpgsql
//AS $$
//BEGIN
//    INSERT INTO your_table_name (
//        col1, col2, col3, -- ... list all 150 columns here
//        col150
//    )
//    SELECT 
//        (p_data->>'col1')::INTEGER,
//        (p_data->> 'col2')::TEXT,
//        (p_data->> 'col3')::BOOLEAN,
//        -- ... use jsonb operators to extract and cast each value
//        (p_data->>'col150')::TIMESTAMP
//    ;
//END;
//$$;

////--Drop PROCEDURE insertclientprofile;

////CREATE OR REPLACE PROCEDURE insertclientprofile(p_data JSONB)
////LANGUAGE plpgsql
////AS $$
////BEGIN
////    INSERT INTO clientprofile (auth_email_sent, auth_status, member_code, client_code)
////    SELECT 
////        (p_data->>'auth_email_sent')::TEXT,
////        (p_data->> 'auth_status')::TEXT,
////        (p_data->> 'member_code')::TEXT,
////        (p_data->> 'client_code')::TEXT
////    ;
////END;
////$$;
//How to call it:sqlCALL insert_large_table_json('{"col1": 42, "col2": "John Doe", "col3": true, "col150": "2026-08-24"}'::jsonb);

//How to call it:sql
//CALL insertclientprofile('{"auth_email_sent": "Y", "auth_status": "N", "member_code": "106167", "client_code": "5010101059"}'::jsonb);

