using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;
using NetCoreCalendar.Models;

namespace NetCoreCalendar.Repositories
{
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<Teacher> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IStudentRepository studentRepository;

        public LessonRepository(ApplicationDbContext context, IMapper mapper,
             UserManager<Teacher> userManager, IHttpContextAccessor httpContextAccessor,
             IStudentRepository studentRepository) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.studentRepository = studentRepository;
        }

        public async Task CreateLesson(LessonCreateVM model)
        {
            var user = await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User);

            DateTime newDateTimeStart = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day,
                                    model.StartTime.Hour, model.StartTime.Minute, model.StartTime.Second);
            DateTime newDateTimeEnd = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day,
                                    model.EndTime.Hour, model.EndTime.Minute, model.EndTime.Second);
            var lesson = mapper.Map<Lesson>(model);
            lesson.Start = newDateTimeStart;
            lesson.End = newDateTimeEnd;
            lesson.RequestingUserId = user.Id;
            await AddAsync(lesson);
        }


        public async Task<List<Lesson>> GetAllLessonsAsync(string employeeId)
        {
            return await context.Lessons
                .Where(q => q.RequestingUserId == employeeId)
                .Include(l => l.Student)
                .ToListAsync();
        }

        public async Task<LessonsViewVM> GetMyLessonsAsync()
        {
            var user = await studentRepository.GetUserRecords();
            var requests = mapper.Map<List<LessonVM>>(await GetAllLessonsAsync(user.Id));

            var model = new LessonsViewVM(requests);
            return model;

        }
    }
}
