using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackRadius.DBHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace StackRadius.Repository
{
    public class MasterUserRepository
    {      
        public string GetMasterUsers(string jsonData)
        {
            JObject obj = JObject.Parse(jsonData);
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "MasterUser_get_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "ALL", DbType.String));
                dbParam.Add(new DbParameter("p_masteruserid", 0, DbType.Int32));
                dbParam.Add(new DbParameter("p_record_from", (Int32?)obj["RecordFrom"], DbType.Int32));
                dbParam.Add(new DbParameter("p_record_to", (Int32?)obj["RecordTo"], DbType.Int32));
                dbParam.Add(new DbParameter("p_sort_key", (String)obj["SortKey"] == null ? "MasterUserGroupId" : (String)obj["SortKey"], DbType.String, 100));
                dbParam.Add(new DbParameter("p_sort_dir", (String)obj["SortDir"] == null ? "ASC" : (String)obj["SortDir"], DbType.String, 10));
                dbParam.Add(new DbParameter("p_search_string", string.IsNullOrEmpty(obj["FilterCondition"].ToString()) ? "" : obj["FilterCondition"].ToString(), DbType.String, 255));

                dtTable = DbHelper.ExecuteDataTable(sqlStr, CommandType.StoredProcedure, dbParam);
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
            catch
            {
                return JsonConvert.SerializeObject(dtTable, Formatting.Indented);
            }
        }

        public string GetMasterUserById(int masterUserId)
        {
            DataTable dtTable = new DataTable();
            try
            {
                string sqlStr = "MasterUser_get_fn";
                List<DbParameter> dbParam = new List<DbParameter>();
                dbParam.Add(new DbParameter("mode", "BYID", DbType.String));
                dbParam.Add(new DbParameter("p_masteruserid", (Int32?)masterUserId, DbType.Int32));
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

        public string InsertUpdateMasterUsers(string jsonData)
        {
            JObject obj = JObject.Parse(jsonData);
            string sqlStr = "MasterUser_get_fn";
            List<DbParameter> dbParam = new List<DbParameter>();
            try
            {
                dbParam = new List<DbParameter>();

                int? MasterUserId = (int?)obj["MasterUserId"];
                if (MasterUserId != null && MasterUserId != 0)
                {
                    dbParam.Add(new DbParameter("Mode", "UPDATE", DbType.String));
                    dbParam.Add(new DbParameter("p_masteruserid", (int?)MasterUserId, DbType.Int32));
                }
                else
                {
                    dbParam.Add(new DbParameter("Mode", "INSERT", DbType.String));
                    dbParam.Add(new DbParameter("p_masteruserid", 0, DbType.Int32));
                }
                dbParam.Add(new DbParameter("p_usergroupid", (Int32?)obj["UserGroupId"], DbType.Int32));
                dbParam.Add(new DbParameter("p_username", (String)obj["UserName"], DbType.String, 50));
                dbParam.Add(new DbParameter("p_usergroupname", (String)obj["UserGroupName"], DbType.String, 50));
                dbParam.Add(new DbParameter("p_userstatus", (Boolean?)obj["UserStatus"], DbType.Boolean));
                dbParam.Add(new DbParameter("p_password", (String)obj["Password"], DbType.String, 25));
                dbParam.Add(new DbParameter("p_title", (String)obj["TitleDisplay"], DbType.String, 50));
                dbParam.Add(new DbParameter("p_lastname", (String)obj["LastName"], DbType.String, 50));
                dbParam.Add(new DbParameter("p_email", (String)obj["Email"], DbType.String, 100));
                dbParam.Add(new DbParameter("p_dob", (DateTime?)obj["DOB"], DbType.DateTime));
                dbParam.Add(new DbParameter("p_phoneno", (String)obj["PhoneNo"], DbType.String, 15));
                dbParam.Add(new DbParameter("p_addressone", (String)obj["AddressOne"], DbType.String, 100));
                dbParam.Add(new DbParameter("p_addresstwo", (String)obj["AddressTwo"], DbType.String, 100));
                dbParam.Add(new DbParameter("p_addressthree", (String)obj["AddressThree"], DbType.String, 100));
                dbParam.Add(new DbParameter("p_lastlogindate", (DateTime?)obj["LastLoginDate"], DbType.DateTime));

                object maxUserMastID = DbHelper.ExecuteNonQuery(sqlStr, CommandType.StoredProcedure, dbParam);
                return "";
                //return Convert.ToString(maxUserMastID);
            }
            catch (Exception exception)
            {
                return Common.InsertUpdateErrorLog(exception, this.GetType().Name + "/InsertUpdateMasterUsers");
            }
        }

        public string DeleteMasterUsers(string jsonData)
        {
            JObject obj = JObject.Parse(jsonData);
            string sqlStr = "MasterUser_get_fn";
            List<DbParameter> dbParam = new List<DbParameter>();
            try
            {
                dbParam = new List<DbParameter>();
                int? MasterUserId = (int?)obj["MasterUserId"];
                if (MasterUserId != null && MasterUserId != 0)
                {
                    dbParam.Add(new DbParameter("Mode", "DELETE", DbType.String));
                    dbParam.Add(new DbParameter("p_masteruserid", MasterUserId, DbType.Int32));
                    DbHelper.ExecuteNonQuery(sqlStr, CommandType.StoredProcedure, dbParam);
                }
                return "";
            }
            catch (Exception exception)
            {
                return Common.InsertUpdateErrorLog(exception, this.GetType().Name + "/DeleteMasterUsers");
            }
        }
    }
}
