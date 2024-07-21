using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenGui_CrystalStar.Code.DatabaseModels.Configurations;

internal class PastGenerationSettingsConfig : IEntityTypeConfiguration<PastGenerationSettings>
{
    public void Configure(EntityTypeBuilder<PastGenerationSettings> builder)
    {
        builder.ToTable("PastGenerationSettings");
        builder.HasKey(x => x.ID);
    }
}