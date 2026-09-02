
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackRadius.Common;
using StackRadius.Entity;
using StackRadius.Models;
using StackRadius.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace StackRadius.Controllers
{
    public class ClientController : Controller
    {
        public ActionResult Index()
        {
            ClientRepository repo = new ClientRepository();
            string retVal = repo.GetAllClients();
            List<ClientModel> lst = JsonConvert.DeserializeObject<List<ClientModel>>(retVal);
            return View("ClientViewGrid", lst);
        }

        public ActionResult Add()
        {
            ClientModel obj = new ClientModel();

            // Data to be initialized goes here
            obj.Mode = "Save";
            obj.ClientId = 3;
            obj.amcCode = "B";
            obj.panNo = "ER4567890";
            obj.mobileNo = "1234567890";
            obj.invEmail = "raja@yahoo.com";
            obj.AMCCodeList = Common.Common.GetAMCCodeList();

            return View("ClientAdd", obj);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            try
            {
                ClientRepository repo = new ClientRepository();
                string retVal = repo.GetClientsById((int)id);
                List<ClientModel> lst = JsonConvert.DeserializeObject<List<ClientModel>>(retVal);

                ClientModel obj = new ClientModel();
                obj = lst.FirstOrDefault();
                obj.Mode = "Edit";
                obj.AMCCodeList = Common.Common.GetAMCCodeList();
                return View("ClientAdd", obj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// InsertUpdateClient
        /// This method is called from the View page using ajax call
        /// </summary>
        /// <param name="clientModelObj"></param>
        /// <returns>new BasicViewModel() object </returns>
        public ActionResult InsertUpdateClient(ClientModel clientModelObj)
        {
            ClientModel obj = new ClientModel();
            obj.Mode = clientModelObj.Mode;
            obj.ClientId = clientModelObj.ClientId;
            obj.amcCode = clientModelObj.amcCode;
            obj.panNo = clientModelObj.panNo;
            obj.mobileNo = clientModelObj.mobileNo;
            obj.invEmail = clientModelObj.invEmail;

            #region Check Client already exists
            ClientRepository repo = new ClientRepository();
            string retVal = repo.GetClientsById((int)clientModelObj.ClientId);
            List<ClientModel> lst = JsonConvert.DeserializeObject<List<ClientModel>>(retVal);

            //Logger.LogDebug("Make Request: " + request.Method + " " + EndPoint + parameters);

            bool IsClientFound = false;
            if (lst.Count > 0)
            {
                for (int i = 0; i < lst.Count(); i++)
                {
                    if (obj.ClientId == lst[i].ClientId)
                    {
                        IsClientFound = true;
                        break;
                    }
                }
            }
            #endregion Check Client already exists

            string data = "";
            string recordStatus = "";
            string jsondata = JsonConvert.SerializeObject(obj);

            if (obj.Mode == "Save") // Insert 
            {
                recordStatus = "save";
                if (IsClientFound)
                {
                    data = "exists";
                }
                else
                {
                    data = repo.InsertOrUpdateClient(jsondata);
                    data = (data == "") ? "success" : "unsuccess";
                }
            }
            else // Update 
            {
                recordStatus = "update";
                if (IsClientFound)
                {
                    data = repo.InsertOrUpdateClient(jsondata);
                    data = (data == "") ? "success" : "unsuccess";
                }
                else
                {
                    data = "notexists";
                }
            }

            return Json(new BasicViewModel() { message = data, status = recordStatus });
        }

        public ActionResult Delete(int? Value)
        {
            try
            {
                ClientModel obj = new ClientModel();
                obj.ClientId = Value;

                ClientRepository repo = new ClientRepository();
                string jsondata = JsonConvert.SerializeObject(obj);
                string retVal = repo.DeleteClient(jsondata);
                string retMesssage = (retVal == "" ? "Client deleted successfully." : retVal);
                return Json(new BasicViewModel() { message = retMesssage, status = (retVal == "" ? "Success" : "Failure") });
            }
            catch
            {
                throw;
            }
        }

            //public ActionResult Delete(int clientId)
            //{
            //    ClientModel obj = new ClientModel();
            //    obj.ClientId = clientId;
            //    string jsondata = JsonConvert.SerializeObject(obj);
            //    ClientRepository repo = new ClientRepository();
            //    string retVal = repo.DeleteClient(jsondata);

            //    // Redirect to an action in the same controller
            //    return RedirectToAction("Index");
            //}
        }
    }


