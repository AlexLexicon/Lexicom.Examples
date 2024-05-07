using Lexicom.Examples.InventoryManagement.Client.Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lexicom.Examples.InventoryManagement.Client.Application.Database.Configurations;
public class ProductFieldEntityTypeConfiguration : IEntityTypeConfiguration<ProductField>
{
    public void Configure(EntityTypeBuilder<ProductField> builder)
    {
        builder.HasKey(pf => new { pf.Key, pf.ProductId });
    }
}
