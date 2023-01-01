using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;
using NetCoreCalendar.Models;
using Org.BouncyCastle.Asn1.Ocsp;

namespace NetCoreCalendar.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<Teacher> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public StudentRepository(ApplicationDbContext context, IMapper mapper, UserManager<Teacher> userManager,
            IHttpContextAccessor httpContextAccessor) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        /**
         * Adds Student object to the database
         **/
        public async Task CreateStudent(StudentCreateVM model)
        {
            var student = await UpdateModel(model);
            await AddAsync(student);
        }

        /**
         * Gets all the student view models
         **/
        public async Task<List<StudentVM>> GetAllStudentsVMAsync()
        {
            var students = await GetAllStudentsAsync();
            var studentsVM = mapper.Map<List<StudentVM>>(students);
            return studentsVM;
        }

        /**
         * Finds all the student records of the current user in the database
         **/
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            var user = await GetUserRecords();
            return await context.Students.Where(q => q.RequestingUserId == user.Id).ToListAsync();
        }

        /**
         * Updates the Student record in the database
         **/
        public async Task UpdateStudentAsync(StudentCreateVM model)
        {
            var student = await UpdateModel(model);
            await UpdateAsync(student);
        }

        /**
         * Find the user that is currently using the app
         **/
        public async Task<Teacher> GetUserRecords()
        {
            return await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User);
        }

        /**
         * Passes the model and returns the Student object
         **/
        public async Task<Student> UpdateModel(StudentCreateVM model)
        {
            var user = await GetUserRecords();
            var student = mapper.Map<Student>(model);
            student.RequestingUserId = user.Id;
            return student;
        }

        /**
         * Gets the student object with the correspinding id and returns it as a model
         **/
        public async Task<StudentVM?> GetStudentAsync(int? id)
        {
            var student = await GetAsync(id);
            return mapper.Map<StudentVM>(student);
        }

        /**
         * Finds the Student object that we need to update
         **/
        public async Task<StudentCreateVM?> GetStudentToUpdateAsync(int? id)
        {
            var student = await GetAsync(id);
            return mapper.Map<StudentCreateVM>(student);
        }
    }
}
