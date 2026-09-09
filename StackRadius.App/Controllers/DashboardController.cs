
using Microsoft.AspNetCore.Mvc;
using StackRadius.Entity;

namespace StackRadius.Controllers
{
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            DashboardModel dashboardModel = new DashboardModel();
            return View("Dashboard", dashboardModel);
        }
    }
}


