using System.Collections.Generic;
using System.Text.RegularExpressions;
using PhraseRecognPrototype;

namespace WordClassTagger
{
    public static class Tokenizer
    {
        const string NonAlphanumericAssertion = @"(?=(\W|$))"; // ?= is a positive lookahead
        public const string Number = @"(?<num>-?(?:\d+)(?:[.,/]\d+)*(?!-))";
        const string EndingAssertion = @"(?:(?:\r\n)*)";
        static readonly RegexOptions TokenREoptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;
        static readonly Regex TokenRE;


        private static readonly string monthsPattern = @"(?:(" + string.Join("|", ConstantValues.Months) + "))";
        private static readonly string seasonsPattern = @"(?:(" + string.Join("|", ConstantValues.Seasons) + "))";
        // NOTE users must type time only after date, also date and time must be at the beginning or at the end of a message
        private static readonly string timePattern = @"(?<time>([ \t\b]*в[ \t\b]*\d{1,2}[:_ ]\d{1,2})|([ \t\b]*в[ \t\b]*\d{0,2}([ \t\b]*час((а)|(ов))?)?([ \t\b]*\d{1,2} м\w*.?)?)([ \t\b]*((дня)|(вечера)|(утра)|(ночи)))?)";
        // Older patterns for time, if it would be turned out that current pattern works incorrectly
        // v1 string timePattern = @"(?<time>([в \t\b]*\d{1,2}[:_ ]\d{1,2})|([в \t\b]*\d{1,2}([ \t\b]*час((а)|(ов)))?([ \t\b]*((дня)|(вечера)|(утра)|(ночи)))?([ \t\b]*\d{1,2} мин)?))";
        // v2 string timePattern = @"(?<time>([в \t\b]*\d{1,2}[:_ ]\d{1,2})|([в \t\b]*\d{1,2}([ \t\b]*час((а)|(ов)))?([ \t\b]*\d{1,2} м\w*.?)?)([ \t\b]*((дня)|(вечера)|(утра)|(ночи)))?)";
        private static readonly string[] datePatterns = new string[]
        {
            // Alphabetic dates
            @"(?<date>(" + string.Join("|", ConstantValues.AlphabeticDates) + "))",

            // dayAndMonth - some day (ex: 20, 2, 02, десятое) and some month as a word and, maybe, some year
            @"(?<date>((((\d{1,2})|(\w+(ое)|(го)))[ \t\b]*" + monthsPattern + @")|" + seasonsPattern + @")[ \t\b]*(\d{1,4}(\s*год(а|у)?)?)?)",

            // examples: 20[. /-]12, 20[. /-]12, 11[. /-]12[. /-]2012, 2[. /-]10 and maybe time
            @"(?<date>\d{1,2}([./\- ]\d{1,4}){1,3})"
        };
        // Combinated pattern
        public static string CommonDatePattern = "(" + string.Join("|", datePatterns) + ")?" + timePattern + "?";


        static readonly string[] tokenPatterns = new string[]
        {
            // URLs
            @"(?<url>((?:https?|ftp)://)(?:www\.)?(?:\p{L}+\.)+\p{L}{2,3}(?:/\S+)*" + NonAlphanumericAssertion + ")" + EndingAssertion,

            // Emails
            @"(?<email>\p{L}+@(\p{L}+\.)+\p{L}+)" + EndingAssertion,

            // html/xml tags
            @"(?<tag></?[a-zA-Z\d=""'-_\s]{1,30}>)" + EndingAssertion,

            // word
            @"(?<word>(\d+-)?[\p{L}]+(?:(?:'|-|’)?[\p{L}]+)*)",

            Number
        };

        static Tokenizer()
        {
            if (TokenRE == null)
            {
                var s = string.Join("|", tokenPatterns);
                TokenRE = new Regex(s, TokenREoptions | RegexOptions.Compiled);
            }
        }

        public static Regex GetTokenRE()
        {
            return TokenRE;
        }

        /// <summary>
        /// Method for getting list of tokens
        /// </summary>
        /// <param name="inputText">String of text to tokenize</param>
        public static List<InputToken> GetTokensListFromInputText(string inputText)
        {
            List<InputToken> inputTokens = new List<InputToken>();
            int inputTokenOrderInTextIndex = 0;

            int inputTokenOrderInSentenceIndex = 1;
            for (Match m = TokenRE.Match(inputText); m.Success; m = m.NextMatch())
            {
                // Don't capture empty matches
                if (m.Value.Length == 0) continue;
                // Don't capture URLs, emails and tags
                if (m.Groups["url"].Success || m.Groups["email"].Success || m.Groups["tag"].Success) continue;

                InputToken newInputToken = new InputToken(m.Value.ToLower(), m.Value, inputTokenOrderInTextIndex, m.Groups["word"].Success);
                inputTokens.Add(newInputToken);
                inputTokenOrderInSentenceIndex++;
                inputTokenOrderInTextIndex++;
            }
            return inputTokens;
        }
    }
}
