using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WordClassTagger
{
    public static class Tokenizer
    {
        const string NonAlphanumericAssertion = @"(?=(\W|$))"; // ?= is a positive lookahead
        public const string Number = @"(?<num>-?(?:\d+)(?:[.,/]\d+)*(?!-))";
        const string EndingAssertion = @"(?:(?:\r\n)*)";

        static readonly RegexOptions TokenREoptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;
        static readonly Regex TokenRE;
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

            Number,
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
            long inputTokenOrderInTextIndex = 1;

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
