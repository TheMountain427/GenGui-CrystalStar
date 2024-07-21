using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenGui_CrystalStar.Code.DatabaseModels.Configurations;

internal class PromptHistoryConfig : IEntityTypeConfiguration<PromptHistory>
{
    public void Configure(EntityTypeBuilder<PromptHistory> builder)
    {
        builder.ToTable("PromptHistory");
        builder.HasKey(x => x.ID);
    }
}