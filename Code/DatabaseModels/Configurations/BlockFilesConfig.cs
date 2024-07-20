using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenGui_CrystalStar.Code.DatabaseModels.Configurations;

internal class BlockFilesConfig : IEntityTypeConfiguration<BlockFiles>
{
    public void Configure(EntityTypeBuilder<BlockFiles> builder)
    {
        builder.ToTable("BlockFiles");
        builder.HasKey(x => x.ID);
    }
}