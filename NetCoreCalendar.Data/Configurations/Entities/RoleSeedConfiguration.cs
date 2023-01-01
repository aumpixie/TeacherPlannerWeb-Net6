using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCoreCalendar.Constants;
using System.Data;

namespace NetCoreCalendar.Data.Configuration.Entities
{
    internal class RoleSeedConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                 new IdentityRole
                 {
                     Id = "e71edcef-91ae-468c-98a2-a3d88d3fd01b",
                     Name = Roles.User,
                     NormalizedName = Roles.User.ToUpper()
                 });
        }
    }
}