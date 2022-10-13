using Microsoft.AspNetCore.Mvc.Rendering;
using NetCoreCalendar.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace NetCoreCalendar.Models
{
    public class LessonCreateVM : IValidatableObject
    {

        [Required]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        // it represents that we are connected to the Student table
        public SelectList? Students { get; set; }

        public decimal? Rate { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        [DisplayFormat(DataFormatString = "{0:t}")]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        [DisplayFormat(DataFormatString = "{0:t}")]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        public bool IsPaid { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StudentId == 0)
            {
                yield return new ValidationResult("Choose a student", new[] { nameof(StudentId) });
            }
        }
    }
}
