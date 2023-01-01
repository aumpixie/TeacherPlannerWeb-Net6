using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace NetCoreCalendar.Models
{
    public class LessonDetailsVM
    {
        public int Id { get; set; }

        [Display(Name = "Start Time")]
        [DisplayFormat(DataFormatString = "{0:t}")]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }

        [Display(Name = "End Time")]
        [DisplayFormat(DataFormatString = "{0:t}")]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public StudentVM? Student { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Paid")]
        public bool IsPaid { get; set; }
        public decimal Rate { get; set; }

        public string? FullName
        {
            get
            {
                return Student?.FirstName + " " + Student?.LastName;
            }
        }
    }
}
