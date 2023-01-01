
namespace NetCoreCalendar.Data
{
    public class Student : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Goal { get; set; }
        public string? RequestingUserId { get; set; }
    }
}
