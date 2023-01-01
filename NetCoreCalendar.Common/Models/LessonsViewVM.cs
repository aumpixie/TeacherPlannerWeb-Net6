namespace NetCoreCalendar.Models
{
    public class LessonsViewVM
    {
        public LessonsViewVM(List<LessonDetailsVM> lessons)
        {
            Lessons = lessons;
        }
        public List<LessonDetailsVM> Lessons { get; set; }
    }
}
