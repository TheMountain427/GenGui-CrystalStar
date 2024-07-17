using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenGui_CrystalStar.Code.DatabaseModels.Configurations;

internal class GenerationSettingsConfig : IEntityTypeConfiguration<GenerationSettings>
{
    public void Configure(EntityTypeBuilder<GenerationSettings> builder)
    {
        builder.ToTable("GenerationSettings");
        builder.HasKey(x => x.ID);
    }
}