using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;

using StackRadius.Common;
using StackRadius.Entity;
using StackRadius.Models;
using StackRadius.Repository;
using StackRadius.Service;
using StackRadius.Service.NSE;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;

namespace StackRadius.Controllers
{
    public class MasterDownloadController : Controller
    {
        public ActionResult Index()
        {
            MasterDownloadModel obj = new MasterDownloadModel();

            // Data to be initialized goes here
            obj.MasterFileTypeList = Common.Common.GetMasterFileTypeList();
            obj.FileType = "SCH";
            return View("Index", obj);
        }

        public ActionResult DownloadFile(MasterDownloadModel obj)
        {
            string filetype = obj.FileType;         //"SCH" , "STP" ... ;
            string retMesssage = "";
            string retStatus = "";
            string retval = "";
            try
            {
                Logger.LogDebug("MasterDownloadController:DownloadFile(). Filetype : " + filetype);
                MasterDownloadRequest request = new MasterDownloadRequest(filetype);
                ReturnMessageWrapper<MasterDownloadResponse> response = MasterDownload.MasterDownloadReport(request);

                if (!response.HasError && response.StatusCode == 200)
                {
                    Logger.LogDebug($"response_status    : {response.Result.response_status}");
                    if (response.Result.response_status == "S")
                    {
                        Logger.LogDebug($"filename           : {response.Result.filename}");
                        Logger.LogDebug($"Content Length     : {response.Result.filecontent.Length}");

                        // Save the content to file 
                        if (response.Result.filecontent.Length > 0)
                        {
                            // save the content to file in the download folder
                            // StackRadius.Common.Common.SaveToFile(response.Result.filecontent, response.Result.filename);

                            // save the content to db after parsing
                            retval = SaveToDB(response.Result.filecontent, filetype);

                            // Insert into Master Download History table
                            // TO DO

                        }
                    }

                    retMesssage = "File successfully downloaded and saved to file/DB.";
                    retStatus = "Success";
                }
                else if (response.HasError && response.StatusCode == 403)   // 403 Forbidden
                {
                    Logger.LogDebug($"Status      : {response.Error.status}");
                    Logger.LogDebug($"Error_type  : {response.Error.error_type}");
                    Logger.LogDebug($"Message     : {response.Error.message}");
                    retMesssage = $"{response.Error.message}";
                    retStatus = "Failure";
                }
                else // 404 
                {
                    Logger.LogDebug($"Has Error     : {response.HasError}");
                    Logger.LogDebug($"Error remark  : {response.Result.response_remark}");
                    retMesssage = $"{response.Result.response_remark}";
                    retStatus = "Failure";
                }

                return Json(new BasicViewModel() { message = retMesssage, status = retStatus });
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Save the downloaded Master Files to db
        /// </summary>
        /// <param name="records"></param>
        /// <param name="filetype"></param>
        /// <returns>No of records saved</returns>
        private string SaveToDB(string records, string filetype)
        {
            int lineCount = 0;
            List<string> textList = new List<string>();
            Dictionary<string, string> fieldNames = new Dictionary<string, string>();
            string retval = "";

            try
            {
                // Read the records line by line and save to text List
                using (var reader = new System.IO.StringReader(records))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lineCount++;
                        textList.Add(line);
                    }
                }

                Logger.LogDebug($"Total Rows (including header) : {lineCount}");

                switch (filetype)
                {
                    case "SCH":
                        fieldNames = Common.Common.GetNSESchemeMasterFieldList();
                        retval = SaveToSchemaMaster(textList, fieldNames);
                        break;
                    case "SIP":
                        fieldNames = Common.Common.GetSIPMasterFieldList();
                        retval = SaveToSIPMaster(textList, fieldNames);
                        break;
                    case "STP":
                        fieldNames = Common.Common.GetSTPMasterFieldList();
                        retval = SaveToSTPMaster(textList, fieldNames);
                        break;
                    case "SWP":
                        fieldNames = Common.Common.GetSWPMasterFieldList();
                        retval = SaveToSWPMaster(textList, fieldNames);
                        break;
                    default:
                        break;
                }
                //data = (retval == "") ? "success" : "unsuccess";
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Error in SaveToDB(): {ex.Message}");
            }

            return retval;
        }

        private string SaveToSchemaMaster(List<string> TextLines, Dictionary<string, string> fieldNames)
        {
            string data = "";
            NSESchemeMasterModel schmaMasterObj = new NSESchemeMasterModel();
            NSESchemeMasterRepository repo = new NSESchemeMasterRepository();

            try
            {
                for (int i = 0; i < TextLines.Count; i++)
                {
                    string line = TextLines[i];
                    if (i == 0)
                    {
                        //Ignore the header row
                        continue;
                    }

                    string[] fieldValues = line.Split('|');

                    string jsonData = GenerateJson(fieldNames, fieldValues);
                    data = repo.InsertSchemeMaster(jsonData);

                    // if there is any error in saving the record,
                    // break the loop and return the line count
                    // check for primary key violation
                    if (data != "") break;
                }
                Logger.LogDebug($"Records saved to Scheme Master : {TextLines.Count}");
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Error in SaveToSchemaMaster(): {ex.Message}");
            }

            //successfully saved all records, return empty string   
            return "";
        }

        private string SaveToSIPMaster(List<string> TextLines, Dictionary<string, string> fieldNames)
        {
            string data = "";
            SIPMasterModel sipMasterObj = new SIPMasterModel();
            SIPMasterRepository repo = new SIPMasterRepository();
            try
            {
                for (int i = 0; i < TextLines.Count; i++)
                {
                    string line = TextLines[i];
                    if (i == 0)
                    {
                        //Ignore the header row
                        continue;
                    }

                    string[] fieldValues = line.Split('|');
                    string jsonData = GenerateJson(fieldNames, fieldValues);
                    data = repo.InsertSIPMaster(jsonData);

                    // if there is any error in saving the record,
                    // break the loop and return the line count
                    // check for primary key violation
                    if (data != "") break;
                }
                Logger.LogDebug($"Records saved to SIP Master : {TextLines.Count}");

            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Error in SaveToSIPMaster(): {ex.Message}");
            }
            //successfully saved all records, return empty string   
            return "";
        }

        private string SaveToSTPMaster(List<string> TextLines, Dictionary<string, string> fieldNames)
        {
            string data = "";
            STPMasterModel stpMasterObj = new STPMasterModel();
            STPMasterRepository repo = new STPMasterRepository();

            try
            {
                for (int i = 0; i < TextLines.Count; i++)
                {
                    string line = TextLines[i];
                    if (i == 0)
                    {
                        //Ignore the header row
                        continue;
                    }

                    string[] fieldValues = line.Split('|');
                    string jsonData = GenerateJson(fieldNames, fieldValues);
                    data = repo.InsertSTPMaster(jsonData);

                    // if there is any error in saving the record,
                    // break the loop and return the line count
                    // check for primary key violation
                    if (data != "") break;
                }
                Logger.LogDebug($"Records saved to STP Master : {TextLines.Count}");

            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Error in SaveToSTPMaster(): {ex.Message}");
            }
            //successfully saved all records, return empty string   
            return "";
        }
        private string SaveToSWPMaster(List<string> TextLines, Dictionary<string, string> fieldNames)
        {
            string data = "";
            SWPMasterModel swpMasterObj = new SWPMasterModel();
            SWPMasterRepository repo = new SWPMasterRepository();

            try
            {
                for (int i = 0; i < TextLines.Count; i++)
                {
                    string line = TextLines[i];
                    if (i == 0)
                    {
                        //Ignore the header row
                        continue;
                    }

                    string[] fieldValues = line.Split('|');
                    string jsonData = GenerateJson(fieldNames, fieldValues);
                    data = repo.InsertSWPMaster(jsonData);

                    // if there is any error in saving the record,
                    // break the loop and return the line count
                    // check for primary key violation
                    if (data != "") break;
                }
                Logger.LogDebug($"Records saved to SWP Master : {TextLines.Count}");

            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Error in SaveToSWPMaster(): {ex.Message}");
            }
            //successfully saved all records, return empty string   
            return "";
        }

        /// <summary>
        /// returns the json
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        private string GenerateJson(Dictionary<string, string> fieldNames, string[] fieldValues)
        {
            if (fieldNames == null) return "{}";

            var sb = new System.Text.StringBuilder();
            int count = Math.Min(fieldNames.Count, fieldValues.Length);

            try
            {

                for (int i = 0; i < count; i++)
                {
                    if (i > 0) sb.Append(", ");
                    string fieldName = fieldNames.Keys.ElementAt(i);
                    string fieldType = fieldNames.Values.ElementAt(i);
                    fieldType = fieldType.ToLower().Trim();

                    sb.Append('"').Append(fieldName).Append("\": ");

                    if (fieldType.ToLower() == "string")
                    {
                        if (string.IsNullOrEmpty(fieldValues[i]))
                            sb.Append("\"\"");
                        else
                            sb.Append('"').Append(fieldValues[i]).Append('"');
                    }
                    else if (fieldType == "int" || fieldType == "long")
                    {
                        if (string.IsNullOrEmpty(fieldValues[i]))
                            sb.Append("0");
                        else
                            sb.Append(fieldValues[i]);
                    }
                    else if (fieldType == "double")
                    {
                        if (string.IsNullOrEmpty(fieldValues[i]))
                            sb.Append("0");
                        else
                            sb.Append('"').Append(fieldValues[i]).Append('"');
                    }
                    else if (fieldType == "date")
                    {
                        if (string.IsNullOrEmpty(fieldValues[i]))
                            sb.Append("null");
                        else
                        {
                            // 1. Parse the string into a DateTime object as "01-01-2010"
                            DateTime date = DateTime.ParseExact(fieldValues[i], "dd-MM-yyyy", CultureInfo.InvariantCulture);

                            // 2. Format the DateTime object into the target "YYYY-MM-DD" string
                            string outputDate = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                            sb.Append('"').Append(outputDate).Append('"');
                        }
                    }
                    else if (fieldType == "time")
                    {
                        if(string.IsNullOrEmpty(fieldValues[i]))
                            sb.Append("null");
                        else
                            sb.Append('"').Append(fieldValues[i]).Append('"');
                    }
                    //else
                    //{
                    //    sb.Append('"').Append(fieldValues[i] ?? "").Append('"');
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Error in GenerateJson(): {ex.Message}");
            }
            return "{" + sb.ToString() + "}";
        }
    }
}

