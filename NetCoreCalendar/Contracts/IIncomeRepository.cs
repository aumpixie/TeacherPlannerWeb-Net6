using NetCoreCalendar.Data;

namespace NetCoreCalendar.Contracts
{
    public interface IIncomeRepository : IGenericRepository<Lesson>
    {
        Task<List<Lesson>> GetLessonsWithinRange(DateTime StartDate, DateTime EndDate);
        Task<int> GetTotalIncome(List<Lesson> lessons);
        Task<int> GetIncomeForWeek();
        Task<int> GetIncomeForMonth();
        Task<int> GetIncomeForYear();
        Task<List<DoughnutChart>> CreateDoughnutChartForWeek();

        Task<List<SplineChart>> CreateSplineChartForWeek();

        Task<List<CarouselDataBinding>> FillCarouselList();
        Task<int> GetIncomeForSpecifiedMonth(int id);
        Task<List<DoughnutChart>> CreateDoughnutChartForMonth(int id);
    }
}
