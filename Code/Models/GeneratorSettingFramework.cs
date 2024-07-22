using GenGui_CrystalStar.Code.DatabaseModels;

namespace GenGui_CrystalStar.Code.Models;

public class GeneratorSettingFramework
{
    public required string BlockName { get; set; }
    public BlockFlag BlockFlag { get; set; }
    public int SelectCount { get; set; }
    public int TotalTags { get; set; }
    public List<int> SelectedLineNumbers { get; set; } = [];
    public List<Tags> SelectedLines { get; set; } = [];
    public List<string> FinalLines { get; set; } = [];
    public TagStyle TagStyle { get; set; } = TagStyle.Clean;
    public Enabled BlockShuffle { get; set; } = Enabled.Disabled;
    public Enabled RandomDrop { get; set;} = Enabled.Disabled;
    public int RandomDropChance { get; set; } = 0;
    public Enabled AddAdj { get; set; } = Enabled.Disabled;
    public int AddAdjChance { get; set; } = 0;
    public AdjType AddAdjType { get; set; } = 0;
}