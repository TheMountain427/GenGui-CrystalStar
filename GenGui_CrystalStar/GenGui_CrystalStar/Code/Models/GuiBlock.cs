
namespace GenGui_CrystalStar.Code.Models;

public class GuiBlock
{
    public string BlockName { get; set; }
    public BlockFlag BlockFlag { get; set; }
    public int SelectCount { get; set; } = 0;
    public bool ShuffleEnabled { get; set; }
    public bool TagStyleEnabled { get; set; }
    public TagStyle SelectedTagStyleOption { get; set; } = TagStyle.Clean;
    public bool RandomDropEnabled { get; set; }
    public double RandomDropChance { get; set; }
    public bool AddAdjEnabled { get; set; }
    public AdjType SelectedAddAdjTypeOption { get; set; }
    public double AddAdjChance { get; set; }
}

