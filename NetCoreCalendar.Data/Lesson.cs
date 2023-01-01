using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreCalendar.Data
{
    public class Lesson : BaseEntity
    {
        [ForeignKey("StudentId")]
        // it represents that we are connected to the Student table
        public Student? Student { get; set; }
        public int StudentId { get; set; }
        public string? Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsPaid { get; set; }
        public string? RequestingUserId { get; set; }
    }
}
