using Microsoft.AspNetCore.Mvc;

namespace NetCoreCalendar.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
