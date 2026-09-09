
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
    public class NSEClientRegistrationController : Controller
    {
        public ActionResult Index()
        {
            NSEClientRegistrationModel obj = new NSEClientRegistrationModel();
            obj.reg_details = "gfggdgds";
            obj.client_code = "Client 1";
            obj.primary_holder_first_name = "Muthukumar";

            return View("NSEClientRegistrationDetail", obj);

            //NSEClientRegistrationRepository repo = new NSEClientRegistrationRepository();
            //string retVal = repo.GetClient();
            //List<NSEClientRegistrationModel> lst = JsonConvert.DeserializeObject<List<NSEClientRegistrationModel>>(retVal);
            //return View("NSEClientRegistrationViewGrid", lst);
        }

        public ActionResult Edit(string mode, int? id)
        {
            try
            {
                NSEClientRegistrationModel obj = new NSEClientRegistrationModel();
                NSEClientRegistrationRepository repo = new NSEClientRegistrationRepository();
                string retVal = repo.GetClientById(id??0);
                List<NSEClientRegistrationModel> lst = JsonConvert.DeserializeObject<List<NSEClientRegistrationModel>>(retVal);
                obj = lst.FirstOrDefault();
                return View("NSEClientRegistrationDetail", obj);
            }
            catch (Exception)
            {
                throw;
            }
        }       
    }
}


