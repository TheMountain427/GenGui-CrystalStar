using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenGui_CrystalStar.Code.DatabaseModels.Configurations;

internal class sqlite_schemaConfig : IEntityTypeConfiguration<sqlite_schema>
{
    public void Configure(EntityTypeBuilder<sqlite_schema> builder)
    {
        builder.ToTable("sqlite_schema");
        builder.HasKey(x => x.type);
    }
}