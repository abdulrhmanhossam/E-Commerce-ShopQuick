using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelClasses;

namespace DataBaseAccess.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CategoryName)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.CategoryName)
                .Metadata.SetAnnotation("Description", "Length can't go above 30");
        }
    }
}
