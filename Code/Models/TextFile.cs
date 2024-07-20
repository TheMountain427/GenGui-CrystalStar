using GenGui_CrystalStar.Code.DatabaseModels;

namespace GenGui_CrystalStar.Code.Models;

public class TextFile
{
    public string FilePath { get; set; } = string.Empty;
    public bool IsLoaded { get; set; } = false;
    public Dictionary<string, int> ContainedBlocks { get; set; } = new Dictionary<string, int>();
}