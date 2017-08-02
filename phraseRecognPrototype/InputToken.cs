using System;

namespace WordClassTagger
{
    public class InputToken : IComparable
    {
        public string Content { get; set; }
        public string ContentWithKeptCase { get; set; }
        public string Tag { get; set; }
        // Whether token is a word or a number
        public bool IsWord { get; set; }

        public InputToken(string content, string contentWithKeptCase, long orderInTextIndex, bool isWord, string tag = "<ЕщеНеЗадан>")
        {
            this.Content = content;
            this.ContentWithKeptCase = contentWithKeptCase;
            this.Tag = tag;
            this.IsWord = isWord;
        }

        public override string ToString()
        {
            return Content;
        }

        public override int GetHashCode()
        {
            return Content.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            return Content.CompareTo(obj.ToString());
        }

        public override bool Equals(object other)
        {
            // When comparing with null it must always return false
            if (other == null)
                return false;
            // If comparing objects have different type then equality is not correct
            if (other.GetType() != this.GetType())
                return false;
            if (Content == other.ToString()) return true;
                else return false;
        }
    }
}
