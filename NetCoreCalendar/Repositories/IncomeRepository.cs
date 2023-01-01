using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Constants;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;
using System.ComponentModel;
using System.Web.WebPages;
using System;
using System.Globalization;

namespace NetCoreCalendar.Repositories
{
    public class IncomeRepository : GenericRepository<Lesson>, IIncomeRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<Teacher> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IStudentRepository studentRepository;

        private DateTime startDate;
        private DateTime endDate;
        private DateTime today;
        private DateTime startYearDate;
        private DateTime endYearDate;

        public IncomeRepository(ApplicationDbContext context, IMapper mapper,
             UserManager<Teacher> userManager, IHttpContextAccessor httpContextAccessor,
             IStudentRepository studentRepository) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.studentRepository = studentRepository;
            startDate = DateTime.Today.AddDays(-6);
            endDate = DateTime.Today.AddDays(1);
            today = DateTime.Now;
            startYearDate = new DateTime(today.Year, Months.January, Months.FirstDayOfMonth);
            endYearDate = new DateTime(today.Year, Months.December, Months.LastDayOfMonth, 23, 59, 59);
        }

        public async Task<List<Lesson>> GetLessonsWithinRange(DateTime StartDate, DateTime EndDate)
        {
            var user = await studentRepository.GetUserRecords();
            var lessons = await context.Lessons
                .Where(q => q.RequestingUserId == user.Id)
                .Include(x => x.Student)
                .Where(y => y.Start >= StartDate && y.Start <= EndDate)
                .ToListAsync();
            return lessons;
        }

        public async Task<int> GetTotalIncome(List<Lesson> lessons)
        {
            decimal totalIncome = lessons
                .Sum(j => j.Rate);
            int result = Decimal.ToInt32(totalIncome);
            return result;
        }

        public async Task<int> GetIncomeForWeek()
        {
            var lessons = await GetLessonsWithinRange(startDate, endDate);
            var income = await GetTotalIncome(lessons);
            return income;
        }

        public async Task<int> GetIncomeForMonth()
        {
            // This Month
            DateTime startOfMonth = new DateTime(today.Year, today.Month, Months.FirstDayOfMonth, 0, 0, 0);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

            var lessons = await GetLessonsWithinRange(startOfMonth, endOfMonth);
            var income = await GetTotalIncome(lessons);
            return income;
        }

        public async Task<int> GetIncomeForSpecifiedMonth(int id)
        {
            DateTime startOfMonth = new DateTime(today.Year, id, Months.FirstDayOfMonth, 0, 0, 0);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

            var lessons = await GetLessonsWithinRange(startOfMonth, endOfMonth);
            var income = await GetTotalIncome(lessons);
            return income;
        }

        public async Task<List<CarouselDataBinding>> FillCarouselList()
        {
            List<CarouselDataBinding> carouselSource = new List<CarouselDataBinding>();
            System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
            for (int month=Months.January; month <= Months.December; month++)
            {
                carouselSource.Add(new CarouselDataBinding
                {
                    Id = month,
                    Title = mfi.GetMonthName(month).ToString(),
                    Income = await GetIncomeForSpecifiedMonth(month),
                    DoughnutChartData = await CreateLineChartForMonth(month)
                });
            }
            return carouselSource;
        }

        public async Task<int> GetIncomeForYear()
        {
            var lessons = await GetLessonsWithinRange(startYearDate, endYearDate);
            var income = await GetTotalIncome(lessons);
            return income;
        }

        public async Task<List<DoughnutChart>> CreateDoughnutChartForWeek()
        {
            var lessonsForWeek = await GetLessonsWithinRange(startDate, endDate);

            var lessons = lessonsForWeek
                .GroupBy(y => y.Student.Id)
                .Select(k => new DoughnutChart
                {
                    StudentName = k.First().Student.FirstName + " " + k.First().Student.LastName,
                    Income = Decimal.ToInt32(k.Sum(j => j.Rate)),
                    FormattedIncome = k.Sum(j => j.Rate).ToString("C0"),
                })
                .OrderByDescending(l => l.Income)
                .ToList();

            return lessons;
        }

        public async Task<List<DoughnutChart>> CreateLineChartForMonth(int id)
        {
            DateTime startOfMonth = new DateTime(today.Year, id, Months.FirstDayOfMonth, 0, 0, 0);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

            var lessonsForMonth = await GetLessonsWithinRange(startOfMonth, endOfMonth);

            var lessons = lessonsForMonth
                .GroupBy(y => y.Student.Id)
                .Select(k => new DoughnutChart
                {
                    StudentName = k.First().Student.FirstName + " " + k.First().Student.LastName,
                    Income = Decimal.ToInt32(k.Sum(j => j.Rate)),
                    FormattedIncome = k.Sum(j => j.Rate).ToString("C0"),
                })
                .OrderByDescending(l => l.Income)
                .ToList();
            return lessons;
        }
        public async Task<IEnumerable<SplineChart>> CreateSplineChartForWeek()
        {
            var lessonsForWeek = await GetLessonsWithinRange(startDate, endDate);

            var result = await CreateSplineChart(lessonsForWeek);
            string[] last7Days = Enumerable.Range(0, Months.DaysAWeek)
            // here we iterate 7 times and add a number of days from the range to the starting date
               .Select(i => startDate.AddDays(i).ToString("dd-MMM"))
               .ToArray();

            var splineChart = await CreateSplineChartForEachDay(last7Days, result);
            return splineChart;
        }

        public async Task<IEnumerable<SplineChart>> CreateSplineChartForYear()
        {
            var lessonsForYear = await GetLessonsWithinRange(startYearDate, endYearDate);
            var result = await CreateSplineChart(lessonsForYear);

            var thisYear = today.Year;
            Calendar myCalendar = new GregorianCalendar();
            int daysYear = myCalendar.GetDaysInYear(thisYear);
            int totalDaysYear = daysYear + 1;
            string[] daysInYear = Enumerable.Range(0, totalDaysYear)
               .Select(i => startYearDate.AddDays(i).ToString("dd-MMM"))
               .ToArray();

            var splineChart = await CreateSplineChartForEachDay(daysInYear, result);
                        return splineChart;

        }

        public async Task<List<SplineChart>> CreateSplineChart(List<Lesson> lessons)
        {
                    var income = lessons
                                .GroupBy(y => y.Student.Id)
                                .Select(k => new SplineChart()
                                {
                                    Day = k.First().Start.ToString("dd-MMM"),
                                    Income = Decimal.ToInt32(k.Sum(j => j.Rate))
                                })
                                .ToList();
                    return income;
        }

        public async Task<IEnumerable<SplineChart>> CreateSplineChartForEachDay(string[] days, List<SplineChart> result)
        {
            var splineChart = from day in days
                              join Income in result on day equals Income.Day
                              into dayIncomeJoined
                              from income in dayIncomeJoined.DefaultIfEmpty()
                              select new SplineChart
                              {
                                  Day = day,
                                  Income = income == null ? 0 : income.Income,
                              };
            return splineChart;
        }
    }
}
