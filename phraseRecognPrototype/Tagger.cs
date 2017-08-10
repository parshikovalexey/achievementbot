using System.Text.RegularExpressions;

namespace WordClassTagger
{
    public static class Tagger
    {
        // getting already defined and initialized pattern in order to "DRY".
        private static Regex NumeralRE = new Regex(Tokenizer.Number);
        // e.g. 30-летний, 20-летие, 2-месячный.
        private static Regex NumeralAndWord = new Regex(@"\d+-\p{L}");

        public static void DefineAndAppendTagToWord(InputToken token)
        {
            if (NumeralRE.IsMatch(token.Content) && !token.IsWord)
            {
                token.Tag = TagsManager.TagsEnum.NUM;
            }
            else if (NumeralAndWord.IsMatch(token.Content))
            {
                // Because there is no way to define yet, an idea is using separate dictionary containing parts of word after hyphen.
                // NOTE Of course this condition branch maybe omitted but it needs for further scaling and adding mentioned above feature.
                token.Tag = TagsManager.TagsEnum.UNRECOGNIZED;
            }
            else
            {
                // Recognition by tagged words dictionary.
                int searchResult = TaggedWordsDictionary.GetLTaggedDicWords().BinarySearch(new TaggedWord(token.Content));
                if (searchResult >= 0)
                    token.Tag = TaggedWordsDictionary.GetLTaggedDicWords()[searchResult].Tag;
                else
                    token.Tag = TagsManager.TagsEnum.UNRECOGNIZED;
            }
        }
    }
}
