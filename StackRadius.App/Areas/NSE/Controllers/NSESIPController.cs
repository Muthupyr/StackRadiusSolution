
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

namespace StackRadius.App.Areas.NSE.Controllers
{
    [Area("NSE")]
    public class NSESIPController : Controller
    {
        public ActionResult Index()
        {
            NSESIPMasterRepository repo = new NSESIPMasterRepository();
            string retVal = repo.GetAllSIPMaster();
            List<NSESIPMasterModel> lst = JsonConvert.DeserializeObject<List<NSESIPMasterModel>>(retVal);
            return View("NSESipViewGrid", lst);
        }
    }
}


