using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WordClassTagger
{
    public static class DicOfTaggedWords
    {
        private static string TaggedWordsDicFilePath = @"TaggedWordsDictionary.txt";
        private static List<TaggedWordFromDic> LTaggedDicWordsAsObjects = new List<TaggedWordFromDic>();
        private static List<string> LTaggedDicWordsAsStrings = new List<string>();

        public static List<TaggedWordFromDic> GetLTaggedDicWords()
        {
            return LTaggedDicWordsAsObjects;
        }

        public static void Load()
        {
            // Preparation
            if (LTaggedDicWordsAsObjects.Any()) LTaggedDicWordsAsObjects.Clear();
            if (LTaggedDicWordsAsStrings.Any()) LTaggedDicWordsAsStrings.Clear();
            LTaggedDicWordsAsStrings = File.ReadAllText(TaggedWordsDicFilePath).Split(new Char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            LTaggedDicWordsAsStrings.Sort();
            long taggedDicWordOrderIndex = 1;
            foreach (var taggedDicWord in LTaggedDicWordsAsStrings)
            {
                string grammeme = taggedDicWord.Substring(taggedDicWord.IndexOf('<'));
                LTaggedDicWordsAsObjects.Add(new TaggedWordFromDic(taggedDicWord.Substring(0, taggedDicWord.IndexOf('<')).ToLower(), grammeme, taggedDicWordOrderIndex));
                taggedDicWordOrderIndex++;
            }
            LTaggedDicWordsAsObjects.Sort();
        }
    }
}
