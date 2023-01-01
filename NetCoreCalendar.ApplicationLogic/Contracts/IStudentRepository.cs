using NetCoreCalendar.Data;
using NetCoreCalendar.Models;

namespace NetCoreCalendar.ApplicationLogic.Contracts
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<List<StudentVM>> GetAllStudentsVMAsync();
        Task<List<Student>> GetAllStudentsAsync();
        Task CreateStudent(StudentCreateVM model);
        Task UpdateStudentAsync(StudentCreateVM model);
        Task<Teacher> GetUserRecords();
        Task<Student> UpdateModel(StudentCreateVM model);
        Task<StudentVM?> GetStudentAsync(int? id);
        Task<StudentCreateVM?> GetStudentToUpdateAsync(int? id);
    }
}
