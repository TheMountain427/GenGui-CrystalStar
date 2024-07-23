

using SQLite;

namespace GenGui_CrystalStar.Code.DatabaseModels
{
    public class BlockFiles
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? BlockName { get; set; }
        public BlockFlag BlockFlag { get; set; }
    }
}