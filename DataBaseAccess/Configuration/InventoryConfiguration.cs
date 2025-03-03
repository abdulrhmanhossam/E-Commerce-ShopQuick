using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelClasses;

namespace DataBaseAccess.Configuration
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.InventoryName).IsRequired();

             builder.Property(x => x.PurchasePrice)
                .HasColumnType("decimal(18, 2)")
                .HasAnnotation("Range", new RangeAttribute(1.0, 999.99)
                { ErrorMessage = "Range 1 to 999.99 only"})
                .HasAnnotation("RegularExpression", new RegularExpressionAttribute(@"^[0,9]+(\.[0-9]{1,2})$")
                {ErrorMessage = "please insert two digits after decimal. Example: 0.00"}).IsRequired();

        }
    }
}