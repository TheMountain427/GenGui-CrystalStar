
namespace GenGui_CrystalStar.Code.Models;

public class BlockGenSettingsList
{
    public List<BlockGenerationSettings> BlockGenSettings { get; set; } = [];
}

public class BlockGenerationSettings
{
    public string BlockName { get; set; } = "";
    public BlockFlag BlockFlag { get; set; }
    public int SelectCount { get; set; } = 0;
    public Enabled BlockShuffleSetting { get; set; }
    public BlockTagStyleSettings BlockTagStyleSettings { get; set; } = new BlockTagStyleSettings();
    public BlockRandomDropSettings BlockRandomDropSettings { get; set; } = new BlockRandomDropSettings();
    public BlockAddAdjectivesSettings BlockAddAdjSettings { get; set; } = new BlockAddAdjectivesSettings();
}
public class BlockAddAdjectivesSettings
{
    public Enabled IsEnabled { get; set; } = Enabled.Disabled;
    public AdjType BlockAdjType { get; set; } = AdjType.All;
    public int BlockAddAdjChance { get; set; } = 0;
}

public class BlockRandomDropSettings
{
    public Enabled IsEnabled { get; set; } = Enabled.Disabled;
    public int BlockRandomDropChance { get; set; } = 0;
}

public class BlockTagStyleSettings
{
    public Enabled IsEnabled { get; set; } = Enabled.Disabled;
    public TagStyle BlockTagStyle { get; set; } = TagStyle.Clean;
}