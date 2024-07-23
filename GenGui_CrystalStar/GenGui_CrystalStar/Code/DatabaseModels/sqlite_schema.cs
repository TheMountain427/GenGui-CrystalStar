using SQLite;

namespace GenGui_CrystalStar.Code.DatabaseModels;

public class sqlite_schema
{
    [PrimaryKey]
    public string type { get; set; }
    public string name { get; set; }
    public string tbl_name { get; set; }
    public int rootpage { get; set; }
    public string text { get; set; }
}