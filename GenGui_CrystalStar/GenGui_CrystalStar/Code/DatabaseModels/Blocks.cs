using SQLite;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GenGui_CrystalStar.Code.DatabaseModels;

public class Blocks
{
    [PrimaryKey]
    public int ID { get; set; }
    public string BlockName { get; set; }
    public BlockFlag BlockFlag { get; set; }
    public int TagCount { get; set ;}
    public int SelectCount { get; set; } = 0;
    public Enabled ShuffleEnabled { get; set; }
    public Enabled TagStyleEnabled { get; set; }
    public TagStyle TagStyleOption { get; set; }
    public Enabled RandomDropEnabled { get; set; }
    public int RandomDropChance { get; set; }
    public Enabled AddAdjEnabled { get; set; }
    public AdjType AddAdjTypeOption { get; set; }
    public int AddAdjChance { get; set; }

}