using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelClasses;

namespace DataBaseAccess.Configuration
{
    public class ProductImagesConfiguration : IEntityTypeConfiguration<ProductImages>
    {
        public void Configure(EntityTypeBuilder<ProductImages> builder)
        {
            builder.Property(x => x.Id).IsRequired();

            builder.HasOne(x => x.Product)
             .WithMany(x => x.ImgUrls)
             .HasForeignKey(x => x.ProductId);
        }
    }
}