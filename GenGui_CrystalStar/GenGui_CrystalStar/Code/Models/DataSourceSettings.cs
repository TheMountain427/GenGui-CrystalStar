

namespace GenGui_CrystalStar.Code.Models;

public interface IDataSourceSettings
{
    string Type { get; set; }
}

public class DataSourceSettings : IDataSourceSettings
{
    public string Type { get; set; }
}
