using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;
using NetCoreCalendar.Models;
using System.Web.Mvc;

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
            var student = await studentRepository.GetStudentAsync(lesson.StudentId);
            lesson.Start = newDateTimeStart;
            lesson.End = newDateTimeEnd;
            lesson.RequestingUserId = user.Id;
            lesson.Rate = student.Rate;
            await AddAsync(lesson);
        }

        public async Task<bool> ExistsDate(LessonCreateVM model)
        {
            var user = await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User);
            DateTime newDateTimeStart = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day,
                                   model.StartTime.Hour, model.StartTime.Minute, model.StartTime.Second);
            var lessonOriginal = mapper.Map<Lesson>(model);
            var lesson = await context.Lessons
              .Where(q => q.RequestingUserId == user.Id)
              .Include(l => l.Student)
              .FirstOrDefaultAsync(m => m.Start == newDateTimeStart);

            if(lesson != null)
            {
                return true;
            }
            return false;
        }


        public async Task<List<LessonDetailsVM>> GetAllLessonsAsync()
        {
            var user = await studentRepository.GetUserRecords();
            var lessons = await context.Lessons
                .Where(q => q.RequestingUserId == user.Id)
                .Include(l => l.Student)
                .ToListAsync();
            var lessonsVM = mapper.Map<List<LessonDetailsVM>>(lessons);
            foreach(var lesson in lessonsVM)
            {
                lesson.StartTime = lesson.Start;
                lesson.EndTime = lesson.End;
            }
        
            return lessonsVM;
        }

        public async Task<List<LessonVM>> GetAllLessonsForCalendarAsync()
        {
            var user = await studentRepository.GetUserRecords();
            var lessons = await context.Lessons
                .Where(q => q.RequestingUserId == user.Id)
                .Include(l => l.Student)
                .ToListAsync();
            var lessonsVM = mapper.Map<List<LessonVM>>(lessons);
            return lessonsVM;
        }

        public async Task<LessonsViewVM> GetMyLessonsAsync()
        {
            var lessonsVM = await GetAllLessonsAsync(); ;
            var model = new LessonsViewVM(lessonsVM);
            return model;
        }

        public async Task<LessonDetailsVM?> GetLessonAsync(int? id)
        {
            var lesson = await context.Lessons
                .Include(l => l.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            var lessonVM = mapper.Map<LessonDetailsVM>(lesson);
            lessonVM.StartTime = lesson.Start;
            lessonVM.StartDate = lesson.Start;
            lessonVM.EndTime = lesson.End;
            return lessonVM;
        }

        public async Task<Lesson> UpdateModel(LessonCreateVM model)
        {
            var user = await studentRepository.GetUserRecords();
            DateTime newDateTimeStart = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day,
                                   model.StartTime.Hour, model.StartTime.Minute, model.StartTime.Second);
            DateTime newDateTimeEnd = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day,
                                    model.EndTime.Hour, model.EndTime.Minute, model.EndTime.Second);
            var lesson = mapper.Map<Lesson>(model);
            lesson.Start = newDateTimeStart;
            lesson.End = newDateTimeEnd;
            lesson.RequestingUserId = user.Id;
            return lesson;
        }

        public async Task UpdatePaid( int? id)
        {
            var lesson = await GetAsync(id);
            if(lesson != null)
            {
                if (lesson.IsPaid)
                {
                    lesson.IsPaid = false;
                }
                else
                {
                    lesson.IsPaid = true;
                }
            }
            await UpdateAsync(lesson);
        }

        public async Task UpdateLessonAsync(LessonCreateVM model)
        {
            var lesson = await UpdateModel(model);
            await UpdateAsync(lesson);
        }

        public async Task<LessonCreateVM?> GetLessonToUpdateAsync(int? id)
        {
            var lesson = await context.Lessons
             .Include(l => l.Student)
             .FirstOrDefaultAsync(m => m.Id == id);
            var model = mapper.Map<LessonCreateVM>(lesson);
            model.StartTime = lesson.Start;
            model.EndTime = lesson.End;
            model.StartDate = lesson.Start;
            return model;
        }

    }
}
