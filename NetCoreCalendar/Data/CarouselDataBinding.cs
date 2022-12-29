namespace NetCoreCalendar.Data
{
    public class CarouselDataBinding
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int Income { get; set; }

        public List<DoughnutChart> DoughnutChartData { get; set; }
    }
}
