using Microsoft.AspNetCore.Mvc;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;

namespace NetCoreCalendar.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IIncomeRepository incomeRepository;
        public DashboardController(ApplicationDbContext context, IIncomeRepository incomeRepository)
        {
            _context = context;
            this.incomeRepository = incomeRepository;
        }

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
            DateTime StartDate = DateTime.Today.AddDays(-6);

            string[] Last7Days = Enumerable.Range(0, 7)
                // here we iterate 7 times and add a number of days from the range to the starting date
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from day in Last7Days
                                      join Income in splineData on day equals Income.Day
                                      into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          Day = day,
                                          Income = income == null ? 0 : income.Income,
                                      };

            var carouselSource = await incomeRepository.FillCarouselList();
            ViewBag.CarouselData = carouselSource;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetMonthlyChart(int id)
        {
            var incomeSpecificMonth = await incomeRepository.GetIncomeForSpecifiedMonth(id);
            
            TempData["Income"] = incomeSpecificMonth.ToString("C0");
            return RedirectToAction(nameof(Index));

        }
    }
}
