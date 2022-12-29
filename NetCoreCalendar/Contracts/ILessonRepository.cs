using NetCoreCalendar.Data;
using NetCoreCalendar.Models;

namespace NetCoreCalendar.Contracts
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {

        Task CreateLesson(LessonCreateVM model);
        Task<List<LessonDetailsVM>> GetAllLessonsAsync();

        Task<LessonsViewVM> GetMyLessonsAsync();
        Task<LessonDetailsVM?> GetLessonAsync(int? id);
        Task<List<LessonVM>> GetAllLessonsForCalendarAsync();
        Task<Lesson> UpdateModel(LessonCreateVM model);
        Task UpdatePaid(int? id);
        Task UpdateLessonAsync(LessonCreateVM model);
        Task<LessonCreateVM?> GetLessonToUpdateAsync(int? id);
        Task<bool> ExistsDate(LessonCreateVM model);
    }
}
