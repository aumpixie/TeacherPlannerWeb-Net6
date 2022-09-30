using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetCoreCalendar.Configuration.Entities;

namespace NetCoreCalendar.Data
{
    public class ApplicationDbContext : IdentityDbContext<Teacher>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RoleSeedConfiguration());
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
    }
}