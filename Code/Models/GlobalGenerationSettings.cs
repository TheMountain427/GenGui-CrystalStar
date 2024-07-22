
namespace GenGui_CrystalStar.Code.Models;

public class GlobalGenerationSettings
{
    public int OutputCount { get; set; } = 1;
    public TrimLastComma TrimLastComma { get; set; } = TrimLastComma.True;
    public GlobalShuffleSetting ShuffleSetting { get; set; } = GlobalShuffleSetting.None;
    public OutputType OutputType { get; set; } = OutputType.Positive;
    public GlobalTagStyleSettings GlobalTagStyleSettings { get; set; } = new GlobalTagStyleSettings();
    public GlobalRandomDropSettings GlobalRandomDropSettings { get; set; } = new GlobalRandomDropSettings();
    public GlobalAddAdjectivesSettings GlobalAddAdjSettings { get; set; } = new GlobalAddAdjectivesSettings();
}

public class GlobalAddAdjectivesSettings
{
    public Enabled IsEnabled { get; set; } = Enabled.Disabled;
    public SelectionScope SelectionScope { get; set; } = SelectionScope.Global;
    public AdjType GlobalAdjType { get; set; } = AdjType.All;
    public int GlobalAddAdjChance { get; set; } = 0;
}

public class GlobalRandomDropSettings
{
    public Enabled IsEnabled { get; set; } = Enabled.Disabled;
    public SelectionScope SelectionScope { get; set; } = SelectionScope.Global;
    public int GlobalRandomDropChance { get; set; } = 0;
}

public class GlobalTagStyleSettings
{
    public Enabled IsEnabled { get; set; } = Enabled.Disabled;
    public SelectionScope SelectionScope { get; set; } = SelectionScope.Global;
    public TagStyle GlobalTagStyle { get; set; } = TagStyle.Clean;
}