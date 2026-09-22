
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

namespace StackRadius.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MasterUserController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                int RecordFrom = 1, RecordTo = 10;
                string SortDir = "";
                MasterUserModel clientObj = new MasterUserModel();
                clientObj.Mode = "ALL";
                clientObj.RecordFrom = RecordFrom;
                clientObj.RecordTo = RecordTo;
                clientObj.SortDir = SortDir;
                clientObj.SortKey = "masterUserId";
                return View("MasterUserSearch", clientObj);
            }
            catch
            {
                throw;
            }
        }

        public ActionResult GridPartial()
        {
            return View("MasterUserGrid");
        }

        public ActionResult GetMasterUserList(MasterUserModel MasterUserModelObj)
        {
            int RecordFrom = 0, RecordTo = 10;
            string SortKey = "", SortDir = "";
            var t = Request.Form;

            string draw = Request.Form["draw"].ToString(); // Returns an empty string "" if "draw" is missing, or the value if it exists
            var start = Request.Form["start"].ToString();
            var length = Request.Form["length"].ToString();
            List<Tuple<int, string>> ColumnHeader = new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(0,"MasterUserId"),
                new Tuple<int, string>(1,"UserName"),
                //new Tuple<int, string>(2,"amcCode"),
                //new Tuple<int, string>(3,"panNo"),
                //new Tuple<int, string>(4,"mobileNo"),
                //new Tuple<int, string>(5,"invEmail"),
            };
            int column = Convert.ToInt16(Request.Form["order[0][column]"]);
            string dir = Request.Form["order[0][dir]"];

            if (dir != null && column >= 0)
            {
                SortKey = ColumnHeader.Where(a => a.Item1 == column).Select(b => b.Item2).FirstOrDefault();
                SortDir = dir == "desc" ? "Desc" : "Asc";
            }

            //string search = Request.Query["search[value]"].ToString();

            string txtUserName = Request.Query["UserName"].ToString();
            string condition = "";

            if (!string.IsNullOrEmpty(txtUserName))
            {
                condition = "username ILIKE '%" + txtUserName + "%'";
            }

            RecordFrom = start != null ? Convert.ToInt32(start) + 1 : 0;
            RecordTo = length != null ? Convert.ToInt32(length) + RecordFrom - 1 : 0;

            var clientobj = new MasterUserModel
            {
                Mode = "ALL",
                RecordFrom = RecordFrom,
                RecordTo = RecordTo,
                SortKey = SortKey != "" ? SortKey : "masterUserId",
                SortDir = SortDir != "" ? SortDir : "DESC",
                FilterCondition = condition
            };

            MasterUserRepository repo = new MasterUserRepository();
            string jsonData = JsonConvert.SerializeObject(clientobj);
            string retVal = repo.GetMasterUsers(jsonData);
            List<MasterUserModel> lst = JsonConvert.DeserializeObject<List<MasterUserModel>>(retVal);
            DataTable<MasterUserModel> obj = new DataTable<MasterUserModel>();
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

        public ActionResult GetAllMasterUsers()
        {
            MasterUserRepository repo = new MasterUserRepository();
            var clientobj = new MasterUserModel
            {
                Mode = "ALL",
                RecordFrom = 1,
                RecordTo = 1000,
                SortKey = "masterUserId",
                SortDir = "DESC"
            };

            string jsonData = JsonConvert.SerializeObject(clientobj);
            string retVal = repo.GetMasterUsers(jsonData);
            List<MasterUserModel> lst = JsonConvert.DeserializeObject<List<MasterUserModel>>(retVal);
            return View("MasterUserGrid", lst);
        }

        public ActionResult Add()
        {
            MasterUserModel obj = new MasterUserModel();

            // Data to be initialized goes here
            obj.Mode = "Save";
            
            //obj.mobileNo = "";
            //obj.invEmail = "";
            //obj.AMCCodeList = Common.Common.GetAMCCodeList();

            return PartialView("MasterUserAdd", obj);
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
                MasterUserRepository repo = new MasterUserRepository();
                //string retVal = repo.GetMasterUsersById(Id ?? 0);
                //List<MasterUserModel> lst = JsonConvert.DeserializeObject<List<MasterUserModel>>(retVal);

                MasterUserModel obj = new MasterUserModel();
                //obj = lst.FirstOrDefault();
                //obj.Mode = "Edit";
                //obj.AMCCodeList = Common.Common.GetAMCCodeList();
                return PartialView("MasterUserAdd", obj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// InsertUpdateMasterUser
        /// This method is called from the View page using ajax call
        /// </summary>
        /// <param name="clientModelObj"></param>
        /// <returns>new BasicViewModel() object </returns>
        public ActionResult InsertUpdateMasterUser(MasterUserModel obj)
        {
            obj.Mode = obj.Mode;          

            #region Check MasterUser already exists
            MasterUserRepository repo = new MasterUserRepository();
            string retVal = repo.GetMasterUserById((int)obj.MasterUserId);
            List<MasterUserModel> lst = JsonConvert.DeserializeObject<List<MasterUserModel>>(retVal);

            bool IsMasterUserFound = false;
            if (lst.Count > 0)
            {
                for (int i = 0; i < lst.Count(); i++)
                {
                    if (obj.MasterUserId == lst[i].MasterUserId)
                    {
                        IsMasterUserFound = true;
                        break;
                    }
                }
            }
            #endregion Check MasterUser already exists

            string data = "";
            string recordStatus = "";
            string jsondata = JsonConvert.SerializeObject(obj);

            if (obj.Mode == "Save") // Insert 
            {
                recordStatus = "save";
                if (IsMasterUserFound)
                {
                    data = "exists";
                }
                else
                {
                    data = repo.InsertUpdateMasterUsers(jsondata);
                    data = (data == "") ? "success" : "unsuccess";
                }
            }
            else // Update 
            {
                recordStatus = "update";
                if (IsMasterUserFound)
                {
                    data = repo.InsertUpdateMasterUsers(jsondata);
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
                MasterUserModel obj = new MasterUserModel();
                obj.MasterUserId = Value;

                MasterUserRepository repo = new MasterUserRepository();
                string jsondata = JsonConvert.SerializeObject(obj);
                string retVal = repo.DeleteMasterUsers(jsondata);
                string retMesssage = (retVal == "" ? "User deleted successfully." : retVal);
                return Json(new BasicViewModel() { message = retMesssage, status = (retVal == "" ? "Success" : "Failure") });
            }
            catch
            {
                throw;
            }
        }
    }
}


