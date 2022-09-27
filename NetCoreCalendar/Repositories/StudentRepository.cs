using NetCoreCalendar.Contracts;
using NetCoreCalendar.Data;

namespace NetCoreCalendar.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
