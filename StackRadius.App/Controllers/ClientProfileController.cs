
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using StackRadius.Common;
using StackRadius.Entity;
using StackRadius.Models;
using StackRadius.Repository;
using StackRadius.Service.NSE;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace StackRadius.Controllers
{
    public class ClientProfileController : Controller
    {
        public ActionResult Index()
        {
            ClientProfileRepository repo = new ClientProfileRepository();
            string retVal = repo.GetAllClientProfiles();
            List<ClientProfileModel> lst = JsonConvert.DeserializeObject<List<ClientProfileModel>>(retVal);
            return View("ClientProfileViewGrid", lst);
        }

        public ActionResult Add()
        {
            ClientProfileModel obj = new ClientProfileModel();

            //// Data to be initialized goes here
            //obj.Mode = "Save";
            //obj.ClientProfileId = 3;
            //obj.amcCode = "B";
            //obj.panNo = "ER4567890";
            //obj.mobileNo = "1234567890";
            //obj.invEmail = "raja@yahoo.com";
            //obj.AMCCodeList = Common.Common.GetAMCCodeList();

            return View("ClientProfileAdd", obj);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            try
            {
                ClientProfileRepository repo = new ClientProfileRepository();
                string retVal = repo.GetClientProfileById(id);
                List<ClientProfileModel> lst = JsonConvert.DeserializeObject<List<ClientProfileModel>>(retVal);

                ClientProfileModel obj = new ClientProfileModel();
                obj = lst.FirstOrDefault();
                obj.Mode = "Edit";
                obj.AMCCodeList = Common.Common.GetAMCCodeList();
                return View("ClientProfileAdd", obj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// InsertUpdateClientProfile
        /// This method is called from the View page using ajax call
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns>new BasicViewModel() object </returns>
        public ActionResult InsertUpdateClientProfile(ClientProfileModel obj)
        {
            //ClientProfileModel obj = new ClientProfileModel();
            string Mode = obj.Mode;

            #region Check ClientProfile already exists
            ClientProfileRepository repo = new ClientProfileRepository();

            string client_code = obj.client_code;
            string retVal = repo.GetClientProfileById(client_code);
            List<ClientProfileModel> lst = JsonConvert.DeserializeObject<List<ClientProfileModel>>(retVal);

            bool IsClientProfileFound = false;
            if (lst.Count > 0)
            {
                for (int i = 0; i < lst.Count(); i++)
                {
                    if (obj.client_code == lst[i].client_code)
                    {
                        IsClientProfileFound = true;
                        break;
                    }
                }
            }
            #endregion Check ClientProfile already exists

            string data = "";
            string recordStatus = "";
            string jsondata = JsonConvert.SerializeObject(obj);

            if (obj.Mode == "Save") // Insert 
            {
                recordStatus = "save";
                if (IsClientProfileFound)
                {
                    data = "exists";
                }
                else
                {
                    data = repo.InsertOrUpdateClientProfile("INSERT", jsondata);
                    data = (data == "") ? "success" : "unsuccess";
                }
            }
            else // Update 
            {
                recordStatus = "update";
                if (IsClientProfileFound)
                {
                    data = repo.InsertOrUpdateClientProfile("UPDATE", jsondata);
                    data = (data == "") ? "success" : "unsuccess";
                }
                else
                {
                    data = "notexists";
                }
            }

            return Json(new BasicViewModel() { message = data, status = recordStatus });
        }
        public ActionResult Delete(string Value)
        {
            try
            {
                ClientProfileModel obj = new ClientProfileModel();
                string client_code = Value;

                ClientProfileRepository repo = new ClientProfileRepository();
                string retVal = repo.DeleteClientProfile(client_code);
                string retMesssage = (retVal == "" ? "Client Profile deleted successfully." : retVal);

                return Json(new BasicViewModel() { message = retMesssage, status = (retVal == "" ? "Success" : "Failure") });
            }
            catch
            {
                throw;
            }

        }

        public ActionResult Download()
        {
            string retMesssage = "";
            string retStatus = "";
            try
            {
                Logger.LogDebug("********* Download Client Profiles ********");

                ClientDetailRequest request = new ClientDetailRequest("01-03-2026", "07-03-2026", "", "", "");
                ReturnMessageWrapper<ClientDetailResponse> response = ClientDetail.ClientDetailReport(request);

                if (!response.HasError && response.StatusCode == 200)
                {
                    Logger.LogDebug($"response_status     : {response.Result.response_status}");
                    Logger.LogDebug($"Total Records       : {response.Result.report_data_total}");
                    retMesssage = $"Client Profile(s) downloaded. Total Records : {response.Result.report_data_total}";
                    retStatus = "Success";

                    string data = SaveClientProfile(response.Result.report_data);

                }
                else if (response.HasError && response.StatusCode == 403)   // 403 Forbidden
                {
                    Logger.LogDebug($"Status      : {response.Error.status}");
                    Logger.LogDebug($"Error_type  : {response.Error.error_type}");
                    Logger.LogDebug($"Message     : {response.Error.message}");
                    retMesssage = $"{response.Error.message}";
                    retStatus = "Failure";
                }
                else
                {
                    Logger.LogDebug($"Has Error     : {response.HasError}");
                    Logger.LogDebug($"Error remark  : {response.Result.error_remark}");
                    retMesssage = $"{response.Result.error_remark}";
                    retStatus = "Failure";
                }
                return Json(new BasicViewModel() { message = retMesssage, status = retStatus });
            }
            catch
            {
                throw;
            }

        }

        //Save the downloaded Client Profile to db
        private string SaveClientProfile(List<ClientProfileModel> report_data)
        {
            string data = "";
            Logger.LogDebug($"Records : {report_data.Count}");

            try
            {               
                if (report_data.Count > 0)
                {
                    string mode = "INSERT";
                    string jsonData = JsonConvert.SerializeObject(report_data[0]);

                    ClientProfileRepository repo = new ClientProfileRepository();
                data=    repo.InsertOrUpdateClientProfile(mode, jsonData);}
                data = (data == "") ? "success" : "unsuccess";
                return data;
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"Error in SaveClientProfile(): {ex.Message}");
                return ex.Message;
            }
        }
    }
}


