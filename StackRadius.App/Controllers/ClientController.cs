
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackRadius.Common;
using StackRadius.Entity;
using StackRadius.Models;
using StackRadius.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StackRadius.Controllers
{
    public class ClientController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                int RecordFrom = 1, RecordTo = 10;
                string SortDir = "";
                ClientModel clientObj = new ClientModel();
                clientObj.Mode = "ALL";
                clientObj.RecordFrom = RecordFrom;
                clientObj.RecordTo = RecordTo;
                clientObj.SortDir = SortDir;
                clientObj.SortKey = "clientId";
                return View("ClientSearch", clientObj);
            }
            catch
            {
                throw;
            }
        }

        public ActionResult GridPartial()
        {
            return View("ClientGrid");
        }

        public ActionResult GetClientList(ClientModel ClientModelObj)
        {
            int RecordFrom = 0, RecordTo = 10;
            string SortKey = "", SortDir = "";
            var t = Request.Form;

            string draw = Request.Form["draw"].ToString(); // Returns an empty string "" if "draw" is missing, or the value if it exists
            var start = Request.Form["start"].ToString();
            var length = Request.Form["length"].ToString();
            List<Tuple<int, string>> ColumnHeader = new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(0,"clientId"),
                new Tuple<int, string>(1,"clientName"),
                new Tuple<int, string>(2,"amcCode"),
                new Tuple<int, string>(3,"panNo"),
                new Tuple<int, string>(4,"mobileNo"),
                new Tuple<int, string>(5,"invEmail"),
            };
            int column = Convert.ToInt16(Request.Form["order[0][column]"]);
            string dir = Request.Form["order[0][dir]"];

            if (dir != null && column >= 0)
            {
                SortKey = ColumnHeader.Where(a => a.Item1 == column).Select(b => b.Item2).FirstOrDefault();
                SortDir = dir == "desc" ? "Desc" : "Asc";
            }

            //string search = Request.Query["search[value]"].ToString();

            string txtClientName = Request.Query["clientName"].ToString();
            string condition = "";

            if (!string.IsNullOrEmpty(txtClientName))
            {
                condition = "clientname ILIKE '%" + txtClientName + "%'";
            }

            RecordFrom = start != null ? Convert.ToInt32(start) + 1 : 0;
            RecordTo = length != null ? Convert.ToInt32(length) + RecordFrom - 1 : 0;

            var clientobj = new ClientModel
            {
                Mode = "ALL",
                RecordFrom = RecordFrom,
                RecordTo = RecordTo,
                SortKey = SortKey != "" ? SortKey : "clientId",
                SortDir = SortDir != "" ? SortDir : "DESC",
                FilterCondition = condition
            };

            ClientRepository repo = new ClientRepository();
            string jsonData = JsonConvert.SerializeObject(clientobj);
            string retVal = repo.GetClients(jsonData);
            List<ClientModel> lst = JsonConvert.DeserializeObject<List<ClientModel>>(retVal);
            DataTable<ClientModel> obj = new DataTable<ClientModel>();
            if (lst.Count > 0)
            {
                obj.draw = draw;
                obj.recordsTotal = lst[0].TotalRowCount;
                obj.recordsFiltered = lst[0].TotalRowCount;
                obj.data = lst;
            }
            else
            {
                obj.draw = draw;
                obj.recordsTotal = 0;
                obj.recordsFiltered = 0;
                obj.data = lst;
            }
            return Json(obj);
        }

        public ActionResult GetAllClients()
        {
            ClientRepository repo = new ClientRepository();
            var clientobj = new ClientModel
            {
                Mode = "ALL",
                RecordFrom = 1,
                RecordTo = 1000,
                SortKey = "clientId",
                SortDir = "DESC"
            };

            string jsonData = JsonConvert.SerializeObject(clientobj);
            string retVal = repo.GetClients(jsonData);
            List<ClientModel> lst = JsonConvert.DeserializeObject<List<ClientModel>>(retVal);
            return View("ClientGrid", lst);
        }

        public ActionResult Add()
        {
            ClientModel obj = new ClientModel();

            // Data to be initialized goes here
            obj.Mode = "Save";
            obj.clientId = 1;
            obj.clientName = "";
            obj.amcCode = "B";
            obj.panNo = "";
            obj.mobileNo = "";
            obj.invEmail = "";
            obj.AMCCodeList = Common.Common.GetAMCCodeList();

            return PartialView("ClientAdd", obj);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? Id, string Mode)
        {
            try
            {
                ClientRepository repo = new ClientRepository();
                string retVal = repo.GetClientsById(Id ?? 0);
                List<ClientModel> lst = JsonConvert.DeserializeObject<List<ClientModel>>(retVal);

                ClientModel obj = new ClientModel();
                obj = lst.FirstOrDefault();
                obj.Mode = "Edit";
                obj.AMCCodeList = Common.Common.GetAMCCodeList();
                return PartialView("ClientAdd", obj);
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
            obj.clientId = clientModelObj.clientId;
            obj.clientName = clientModelObj.clientName;
            obj.amcCode = clientModelObj.amcCode;
            obj.panNo = clientModelObj.panNo;
            obj.mobileNo = clientModelObj.mobileNo;
            obj.invEmail = clientModelObj.invEmail;

            #region Check Client already exists
            ClientRepository repo = new ClientRepository();
            string retVal = repo.GetClientsById((int)clientModelObj.clientId);
            List<ClientModel> lst = JsonConvert.DeserializeObject<List<ClientModel>>(retVal);

            //Logger.LogDebug("Make Request: " + request.Method + " " + EndPoint + parameters);

            bool IsClientFound = false;
            if (lst.Count > 0)
            {
                for (int i = 0; i < lst.Count(); i++)
                {
                    if (obj.clientId == lst[i].clientId)
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
                obj.clientId = Value;

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


    }
}


