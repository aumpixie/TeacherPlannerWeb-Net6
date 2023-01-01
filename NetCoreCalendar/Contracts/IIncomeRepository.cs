using NetCoreCalendar.Data;

namespace NetCoreCalendar.Contracts
{
    public interface IIncomeRepository : IGenericRepository<Lesson>
    {
        Task<List<Lesson>> GetLessonsWithinRange(DateTime StartDate, DateTime EndDate);
        int GetTotalIncome(List<Lesson> lessons);
        Task<int> GetIncomeForWeek();
        Task<int> GetIncomeForMonth();
        Task<int> GetIncomeForYear();
        Task<List<DoughnutChart>> CreateDoughnutChartForWeek();
        List<SplineChart> CreateSplineChart(List<Lesson> lessons);
        IEnumerable<SplineChart> CreateSplineChartForEachDay(string[] days, List<SplineChart> result);
        Task<IEnumerable<SplineChart>> CreateSplineChartForWeek();
        Task<IEnumerable<SplineChart>> CreateSplineChartForYear();
        Task<List<DoughnutChart>> CreateLineChartForMonth(int id);   
        Task<int> GetIncomeForSpecifiedMonth(int id);
        Task<List<CarouselDataBinding>> FillCarouselList();  
    }
}
