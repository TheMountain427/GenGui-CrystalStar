

namespace GenGui_CrystalStar.Code.Exceptions;

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