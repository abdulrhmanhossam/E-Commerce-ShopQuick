using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelClasses;

namespace DataBaseAccess.Configuration
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();
            builder.Property(x => x.Address).IsRequired();
            builder.Property(x => x.City).IsRequired();
            builder.Property(x => x.Country);
            builder.Property(x => x.PostalCode).IsRequired();
            builder.Property(x => x.State).IsRequired();
        }
    }
}
