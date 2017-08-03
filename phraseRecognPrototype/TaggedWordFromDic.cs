using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordClassTagger
{
    public class TaggedWordFromDic : IComparable
    {
        public string Word { get; set; }
        public string Grammeme { get; set; }

        public TaggedWordFromDic(string word, string grammeme, long orderIndex)
        {
            this.Word = word;
            this.Grammeme = grammeme;
        }

        public TaggedWordFromDic(string word)
        {
            this.Word = word;
        }

        public override string ToString()
        {
            return Word;
        }

        public override bool Equals(object other)
        {
            if (this.Word == other.ToString()) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return Word.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            return Word.CompareTo(obj.ToString());
        }
    }
}

