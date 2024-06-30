using System.Collections.Generic;

namespace GenGui_CrystalStar.Exceptions
{
    public class InvalidFlag : System.Exception
    {
        public InvalidFlag() { }
        public InvalidFlag(string message) : base(message) { }
        public InvalidFlag(string message, System.Exception inner) : base(message, inner) { }
    }

    public class UnclosedBlock : System.Exception
    {
        public UnclosedBlock() { }
        public UnclosedBlock(string message) : base(message) { }
        public UnclosedBlock(string message, System.Exception inner) : base(message, inner) { }
    }

    public class GetAllData
    {
        public static string DataContext(List<string> list, int position)
        {
            if ( position > 0 && position < list.Count)
            {
                return $"\nContext:\n Previous Line: {list[position-1]}\n Offending Line: {list[position]}\n Next Line: {list[position + 1]}";
            }
            else if ( position == 0)
            {
                return $"\nContext:\n Previous Line: (Start of File)\n Offending Line: {list[position]}\n Next Line: {list[position + 1]}";
            }
            else
            {
                return $"\nContext:\n Previous Line: (Start of File)\n Offending Line: {list[position]}\n Next Line: (EOF)";
            }
        }
    }
}