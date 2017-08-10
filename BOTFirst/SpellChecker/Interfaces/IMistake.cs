using System.Collections.Generic;


namespace BOTFirst.Spellchecker
{
    public interface IMistake
    {
        string Original { get; set; }
        Position Position { get; set; }
        List<string> Replacements { get; set; }
        
    }
    public struct Position
    {
        public int Begin;
        public int Length;
        public Position (int begin, int len)
        {
            Begin = begin;
            Length = len;
        }
    }
}
