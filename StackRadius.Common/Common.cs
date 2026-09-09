using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        public static List<SelectListItem> GetMasterFileTypeList()
        {
            List<SelectListItem> MasterFileTypeList = new List<SelectListItem>();
            MasterFileTypeList.Add(new SelectListItem("Consolidated Scheme master (SCH)", "SCH"));
            MasterFileTypeList.Add(new SelectListItem("SIP Scheme Master (SIP)", "SIP"));
            MasterFileTypeList.Add(new SelectListItem("STP Scheme Master (STP)", "STP"));
            MasterFileTypeList.Add(new SelectListItem("SWP SchemeMaster (SWP)", "SWP"));
            //MasterFileTypeList.Add(new SelectListItem("NAV Download (NAV)", "NAV"));
            //MasterFileTypeList.Add(new SelectListItem("Settlement Calendar Download (SET)", "SET"));
            return MasterFileTypeList;
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

        public static Dictionary<string, string> GetNSESchemeMasterFieldList()
        {
            Dictionary<string, string> fieldInfo = new Dictionary<string, string>();

            string[] fieldName = ("unique_sr_no,scheme_code,rta_scheme_code,amc_scheme_code," +
                "isin,amc_code,scheme_type,plan_type,scheme_name,purchase_allowed," +
                "purchase_transaction_mode,new_purchase_min_amount,additional_purchase_min_amount," +
                "additional_purchase_max_amount,purchase_amount_multiplier,purchase_cutoff_time," +
                "redemption_allowed,redemption_transaction_mode,redemption_min_qty,redemption_qty_multiplier," +
                "redemption_max_qty,redemption_min_amount,redemption_max_amount,redemption_amount_multiplier," +
                "redemption_cutoff_time,rta_agent_code,amc_active_flag,div_reinvest_flag,sip_allowed," +
                "stp_enabled,swp_enabled,switch_allowed,settlement_type,amc_ind,face_value,scheme_start_date," +
                "maturity_date,exit_load_flag,exit_load,lock_in_period_flag,lock_in_period," +
                "channel_partner_code,reopening_date,open_close_ended_scheme").Split(',');

            string[] fieldType = ("int,string,string,string,string,string,string,string,string,string,string,double,double,int,double,time,string,string,double,double,int,double,int,double,time,string,string,string,string,string,string,string,string,string,int,date,date,string,string,string,int,string,date,string").Split(',');

            for (int i = 0; i < fieldName.Length; i++)
            {
                fieldInfo.Add(fieldName[i], fieldType[i]);
            }

            return fieldInfo;
        }

        public static Dictionary<string, string> GetSIPMasterFieldList()
        {
            Dictionary<string, string> fieldInfo = new Dictionary<string, string>();

            string[] fieldName = ("amc_code,amc_name,scheme_code,scheme_name,sip_transaction_mode," +
                "sip_frequency,sip_dates,sip_minimum_gap,sip_maximum_gap,sip_installment_gap," +
                "sip_status,sip_minimum_installment_amount,sip_maximum_installment_amount," +
                "sip_multiplier_amount,sip_minimum_installment_numbers," +
                "sip_maximum_installment_numbers,scheme_isin,scheme_type,pause_flag," +
                "pause_minimum_installments,pause_maximum_installments,pause_modification_count," +
                "filler_1,filler_2,filler_3,filler_4,filler_5").Split(',');

            string[] fieldType = ("string,string,string,string,string,string,string,int,int,int,int,int,int,int,int,int,string,string,string,int,int,int,string,string,string,string,string").Split(',');

            for (int i = 0; i < fieldName.Length; i++)
            {
                fieldInfo.Add(fieldName[i], fieldType[i]);
            }
            return fieldInfo;
        }

        public static Dictionary<string, string> GetSTPMasterFieldList()
        {
            Dictionary<string, string> fieldInfo = new Dictionary<string, string>();

            string[] fieldName = ("amc_code,amc_name,nse_scheme_code,scheme_name,scheme_isin,scheme_type," +
                      "astp_transaction_mode,astp_in_minimum_installment_amount," +
                      "astp_in_maximum_installment_amount,astp_in_multiplier_amount," +
                      "astp_out_minimum_installment_amount,astp_out_maximum_installment_amount," +
                      "astp_out_multiplier_amount,astp_minimum_installment_units," +
                      "astp_maximum_installment_units,astp_multiplier_units," +
                      "astp_minimum_installment_numbers,astp_maximum_installment_numbers," + 
                      "astp_reg_in,astp_reg_out,astp_frequency,astp_dates,astp_minimum_gap," + 
                      "astp_maximum_gap,astp_installment_gap,astp_status")
                      .Split(',');

            string[] fieldType = ("string,string,string,string,string,string,string,int,bigint,double,int,int,double,double,int,double,int,int,int,int,string,string,int,int,int,string").Split(',');

            for (int i = 0; i < fieldName.Length; i++)
            {
                fieldInfo.Add(fieldName[i], fieldType[i]);
            }
            return fieldInfo;
        }

        public static Dictionary<string, string> GetSWPMasterFieldList()
        {
            Dictionary<string, string> fieldInfo = new Dictionary<string, string>();

            string[] fieldName = ("amc_name,nse_scheme_code,scheme_name,scheme_isin,scheme_type,aswp_transaction_mode," +
                                  "aswp_minimum_installment_amount,aswp_maximum_installment_amount,aswp_multiplier_amount," +
                                  "aswp_minimum_installment_units,aswp_maximum_installment_units,aswp_multiplier_units," +
                                  "aswp_minimum_installment_numbers,aswp_maximum_installment_numbers," +
                                  "aswp_frequency,aswp_dates,aswp_minimum_gap,aswp_maximum_gap,aswp_installment_gap")
                                  .Split(',');

            string[] fieldType = ("string,string,string,string,string,string,string,double,int,double,double,int,double,int,int,string,string,int,int,int,string")
                                 .Split(',');

            for (int i = 0; i < fieldName.Length; i++)
            {
                fieldInfo.Add(fieldName[i], fieldType[i]);
            }
            return fieldInfo;
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