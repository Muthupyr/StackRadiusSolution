
using Microsoft.AspNetCore.Mvc;
using StackRadius.Entity;

namespace StackRadius.App.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            DashboardModel dashboardModel = new DashboardModel();
            return View("Dashboard", dashboardModel);
        }
    }
}


