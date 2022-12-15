using Microsoft.AspNetCore.Mvc;

namespace NetCoreCalendar.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
