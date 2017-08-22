using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WordClassTagger
{
    public static class TaggedWordsDictionary
    {
        private static List<TaggedWord> TaggedWords = new List<TaggedWord>();
        private static List<string> TaggedWordsAsStrings = new List<string>();

        public static List<TaggedWord> GetLTaggedDicWords()
        {
            return TaggedWords;
        }

        public static void Load()
        {
            // Preparation.
            TaggedWordsAsStrings = File.ReadAllText(ConstantValues.TaggedWordsDicFilePath).Split(new Char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            TaggedWordsAsStrings.Sort();
            foreach (var taggedDicWord in TaggedWordsAsStrings)
            {
                string tag = taggedDicWord.Substring(taggedDicWord.IndexOf('<')); 
                TaggedWords.Add(new TaggedWord(taggedDicWord.Substring(0, taggedDicWord.IndexOf('<')).ToLower(), TagsManager.StringTagToEnumTag(tag)));
            }
        }
    }
}
