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

        public async Task CreateStudent(StudentCreateVM model)
        {
            var student = await UpdateModel(model);
            await AddAsync(student);
        }

        public async Task<List<StudentVM>> GetAllStudentsVMAsync()
        {
            var students = await GetAllStudentsAsync();
            var studentsVM = mapper.Map<List<StudentVM>>(students);
            return studentsVM;
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            var user = await GetUserRecords();
            return await context.Students.Where(q => q.RequestingUserId == user.Id).ToListAsync();
        }

        public async Task UpdateStudentAsync(StudentCreateVM model)
        {
            var student = await UpdateModel(model);
            await UpdateAsync(student);
        }

        public async Task<Teacher> GetUserRecords()
        {
            return await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User);
        }

        public async Task<Student> UpdateModel(StudentCreateVM model)
        {
            var user = await GetUserRecords();
            var student = mapper.Map<Student>(model);
            student.RequestingUserId = user.Id;
            return student;
        }

        public async Task<StudentVM?> GetStudentAsync(int? id)
        {
            var student = await GetAsync(id);
            return mapper.Map<StudentVM>(student);
        }

        public async Task<StudentCreateVM?> GetStudentToUpdateAsync(int? id)
        {
            var student = await GetAsync(id);
            return mapper.Map<StudentCreateVM>(student);
        }
    }
}
