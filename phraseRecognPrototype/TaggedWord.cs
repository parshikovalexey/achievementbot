using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordClassTagger
{
    public class TaggedWord : IComparable
    {
        public string Word { get; set; }
        public int Tag { get; set; }

        public TaggedWord(string word, int tag, long orderIndex)
        {
            this.Word = word;
            this.Tag = tag;
        }

        public TaggedWord(string word)
        {
            this.Word = word;
        }


        public string GetStringTag()
        {
            return Tags.EnumTagToStringTag(Tag);
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

