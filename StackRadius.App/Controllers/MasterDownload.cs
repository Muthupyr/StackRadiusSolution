using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using StackRadius.Common;
using StackRadius.Entity;
using StackRadius.Models;
using StackRadius.Repository;
using StackRadius.Service;
using StackRadius.Service.NSE;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace StackRadius.Controllers
{
    public class MasterDownloadController : Controller
    {
        public ActionResult Download()
        {
            return View();
        }

        public ActionResult DownloadFile()
        {
            string filetype = "SCH";
            string retMesssage = "";
            string retStatus = "";
            try
            {
                Logger.LogDebug("MasterDownloadController:DownloadFile()");
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
                            StackRadius.Common.Common.SaveToFile(response.Result.filecontent, response.Result.filename);

                            // save the content to db after parsing
                            SaveSchemaMasterToDB(response.Result.filecontent, filetype);

                            // Insert the Master Download History table
                            // TO DO

                        }
                    }
                    retMesssage = "File successfully downloaded and saved as file.";
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
        /// Save the downloaded Schema Master to db
        /// </summary>
        /// <param name="records"></param>
        /// <param name="filetype"></param>
        /// <returns>No of records saved</returns>
        private int SaveSchemaMasterToDB(string records, string filetype)
        {
            string[] fieldNames = GetSchemeMasterFieldList();
            int lineCount;
            string mode = "INSERT";
            SchemaMasterModel schmaMasterObj = new SchemaMasterModel();

            string data = "";
            lineCount = 0;
            try
            {
                using (var reader = new System.IO.StringReader(records))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lineCount++;
                        if (lineCount == 1) continue; // Ignore the header row
                                                
                        Logger.LogDebug($"Line [{lineCount}]: {line}");
                        string[] fieldValues = line.Split('|');
                        string jsonData = GenerateSchemaMasterJson(fieldNames, fieldValues);

                        SchemeMasterRepository repo = new SchemeMasterRepository();
                        data = repo.InsertOrUpdateSchemeMaster(mode, jsonData);
                        if (data != "") break;

                        //23505: duplicate key value violates unique constraint "nse_scheme_master_pkey"
                        //DETAIL: Detail redacted as it may contain sensitive data. Specify 'Include Error Detail' in the connection string to include this information.
                    }
                }
                data = (data == "") ? "success" : "unsuccess";
                return lineCount;
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Error in SaveSchemaMasterToDB(): {ex.Message}");
                return lineCount;
            }
        }

        /// <summary>
        /// retusn the json
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        private string GenerateSchemaMasterJson(string[] fieldNames, string[] fieldValues)
        {
            //json = {"unique_sr_no": "29724", "scheme_code": "BS41B-GR"}";
            var str = "";
            for (int i = 0; i < fieldValues.Length; i++)
            {
                str += string.Concat("\"", fieldNames[i], "\": ", "\"", fieldValues[i], "\", ");
            }
            str = str.Trim();
            str = str.TrimEnd(',');
            string json = string.Concat("{", str, "}");
            return json;
        }

        private string[] GetSchemeMasterFieldList()
        {
            string fieldList = "unique_sr_no,scheme_code,rta_scheme_code,amc_scheme_code," +
                "isin,amc_code,scheme_type,plan_type,scheme_name,purchase_allowed," +
                "purchase_transaction_mode,new_purchase_min_amount,additional_purchase_min_amount," +
                "additional_purchase_max_amount,purchase_amount_multiplier,purchase_cutoff_time," +
                "redemption_allowed,redemption_transaction_mode,redemption_min_qty,redemption_qty_multiplier," +
                "redemption_max_qty,redemption_min_amount,redemption_max_amount,redemption_amount_multiplier," +
                "redemption_cutoff_time,rta_agent_code,amc_active_flag,div_reinvest_flag,sip_allowed," +
                "stp_enabled,swp_enabled,switch_allowed,settlement_type,amc_ind,face_value,scheme_start_date," +
                "maturity_date,exit_load_flag,exit_load,lock_in_period_flag,lock_in_period," +
                "channel_partner_code,reopening_date,open_close_ended_scheme";
            return fieldList.Split(',');

        }


    }
}

