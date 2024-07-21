using SQLite;

namespace GenGui_CrystalStar.Code.DatabaseModels;

public class PastGenerationSettings
{
    [PrimaryKey]
    public int ID { get; set; }
    public string GlobalSettingsJson { get; set; } = "";
    public string BlockSettingsJson { get; set; } = "";
}
