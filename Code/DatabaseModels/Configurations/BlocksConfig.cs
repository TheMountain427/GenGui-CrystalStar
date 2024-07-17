using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenGui_CrystalStar.Code.DatabaseModels.Configurations;

internal class BlocksConfig : IEntityTypeConfiguration<Blocks>
{
    public void Configure(EntityTypeBuilder<Blocks> builder)
    {
        builder.ToTable("Blocks");
        builder.HasKey(x => x.ID);
    }
}