
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackRadius.Entity;
using StackRadius.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StackRadius.App.Areas.Dashboard.Controllers
{
    //Tiles = new StatTileVm[]
    //    {
    //                new (Label: "Schemes",          Value: "13,393", Sub: "across 66 SEBI-registered AMCs", IconId: "i-book",           Tone: "indigo"),
    //                new (Label: "SIP-eligible",     Value: "12,182", Sub: "91% of the universe",            IconId: "i-refresh",        Tone: "emerald"),
    //                new (Label: "Purchase-active",  Value: "11,116", Sub: "83% currently accepting orders", IconId: "i-check-circle",   Tone: "blue"),
    //                new (Label: "AMCs",             Value: "66",     Sub: "SEBI-registered mutual funds",   IconId: "i-building",       Tone: "purple"),
    //                new (Label: "Scheme types",     Value: "5",      Sub: "Debt · Equity · ETF · Liquid · Overnight", IconId: "i-layers", Tone: "amber"),
    //                new (Label: "Settlement cycles", Value: "10",    Sub: "L0 · L1 · MF · T1..T7",          IconId: "i-file-chart",     Tone: "sky"),
    //    };

    [Area("Dashboard")]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            DashboardModel dashboardModel = new DashboardModel();
            DashboardRepository repo = new DashboardRepository();

            try
            {
                string jsonData = "{\"Mode\": \"ALL\"}";
                string retVal = repo.GetDahboard(jsonData);
                List<DashboardModel> lst = JsonConvert.DeserializeObject<List<DashboardModel>>(retVal);
                dashboardModel = lst.FirstOrDefault();
            }
            catch (Exception ex)
            {
                //Logger.LogDebug($"Error in DashboardController->Index(): {ex.Message}");
            }

            return View("Dashboard", dashboardModel);
        }
    }
}


