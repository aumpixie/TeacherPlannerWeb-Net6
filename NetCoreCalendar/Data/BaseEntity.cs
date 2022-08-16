using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreCalendar.Data
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Rate { get; set; }
    }
}
