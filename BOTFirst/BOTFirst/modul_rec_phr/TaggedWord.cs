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
        public TagsManager.TagsEnum Tag { get; set; }

        public TaggedWord(string word, TagsManager.TagsEnum tag)
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
            return TagsManager.EnumTagToStringTag(Tag);
        }

        public override string ToString()
        {
            return Word;
        }

        public override bool Equals(object other)
        {
            // When comparing with null it must always return false.
            if (other == null)
                return false;
            // If comparing objects have different type then equality is not correct.
            if (other.GetType() != this.GetType())
                return false;
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

