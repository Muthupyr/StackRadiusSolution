using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace StackRadius.Common
{
    public static class Common
    {
        public static void SaveToFile(string source, string filename)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string debugFolder = "\\bin\\Debug\\net5.0\\";
            string releaseFolder = "\\bin\\Release\\net5.0\\";
            string folderPath = "";

            if (baseDirectory.EndsWith(debugFolder))
            {
                folderPath = baseDirectory.Substring(0, baseDirectory.Length - debugFolder.Length);
            }
            if (baseDirectory.EndsWith(releaseFolder))
            {
                folderPath = baseDirectory.Substring(0, baseDirectory.Length - releaseFolder.Length);
            }

            folderPath = Path.Combine(folderPath, "Downloads");
            string filePath = Path.Combine(folderPath, filename);

            // Write the string to the file
            File.WriteAllText(filePath, source);
            Logger.LogDebug("File saved to " + filePath + ".");
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

        public static List<SelectListItem> GetAMCCodeList()
        {
            List<SelectListItem> AMCCodeList = new List<SelectListItem>();

            // Loops through the actual enum constants
            foreach (AMCCodes amcCode in Enum.GetValues<AMCCodes>())
            {
                SelectListItem item = new SelectListItem();
                item.Text = amcCode.ToString();
                item.Value = amcCode.ToString();
                AMCCodeList.Add(item);
            }
            return AMCCodeList;
        }
        public enum AMCCodes
        {
            B,
            K,
            H,
            G,
            CR,
            O,
            UK
        }

        //public static void ErrorLog(Exception exception, string url, string userName)
        //{
        //    ErrorLog.ErrorLogClient Req = new ErrorLog.ErrorLogClient();
        //    ErrorLogViewModel ErrorLogViewModel = new ErrorLogViewModel();
        //    ErrorLogViewModel.UserName = userName;
        //    ErrorLogViewModel.DateTime = Common.ConvertUTCtoLocalTime();
        //    ErrorLogViewModel.ErrorLocation = "Referer:" + url + "|Stack Trace:" + exception.ToString();
        //    type = exception.GetType();
        //    ErrorLogViewModel.ErrorType = type.Name;
        //    ErrorLogViewModel.ErrorDescription = exception.Message.ToString();
        //    string JsonData = JsonConvert.SerializeObject(ErrorLogViewModel);
        //    Req.InsertUpdateErrorLog(JsonData);
        //}
        public static T DeepCopy<T>(T source)
        {
            if (source == null) return default;

            string jsonString = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<T>(jsonString)!;
        }

    }
}