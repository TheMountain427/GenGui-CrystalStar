using SQLite;

namespace GenGui_CrystalStar.Code.DatabaseModels;

public class Blocks
{
    [PrimaryKey]
    public int ID { get; set; }
    public string BlockName { get; set; }
    public BlockFlag BlockFlag { get; set; }
    public int TagCount { get; set ;}
    public int SelectCount { get; set; } = 0;
}