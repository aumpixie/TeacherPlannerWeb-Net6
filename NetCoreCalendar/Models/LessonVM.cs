using NetCoreCalendar.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace NetCoreCalendar.Models
{
    public class LessonVM
    {
        public int Id { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public StudentVM? Student { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Paid")]
        public bool IsPaid { get; set; }
        public decimal Rate { get; set; }
    }
}
