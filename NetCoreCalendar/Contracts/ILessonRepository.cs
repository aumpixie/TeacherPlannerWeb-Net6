using NetCoreCalendar.Data;
using NetCoreCalendar.Models;

namespace NetCoreCalendar.Contracts
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {

        Task CreateLesson(LessonCreateVM model);
        Task<List<Lesson>> GetAllLessonsAsync(string employeeId);

        Task<LessonsViewVM> GetMyLessonsAsync();
    }
}
