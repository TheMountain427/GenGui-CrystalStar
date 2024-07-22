using SQLite;

namespace GenGui_CrystalStar.Code.DatabaseModels;

public class PromptHistory
{
    [PrimaryKey]
    public int ID { get; set; }
    public string Output { get; set; } = "";
    public string GenSettings { get; set; } = "";
    public string BlockAttributes { get; set; } = "";

}