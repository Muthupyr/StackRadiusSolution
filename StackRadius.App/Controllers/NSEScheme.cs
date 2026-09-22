
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
    public class NSESchemeController : Controller
    {
        public ActionResult Index()
        {
            NSESchemeMasterRepository repo = new NSESchemeMasterRepository();
            string retVal = repo.GetAllSchemeMaster();
            List<NSESchemeMasterModel> lst = JsonConvert.DeserializeObject<List<NSESchemeMasterModel>>(retVal);
            return View("NSESchemeViewGrid", lst);
        }
    }
}


