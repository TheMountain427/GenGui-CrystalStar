// using System.Collections;
// using System.Collections.Generic;

// namespace GenGuiReloaded
// {
//     public class Tag
//     {
//         private string Line { get; }
//         private int OriginalLineNumber { get; }
//         private int HashLineNumber { get; }

//         public Tag( string line, int originalLN, int hashLN )
//         {
//             Line = line;
//             OriginalLineNumber = originalLN;
//             HashLineNumber = hashLN;
//         }
//     }

//     public class BlockData : Dictionary<string, object>
//     {
//         private Tag Tags { get; }
//         private string BlockName { get; }
//         private string BlockFlag { get; }
//         private int SelectCount { get; }
//         private List<object> SelectedLines { get; }

//         private readonly Dictionary<string, object>? property;

//         public BlockData( Dictionary<string, object>? properties)
//         {
//             Tags = new Tag("", 0, 0);
//             BlockName = "";
//             BlockFlag = "";
//             SelectCount = 0;
//             SelectedLines = new List<object>();

//             property = properties;
//         }

//         public BlockData( )
//         {
//             Tags = new Tag("", 0, 0);
//             BlockName = "";
//             BlockFlag = "";
//             SelectCount = 0;
//             SelectedLines = new List<object>();
//         }
//     }

//     public class MainProgram
//     {

//         public static void Main()
//         {

//             var test = new BlockData();
//             Console.WriteLine($"{test}");


//         }

//     }

// }











using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.RegularExpressions;





string datapath = @"C:\Users\sbker\OneDrive\Desktop\(WS)-GenGui-CrystalStar\Prompt Collections";

string[] files = Directory.GetFiles(datapath, "*.txt");

foreach (var file in files)
{
    Console.WriteLine($"{file}");
}

// string[] lines = File.ReadAllLines(files[0]);

var datalines = new List<string>();
datalines.Capacity = 20000;

foreach (var file in files)
{
    var lines = File.ReadAllLines(file);
    foreach (var line in lines)
    {
        datalines.Add(line);
    }
}

var startofblocks = new OrderedDictionary();
var endofblocks = new OrderedDictionary();
var datacount = datalines.Count;
var blocks = new Dictionary<string,Dictionary<string, object>>();

for ( int i = 0; i < datacount; i++ )
{
    try 
    {
        if (Regex.IsMatch(datalines[i], @"\s?\{.*:.*$"))
        {
            var match = Regex.Match(datalines[i],@"^\s?\{\s?(\S*):.*$");
            string blockname = match.Groups[1].ToString();
            startofblocks.Add(blockname, i);
            var flagmatch = Regex.Match(datalines[i], @".*:(\S*).*$");
            string blockflag = flagmatch.Groups[1].ToString();
            i++;
            var tags = new List<string>();
            while (i < datacount && Regex.IsMatch(datalines[i], @"^\s?\}\s?") == false)
            {
                tags.Add(datalines[i]);
                i++;
            }
            endofblocks.Add(blockname, i);
            var thestuff = new Dictionary<string, object>
            {
                { "Tags", tags },
                { "BlockName", blockname },
                { "BlockFlag", blockflag},
                { "SelectCount", 0},
                { "SelectedLines", new List<string>()}
            };
            blocks.Add(blockname, thestuff);
        } 
        // else if (Regex.IsMatch(line, @"^\s?\}\s?"))
        // {
        //     int laststart = startofblocks.Count - 1;
        //     if (laststart >= 0)
        //     {  // v the fuck is this shit? v , shits easier to do in powershell
        //         var findtheDictionarykey = startofblocks.Cast<DictionaryEntry>().ElementAt(laststart).Key.ToString();
        //         endofblocks.Add(findtheDictionarykey!, i);
        //     }
        //     else 
        //     {
        //         var e = new Exception();
        //         throw e;
        //     }
        // }
        // i++;
    }
    catch
    {
        Console.WriteLine("I died");
        Environment.Exit(0);
    }
} 

try 
{
    if (startofblocks.Count != endofblocks.Count)
    {
        var e = new Exception();
    }
}
catch
{
    Console.WriteLine("I should learn how to properly do these exceptions");
    Environment.Exit(0);
}



foreach (DictionaryEntry item in startofblocks)
{
    Console.WriteLine($"{item.Key}: {item.Value}");
}
foreach (DictionaryEntry item in endofblocks)
{
    Console.WriteLine($"{item.Key}: {item.Value}");
}

foreach ( var item in blocks.Keys)
{
    Console.WriteLine($"{item}");
    Console.WriteLine($"{blocks[$"{item}"]}");
    
}

























