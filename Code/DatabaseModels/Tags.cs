using SQLite;

namespace GenGui_CrystalStar.Code.DatabaseModels;

public class Tags
{
    [PrimaryKey]
    public int ID { get; set; }
    public int LineNumber { get; set; }
    public string Line { get; set; }
    public string CommaTag { get; set; }
    public string CleanTag { get; set; }
    public string CleanTagUnderscore { get; set; }
    public string CommaTagUnderscore { get; set; }
    public bool IsMultiTag { get; set; }
    public string BlockName { get; set; }
    public BlockFlag BlockFlag { get; set; }
}