using Newtonsoft.Json;
using StackRadius.App;
using StackRadius.Entity;
using System;
using System.Text.Json.Nodes;

namespace StackRadius.Repository
{
    public class Common
    {
        /// <summary>
        /// Usage : Common.InsertUpdateErrorLog(exception, this.GetType().Name + "/GetMasterUser")
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static string InsertUpdateErrorLog(Exception ex, string location)
        {
            ErrorLogRepository repo = new ErrorLogRepository();
            ErrorLogModel obj = new ErrorLogModel();
            obj.UserName = "DA Layer";
            obj.DateTime = System.DateTime.Now;
            obj.ErrorLocation = "Referer:" + location + "|Stack Trace:" + ex.ToString();
            obj.ErrorType = ex.Message;
            obj.ErrorDescription = ex.StackTrace;
            repo.InsertUpdateErrorLog(JsonConvert.SerializeObject(obj));

            //SqlException SqlExcep = default(SqlException);
            //SqlExcep = (SqlException)ex;
            //if (SqlExcep != null)
            //{
            //    List<SqlError> SqlErrorList = new List<SqlError>()
            //    {
            //        new SqlError() { ErrorNumber = 2812, ErrorValue = "StoredProcedureMissing" } ,
            //        new SqlError() { ErrorNumber = 2601, ErrorValue = "DUPLICATE" } ,
            //        new SqlError() { ErrorNumber = 547, ErrorValue = "FOREIGN" },
            //        new SqlError() { ErrorNumber = 2627, ErrorValue = "UNIQUE" }
            //    };
            //    string ErrorValue = SqlErrorList.Where(a => a.ErrorNumber == SqlExcep.Number).Select(b => b.ErrorValue).FirstOrDefault();
            //    return ErrorValue;
            //}
            return null;
        }
        public static string RemovePropertyFromJson(string jsonData, string propertyToRemove)
        {
            if (jsonData.Trim() == string.Empty)
                return "";

            // 1. Parse to mutable node
            JsonNode node = JsonNode.Parse(jsonData);

            // 2. Remove property
            node.AsObject().Remove(propertyToRemove);

            // 3. Convert back to string
            string result = node.ToJsonString();

            return result;
        }

    }
}