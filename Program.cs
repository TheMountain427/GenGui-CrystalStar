using GenGui_CrystalStar.Services;
using Microsoft.Extensions.Configuration;
namespace GenGui_CrystalStar;

class Program
{
    static void Main(string[] args)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
        string datapath = config.GetValue<string>("DataSource:Path")!;

        var dbservice = new GenGuiDataBaseService();
        var textservice = new TextFileSourceService(datapath);
    }
}
