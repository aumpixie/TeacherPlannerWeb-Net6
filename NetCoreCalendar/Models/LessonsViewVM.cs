namespace NetCoreCalendar.Models
{
    public class LessonsViewVM
    {
        public LessonsViewVM(List<LessonVM> lessons)
        {
            Lessons = lessons;
        }

        public List<LessonVM> Lessons { get; set; }
    }
}
