using System.Text.RegularExpressions;
using GenGui_CrystalStar.Exceptions;
using GenGui_CrystalStar.Validations;

namespace GenGui_CrystalStar.Services;

public class TagBlocks : Dictionary<string, BlockData>
{
    public Dictionary<string, BlockData> Tagblocks { get; set; }

    public TagBlocks()
    {
        Tagblocks = new Dictionary<string, BlockData>();
    }


}

public class BlockData 
{
    public List<string> Tags { get; set; }
    public string BlockName { get; set; }
    public string BlockFlag { get; set; }
    public int SelectCount { get; set; }
    public List<string> SelectedLines { get; set; }

    // Parameterless constructor
    public BlockData() 
    {
        Tags = new List<string>();
        BlockName = string.Empty;
        BlockFlag = string.Empty;
        SelectCount = 0;
        SelectedLines = new List<string>();
    }

    // Constructor that accepts a dictionary for initialization
    public BlockData(Dictionary<string, object> properties) : this() // <-- this() is what I was missing
    {
        Init(properties);
    }

    // Init method to initialize properties
    private void Init(Dictionary<string, object> properties) 
    {
        // Initialize with default values
        Tags = properties.TryGetValue("Tags", out var tags) ? tags as List<string> : new List<string>();
        BlockName = properties.TryGetValue("BlockName", out var blockname) ? blockname as string : string.Empty;
        BlockFlag = properties.TryGetValue("BlockFlag", out var blockflag) ? blockflag as string : string.Empty;
        SelectCount = properties.TryGetValue("SelectCount", out var selectcount) ? (int)selectcount : SelectCount = 0;
        SelectedLines = properties.TryGetValue("SelectedLines", out var selectedlines) ? selectedlines as List<string> : new List<string>();

    }
}



public class GetData
{

    public static async Task<TagBlocks> GetLinesFromTxtFiles()
    {

        // grab files from path
        string datapath = @"C:\Users\sbker\OneDrive\Desktop\(WS)-GenGui-CrystalStar\Prompt Collections";
        string[] files = Directory.GetFiles(datapath, "*.txt");

        // grab lines from files
        var datalines = new List<string> { Capacity = 20000 };
        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                datalines.Add(line);
            }
        }

        // block start and end, technically not needed but may be of use later
        var startofblocks = new Dictionary<string, int>();
        var endofblocks = new Dictionary<string, int>();

        // blocks section, probably removed later when made into class
        var blocks = new TagBlocks();

        // grab count so it is not re-called every loop <--- actually it shouldn't matter because references?
        var total_lines = datalines.Count;

        /* loop through lines, do all the things within a single loop
        GenGui-Reloaded loops to find blocks, then loops through each block
        we will increment the linenum within the loop while inside a block
        after leaving a block, the for continues having 'skipped' lines
        basically the for loop looks for a start of a block */
        for (int linenum = 0; linenum < total_lines; linenum++)
        {
            /*         default .Net Regex Options = case sensitive, not m
                    Regex.Matches = g (global), Regex.Match = not global, return after first */
            try
            {
                /*         start of a block
                        \s*\{.*:.*$ <- 86 steps to fail, oops
                        ^\s*\{.*:.*$ <- 3 steps to fail 
                        ^\s*\{\s*\w*:.*$ faster than above line */
                if (Regex.IsMatch(datalines[linenum], @"(?i)^\s*\{\s*\w*:.*$"))
                {
                    // Validate flag exists and is a valid one
                    var flagmatch = Regex.Match(datalines[linenum], @"(?i)\W*\w*:\s*(\w*)$");
                    if (flagmatch.Success == false)
                    {
                        var e = new InvalidFlag($"{GetAllData.DataContext(datalines, linenum)}");
                        throw e;
                    }
                    else if (Flags.ValidFlags.Contains(flagmatch.Groups[1].ToString()) == false)
                    {
                        var e = new InvalidFlag($"{flagmatch.Groups[1]} is not a valid flag.{GetAllData.DataContext(datalines, linenum)}");
                        throw e;
                    }
                    else
                    {
                        string blockflag = flagmatch.Groups[1].ToString();

                        /*             capture the blockname, save for later, add to startofblocks
                                    ^\s*\{\s*(\w*):.*$ <- 11 steps
                                    ^\W*(\w*) <- 6 steps */
                        var match = Regex.Match(datalines[linenum], @"(?i)^\W*(\w*)");
                        string blockname = match.Groups[1].ToString();
                        startofblocks.Add(blockname, linenum);


                        // increase linenum so we don't add the blockstart to tags list
                        linenum++;

                        // grab the tags
                        var tags = new List<string>();
                        // go till end of block but don't capture that one, if a new start block is found, throw and discard the current block
                        while (linenum < total_lines && Regex.IsMatch(datalines[linenum], @"^\s*\}\s*$|^\s*\{") == false)
                        {
                            tags.Add(datalines[linenum]);
                            linenum++;
                        }
                        if (Regex.IsMatch(datalines[linenum], @"^\s*\}\s*$") == false || linenum == total_lines)
                        {
                            linenum--; //negate last skip
                            var e = new UnclosedBlock($"{GetAllData.DataContext(datalines, linenum)}");
                            throw e;
                        }

                        // add blockname and endblock linenum
                        endofblocks.Add(blockname, linenum);

                        // create the Dictionary with the goods
                        var thestuff = new Dictionary<string, object>
                        {
                            { "Tags", tags },
                            { "BlockName", blockname },
                            { "BlockFlag", blockflag }
                        };
                        

                        // add to blocks Dictionary
                        blocks.Add(blockname, new BlockData(thestuff));
                    }
                }
            }
            catch (InvalidFlag e)
            {
                Console.WriteLine($"{e}");
            }
            catch (UnclosedBlock e)
            {
                Console.WriteLine($"{e}");
            }
        }
        return blocks;
    }
}
