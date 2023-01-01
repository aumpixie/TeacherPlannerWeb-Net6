using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;

namespace NetCoreCalendar.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IIncomeRepository incomeRepository;
        public DashboardController(ApplicationDbContext context, IIncomeRepository incomeRepository)
        {
            _context = context;
            this.incomeRepository = incomeRepository;
        }

        // GET: Dashboard
        /**
         * Fills the ViewBags with the corresponding data, 
         * which we will pass to the charts for the visual representation
         **/
        public async Task<IActionResult> Index()
        {
            var incomeWeek = await incomeRepository.GetIncomeForWeek();
            var incomeMonth = await incomeRepository.GetIncomeForMonth();
            var incomeYear = await incomeRepository.GetIncomeForYear();
            ViewBag.TotalIncomeWeek = incomeWeek.ToString("C0");
            ViewBag.TotalIncomeMonth = incomeMonth.ToString("C0");
            ViewBag.TotalIncomeYear = incomeYear.ToString("C0");

            var doughnutData = await incomeRepository.CreateDoughnutChartForWeek();
            ViewBag.DoughnutChartData = doughnutData;

            var splineData = await incomeRepository.CreateSplineChartForWeek();

            ViewBag.SplineChartData = splineData;

            var splineDataYear = await incomeRepository.CreateSplineChartForYear();
            ViewBag.SplineChartDataYear = splineDataYear;

            var carouselSource = await incomeRepository.FillCarouselList();
            ViewBag.CarouselData = carouselSource;

            return View();
        }
    }
}
