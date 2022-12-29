using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;
using System.ComponentModel;

namespace NetCoreCalendar.Repositories
{
    public class IncomeRepository : GenericRepository<Lesson>, IIncomeRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<Teacher> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IStudentRepository studentRepository;

        public DateTime startDate;
        public DateTime endDate;

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
            // Total Income
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
            DateTime today = DateTime.Now;
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

            var lessons = await GetLessonsWithinRange(startOfMonth, endOfMonth);
            var income = await GetTotalIncome(lessons);
            return income;
        }

        public async Task<int> GetIncomeForSpecifiedMonth(int id)
        {
            //var carouselList = await FillCarouselList();

            //var element = carouselList[id];
            //int month = element.Id;
            DateTime today = DateTime.Now;
            DateTime startOfMonth = new DateTime(today.Year, id, 1, 0, 0, 0);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

            var lessons = await GetLessonsWithinRange(startOfMonth, endOfMonth);
            var income = await GetTotalIncome(lessons);
            return income;

        }

        public async Task<List<CarouselDataBinding>> FillCarouselList()
        {
            List<CarouselDataBinding> carouselSource = new List<CarouselDataBinding>();
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 1,
                Title = "January",
                Income = await GetIncomeForSpecifiedMonth(1),
                DoughnutChartData = await CreateDoughnutChartForMonth(1)
            }); 
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 2,
                Title = "February",
                Income = await GetIncomeForSpecifiedMonth(2),
                DoughnutChartData = await CreateDoughnutChartForMonth(2)
            });
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 3,
                Title = "March",
                Income = await GetIncomeForSpecifiedMonth(3),
                DoughnutChartData = await CreateDoughnutChartForMonth(3)
            });
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 4,
                Title = "April",
                Income = await GetIncomeForSpecifiedMonth(4),
                DoughnutChartData = await CreateDoughnutChartForMonth(4)
            });
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 5,
                Title = "May",
                Income = await GetIncomeForSpecifiedMonth(5),
                DoughnutChartData = await CreateDoughnutChartForMonth(5)
            });
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 6,
                Title = "June",
                Income = await GetIncomeForSpecifiedMonth(6),
                DoughnutChartData = await CreateDoughnutChartForMonth(6)
            });
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 7,
                Title = "July",
                Income = await GetIncomeForSpecifiedMonth(7),
                DoughnutChartData = await CreateDoughnutChartForMonth(7)
            });
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 8,
                Title = "August",
                Income = await GetIncomeForSpecifiedMonth(8),
                DoughnutChartData = await CreateDoughnutChartForMonth(8)
            });
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 9,
                Title = "September",
                Income = await GetIncomeForSpecifiedMonth(9),
                DoughnutChartData = await CreateDoughnutChartForMonth(9)
            });
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 10,
                Title = "October",
                Income = await GetIncomeForSpecifiedMonth(10),
                DoughnutChartData = await CreateDoughnutChartForMonth(10)
            });
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 11,
                Title = "November",
                Income = await GetIncomeForSpecifiedMonth(11),
                DoughnutChartData = await CreateDoughnutChartForMonth(11)
            });
            carouselSource.Add(new CarouselDataBinding
            {
                Id = 12,
                Title = "December",
                Income = await GetIncomeForSpecifiedMonth(12),
                DoughnutChartData = await CreateDoughnutChartForMonth(12)
            });

            return carouselSource;
        }

        public async Task<int> GetIncomeForYear()
        {
            // This Year
            DateTime today = DateTime.Now;
            DateTime startYearDate = new DateTime(today.Year, 1, 1);
            DateTime endYearDate = new DateTime(today.Year, 12, 31);

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
                    StudentName = k.First().Student.FirstName + k.First().Student.LastName,
                    Income = Decimal.ToInt32(k.Sum(j => j.Rate)),
                    FormattedIncome = k.Sum(j => j.Rate).ToString("C0"),
                })
                .OrderByDescending(l => l.Income)
                .ToList();

            return lessons;
        }

        public async Task<List<DoughnutChart>> CreateDoughnutChartForMonth(int id)
        {
            DateTime today = DateTime.Now;
            DateTime startOfMonth = new DateTime(today.Year, id, 1, 0, 0, 0);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

            var lessonsForMonth = await GetLessonsWithinRange(startOfMonth, endOfMonth);

            var lessons = lessonsForMonth
                .GroupBy(y => y.Student.Id)
                .Select(k => new DoughnutChart
                {
                    StudentName = k.First().Student.FirstName + k.First().Student.LastName,
                    Income = Decimal.ToInt32(k.Sum(j => j.Rate)),
                    FormattedIncome = k.Sum(j => j.Rate).ToString("C0"),
                })
                .OrderByDescending(l => l.Income)
                .ToList();
            return lessons;
        }
        public async Task<List<SplineChart>> CreateSplineChartForWeek()
        {
            var lessonsForWeek = await GetLessonsWithinRange(startDate, endDate);

            var income = lessonsForWeek
                .GroupBy(y => y.Student.Id)
                .Select(k => new SplineChart()
                {
                    Day = k.First().Start.ToString("dd-MMM"),
                    Income = Decimal.ToInt32(k.Sum(j => j.Rate))
                })
                .ToList();
            return income;
        }
    }
}
