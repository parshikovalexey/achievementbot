using System.Collections.Generic;


namespace BOTFirst.Spellchecker
{
    public abstract class Mistake
    {
        public string Original { get; set; }
        public Position Position { get; set; }
        public List<string> Replacements { get; set; }
        
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
