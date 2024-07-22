
namespace GenGui_CrystalStar.Code.Models;

public interface ITextFileSourceSettings
{
    public string Path { get; set; }
}

public class TextFileSourceSettings : ITextFileSourceSettings
{
    public string Path { get; set; }
}