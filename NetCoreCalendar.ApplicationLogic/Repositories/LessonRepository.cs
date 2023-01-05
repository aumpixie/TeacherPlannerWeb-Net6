using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.ApplicationLogic.Contracts;
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

        /**
         * Adds the lesson object to the database
         **/
        public async Task CreateLesson(LessonCreateVM model)
        {
            // we find the user that is currently using the app
            var user = await studentRepository.GetUserRecords();

            // combine all the DateTime properties of the model into two DateTime variables
            DateTime newDateTimeStart = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day,
                                    model.StartTime.Hour, model.StartTime.Minute, model.StartTime.Second);
            DateTime newDateTimeEnd = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day,
                                    model.EndTime.Hour, model.EndTime.Minute, model.EndTime.Second);

            var lesson = mapper.Map<Lesson>(model);
            // find the student for whom the lesson is created
            var student = await studentRepository.GetStudentAsync(lesson.StudentId);
            if(student != null)
            {
                // pass missing information to the variables that will be visible in the database table
                lesson.Start = newDateTimeStart;
                lesson.End = newDateTimeEnd;
                lesson.RequestingUserId = user.Id;
                lesson.Rate = student.Rate;
            }
            await AddAsync(lesson);
        }

        /**
         * Returns false if there is no lesson for the existing date and time, and true otherwise
         **/
        public async Task<bool> ExistsDate(LessonCreateVM model)
        {
            // combine all the DateTime properties that the user mentioned in the model into one DateTime variable
            DateTime newDateTimeStart = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day,
                                   model.StartTime.Hour, model.StartTime.Minute, model.StartTime.Second);
            LessonVM lesson = await CheckLessonDate(newDateTimeStart);

            // checks the id to make sure that user wants to edit the lesson, not creates a new one for the existing date and time
            if (lesson != null && model.Id != lesson.Id)
            {
                return true;
            }
            return false;
        }


        /*
         * Checks if a lesson with the same date and time already exists in our database
         */

        public async Task<LessonVM> CheckLessonDate(DateTime startDate)
        {

            // we find the user that is currently using the app
            var user = await studentRepository.GetUserRecords();
            var lesson = await context.Lessons
                  .AsNoTracking()
                  .Where(q => q.RequestingUserId == user.Id)
                  .Include(l => l.Student)
                  .FirstOrDefaultAsync(m => m.Start == startDate);
 
            var model = mapper.Map<LessonVM>(lesson);
            return model;
        }

        /**
         * Finds all the lessons of the user in the database
         **/
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

        /**
         * Finds all the lessons of the user in the database to serialize them for the calendar
         **/
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

        /**
         * Gets the lesson view models 
         **/
        public async Task<LessonsViewVM> GetMyLessonsAsync()
        {
            var lessonsVM = await GetAllLessonsAsync(); ;
            var model = new LessonsViewVM(lessonsVM);
            return model;
        }

        /**
         * Finds a lesson with teh corresponding id in the database
         **/
        public async Task<LessonDetailsVM?> GetLessonAsync(int? id)
        {
            var lesson = await context.Lessons
                .Include(l => l.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            var lessonVM = mapper.Map<LessonDetailsVM>(lesson);
            if(lesson != null)
            {
                lessonVM.StartTime = lesson.Start;
                lessonVM.StartDate = lesson.Start;
                lessonVM.EndTime = lesson.End;
            }
            return lessonVM;
        }

        /**
         * Updates the information about the existing lesson in the database
         **/
        public async Task<Lesson> UpdateModel(LessonCreateVM model)
        {
            // find the user that is currently using the app
            var user = await studentRepository.GetUserRecords();

            // combine all the DateTime properties of the model into two DateTime variables
            DateTime newDateTimeStart = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day,
                                   model.StartTime.Hour, model.StartTime.Minute, model.StartTime.Second);
            DateTime newDateTimeEnd = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day,
                                    model.EndTime.Hour, model.EndTime.Minute, model.EndTime.Second);

            var lesson = mapper.Map<Lesson>(model);
            // pass missing information to the variables that will be visible in the database table
            lesson.Start = newDateTimeStart;
            lesson.End = newDateTimeEnd;
            lesson.RequestingUserId = user.Id;
            return lesson;
        }

        /**
         * Updates the isPaid Property of the object with the indicated id in the database
         **/
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

        /**
         * Updates the lesson record in the database
         **/
        public async Task UpdateLessonAsync(LessonCreateVM model)
        {
            var lesson = await UpdateModel(model);
            await UpdateAsync(lesson);
        }

        /**
         * Finds the lesson with the indicated id to update
         **/
        public async Task<LessonCreateVM?> GetLessonToUpdateAsync(int? id)
        {
            var lesson = await context.Lessons
             .Include(l => l.Student)
             .FirstOrDefaultAsync(m => m.Id == id);

            var model = mapper.Map<LessonCreateVM>(lesson);
            if (lesson != null)
            {
                model.StartTime = lesson.Start;
                model.EndTime = lesson.End;
                model.StartDate = lesson.Start;
            }
            return model;
        }
    }
}
