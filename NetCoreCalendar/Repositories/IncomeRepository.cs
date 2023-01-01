using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Constants;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;
using System.Globalization;

namespace NetCoreCalendar.Repositories
{
    public class IncomeRepository : GenericRepository<Lesson>, IIncomeRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IStudentRepository studentRepository;

        private readonly DateTime startDate;
        private readonly DateTime endDate;
        private readonly DateTime today;
        private readonly DateTime startYearDate;
        private readonly DateTime endYearDate;

        private DateTime startOfMonth;
        private DateTime endOfMonth;

        public IncomeRepository(ApplicationDbContext context,
             IStudentRepository studentRepository) : base(context)
        {
            this.context = context;
            this.studentRepository = studentRepository;

            // The Start Of the Week (7 Days ago)
            startDate = DateTime.Today.AddDays(Months.DaysAWeekToSubtract);
            endDate = DateTime.Today.AddDays(Months.OneDay);

            today = DateTime.Now;
            // This Year
            startYearDate = new DateTime(today.Year, Months.January, Months.FirstDayOfMonth);
            endYearDate = new DateTime(today.Year, Months.December, Months.LastDayOfMonth,
                Months.MaxHourADay, Months.MaxMinutesAnHour, Months.MaxSecondsAMinute);
            // This Month
            startOfMonth = new DateTime(today.Year, today.Month, Months.FirstDayOfMonth, 0, 0, 0);
            endOfMonth = startOfMonth.AddMonths(Months.OneMonth).AddSeconds(Months.MinSeconds);
        }

        /**
         * Finds the Lessons in the database that fall into the corressponding timeframe
         **/
        public async Task<List<Lesson>> GetLessonsWithinRange(DateTime StartDate, DateTime EndDate)
        {
            var user = await studentRepository.GetUserRecords();
            var lessons = await context.Lessons
                .Where(q => q.RequestingUserId == user.Id) // take only the lessons of the user that is currently logged in
                .Include(x => x.Student) // include the Student table, as some of the fields a
                .Where(y => y.Start >= StartDate && y.Start <= EndDate)
                .ToListAsync();
            return lessons;
        }

        /**
         * Calculates the total of the given lessons' rates 
         **/
        public int GetTotalIncome(List<Lesson> lessons)
        {
            decimal totalIncome = lessons
                .Sum(j => j.Rate);
            int result = Decimal.ToInt32(totalIncome);
            return result;
        }

        /**
        * Calculates the total of the lessons' rates in the last 7 Days
        **/
        public async Task<int> GetIncomeForWeek()
        {
            var lessons = await GetLessonsWithinRange(startDate, endDate);
            var income = GetTotalIncome(lessons);
            return income;
        }

        /**
        * Calculates the total of the lessons' rates in the current month
        **/
        public async Task<int> GetIncomeForMonth()
        {
            var lessons = await GetLessonsWithinRange(startOfMonth, endOfMonth);
            var income = GetTotalIncome(lessons);
            return income;
        }

        /**
        * Calculates the total of the lessons' rates in the indicated month
        **/
        public async Task<int> GetIncomeForSpecifiedMonth(int id)
        {
            startOfMonth = new DateTime(today.Year, id, Months.FirstDayOfMonth, 0, 0, 0);
            endOfMonth = startOfMonth.AddMonths(Months.OneMonth).AddSeconds(Months.MinSeconds);

            var lessons = await GetLessonsWithinRange(startOfMonth, endOfMonth);
            var income = GetTotalIncome(lessons);
            return income;
        }

        /**
        * Calculates the total of the lessons' rates in the current year
        **/
        public async Task<int> GetIncomeForYear()
        {
            var lessons = await GetLessonsWithinRange(startYearDate, endYearDate);
            var income = GetTotalIncome(lessons);
            return income;
        }

        /**
         * Fills the List with Carousel Objects that we fill with the information about the lessons
         * for each month in a year. We pass this List to the ViewBag that we use in our Dashboard Index View
         **/
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

        /**
        * Fills the List with DoughnutChart Objects that we fill with the information about the lessons
        * in the last 7 Days. We pass this List to the ViewBag that we use in our Dashboard Index View
        **/
        public async Task<List<DoughnutChart>> CreateDoughnutChartForWeek()
        {
            var lessonsForWeek = await GetLessonsWithinRange(startDate, endDate);

            var lessons = lessonsForWeek
                .GroupBy(y => y.Student?.Id)
                .Select(k => new DoughnutChart
                {
                    StudentName = k.First().Student?.FirstName + " " + k.First().Student?.LastName,
                    Income = Decimal.ToInt32(k.Sum(j => j.Rate)),
                    FormattedIncome = k.Sum(j => j.Rate).ToString("C0"),
                })
                .OrderByDescending(l => l.Income)
                .ToList();

            return lessons;
        }

        /**
        * Fills the List with DoughnutChart Objects that we fill with the information about the lessons
        * for the indicated month. We pass this List to the ViewBag that we use in our Dashboard Index View.
        * We use this List as a Source for the LineChart, because they have the same properties
        **/
        public async Task<List<DoughnutChart>> CreateLineChartForMonth(int id)
        {
            startOfMonth = new DateTime(today.Year, id, Months.FirstDayOfMonth, 0, 0, 0);
            endOfMonth = startOfMonth.AddMonths(Months.OneMonth).AddSeconds(Months.MinSeconds);

            var lessonsForMonth = await GetLessonsWithinRange(startOfMonth, endOfMonth);

            var lessons = lessonsForMonth
                .GroupBy(y => y.Student?.Id)
                .Select(k => new DoughnutChart
                {
                    StudentName = k.First().Student?.FirstName + " " + k.First().Student?.LastName,
                    Income = Decimal.ToInt32(k.Sum(j => j.Rate)),
                    FormattedIncome = k.Sum(j => j.Rate).ToString("C0"),
                })
                .OrderByDescending(l => l.Income)
                .ToList();
            return lessons;
        }

        /**
        * Creates IEnumerable with SplineChart Objects that we fill with the information about the lessons
        * in the last 7 Days. We pass it later to the ViewBag that we use in our Dashboard Index View.
        * We use array, so that all of days in the week will be visible in our chart,
        * even if the income was 0 that day
        **/
        public async Task<IEnumerable<SplineChart>> CreateSplineChartForWeek()
        {
            var lessonsForWeek = await GetLessonsWithinRange(startDate, endDate);

            var result = CreateSplineChart(lessonsForWeek);
            string[] last7Days = Enumerable.Range(0, Months.DaysAWeek)
            // here we iterate 7 times and add a number of days from the range to the starting date
               .Select(i => startDate.AddDays(i).ToString("dd-MMM"))
               .ToArray();

            var splineChart = CreateSplineChartForEachDay(last7Days, result);
            return splineChart;
        }

        /**
       * Creates IEnumerable with SplineChart Objects that we fill with the information about the lessons
       * in the current year. We pass it later to the ViewBag that we use in our Dashboard Index View.
       * We use array, so that all of days in the year will be visible in our chart,
       * even if the income was 0 that day
       **/
        public async Task<IEnumerable<SplineChart>> CreateSplineChartForYear()
        {
            var lessonsForYear = await GetLessonsWithinRange(startYearDate, endYearDate);
            var result = CreateSplineChart(lessonsForYear);

            var thisYear = today.Year;
            Calendar myCalendar = new GregorianCalendar();
            int daysYear = myCalendar.GetDaysInYear(thisYear);
            string[] daysInYear = Enumerable.Range(0, daysYear)
               .Select(i => startYearDate.AddDays(i).ToString("dd-MMM"))
               .ToArray();

            var splineChart = CreateSplineChartForEachDay(daysInYear, result);
            return splineChart;
        }

        /**
         * Creates a List of SplineChart Objects from the given List of Lesson objects
         **/
        public List<SplineChart> CreateSplineChart(List<Lesson> lessons)
        {
            var income = lessons
                        .GroupBy(y => y.Start.Date)
                        .Select(k => new SplineChart()
                        {
                            Day = k.First().Start.ToString("dd-MMM"),
                            Income = Decimal.ToInt32(k.Sum(j => j.Rate))
                        })
                        .ToList();
          return income;
        }

        /**
         * We combine the array and the List, so that we have the IEnumerable of all days of the year
         * and the corresponding income that the user earned on each day
         **/
        public IEnumerable<SplineChart> CreateSplineChartForEachDay(string[] days, List<SplineChart> result)
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
