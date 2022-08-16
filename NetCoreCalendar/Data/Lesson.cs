using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreCalendar.Data
{
    public class Lesson : BaseEntity
    {
        public string Name { get; set; }

        [ForeignKey("StudentId")]
        // it represents that we are connected to the Student table
        public Student Student { get; set; }
        public int StudentId { get; set; }


        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsPaid { get; set; }
    }
}
