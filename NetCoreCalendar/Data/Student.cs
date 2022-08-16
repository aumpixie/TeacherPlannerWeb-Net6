using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreCalendar.Data
{
    public class Student : BaseEntity
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Goal { get; set; }
    }
}
