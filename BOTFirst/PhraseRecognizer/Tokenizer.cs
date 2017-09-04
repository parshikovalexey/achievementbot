using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace WordClassTagger {
    public static class Tokenizer {
        // ?= is a positive lookahead here.
        const string NonAlphanumericAssertion = @"(?=(\W|$))";
        public const string Number = @"(?<num>-?(?:\d+)(?:[.,/]\d+)*(?!-))";
        const string EndingAssertion = @"(?:(?:\r\n)*)";
        static readonly RegexOptions TokenREoptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;
        static readonly Regex TokenRE;

        private static readonly string MonthsPattern = @"(?:(" + string.Join("|", ConstantValues.Months) + "))";
        private static readonly string SeasonsPattern = @"(?:(" + string.Join("|", ConstantValues.Seasons) + "))";
        // NOTE Users must type time only after date, also date and time must be at the beginning or at the end of a message.
        // Matching time examples (half-regular sintax for code minimization): в 2 часа (10 минут)? ночи|дня|утра|вечера, в 10[:_ ]00, ночью|днем|утром|вечером.
        private static readonly string TimePattern = @"(?<time>((((в[ \t\b]*)?\d{1,2}[:_ ]\d{1,2})|((?=[ \t\b]*)в\s+[ \t\b]*\d{0,2}([ \t\b]*час((а)|(ов))?)?([ \t\b]*\d{1,2} ?м\p{L}*\.?)?))([ \t\b]*((дня)|(вечера)|(утра)|(ночи)))?)|((ночью)|(днем)|(утром)|(вечером)|(под утро)|(под вечер)))";
        // Older patterns for time, if it would be turned out that current pattern works incorrectly.
        // v1 @"(?<time>([в \t\b]*\d{1,2}[:_ ]\d{1,2})|([в \t\b]*\d{1,2}([ \t\b]*час((а)|(ов)))?([ \t\b]*((дня)|(вечера)|(утра)|(ночи)))?([ \t\b]*\d{1,2} мин)?))";
        // v2 @"(?<time>([в \t\b]*\d{1,2}[:_ ]\d{1,2})|([в \t\b]*\d{1,2}([ \t\b]*час((а)|(ов)))?([ \t\b]*\d{1,2} м\w*.?)?)([ \t\b]*((дня)|(вечера)|(утра)|(ночи)))?)";
        // v3 @"(?<time>((([ \t\b]*в[ \t\b]*\d{1,2}[:_ ]\d{1,2})|([ \t\b]*в[ \t\b]*\d{0,2}([ \t\b]*час((а)|(ов))?)?([ \t\b]*\d{1,2} ?м\p{L}*\.?)?))([ \t\b]*((дня)|(вечера)|(утра)|(ночи)))?)|(ночью)|(днем)|(утром)|(вечером))";
        // v4 @"(?<time>((([ \t\b]*в?[ \t\b]*\d{1,2}[:_ ]\d{1,2})|((?=[ \t\b]*)в\s+[ \t\b]*\d{0,2}([ \t\b]*час((а)|(ов))?)?([ \t\b]*\d{1,2} ?м\p{L}*\.?)?))([ \t\b]*((дня)|(вечера)|(утра)|(ночи)))?)|((ночью)|(днем)|(утром)|(вечером)|(под утро)|(под вечер)))";
        private static readonly string[] DatePatterns = new string[]
        {
            // Alphabetic dates.
            @"((?=[ \t\b]*)(во?)?[ \t\b,]*(" + string.Join("|", ConstantValues.AlphabeticDates) + "))",

            // Some day (ex: 20, 2, 02, десятое) and some month as a word and, maybe, some year.
            @"(((((\d{1,2})|(\p{L}+(ое)|(го)))[ \t\b]*" + MonthsPattern + @")|" + SeasonsPattern + @")[ \t\b]*(\d{1,4}(\s*год(а|у)?)?)?)",

            // Examples: 20[. /-]12, 20[. /-]12, 11[. /-]12[. /-]2012, 2[. /-]10 and maybe time.
            @"(\d{1,2}([./\- ]\d{1,4}){1,3})"
        };
        // Combinated pattern.
        // Presence of some words between date and time is allowed.
        public static string CommonDatePattern = "(?<date>" + string.Join("|", DatePatterns) + ")|" + TimePattern;

        static readonly string[] TokenPatterns = new string[]
        {
            // URLs.
            @"(?<url>((?:https?|ftp)://)(?:www\.)?(?:\p{L}+\.)+\p{L}{2,3}(?:/\S+)*" + NonAlphanumericAssertion + ")" + EndingAssertion,

            // Emails.
            @"(?<email>\p{L}+@(\p{L}+\.)+\p{L}+)" + EndingAssertion,

            // html/xml tags.
            @"(?<tag></?[a-zA-Z\d=""'-_\s]{1,30}>)" + EndingAssertion,

            // Words.
            @"(?<word>(\d+-)?[\p{L}]+(?:(?:'|-|’)?[\p{L}]+)*)",

            Number
        };

        static Tokenizer() {
            if (TokenRE == null) {
                var s = string.Join("|", TokenPatterns);
                TokenRE = new Regex(s, TokenREoptions | RegexOptions.Compiled);
            }
        }

        public static Regex GetTokenRE() {
            return TokenRE;
        }

        /// <summary>
        /// Method for getting list of tokens.
        /// </summary>
        /// <param name="inputText">String of text to tokenize</param>
        public static List<InputToken> GetTokensListFromInputText(string inputText) {
            List<InputToken> inputTokens = new List<InputToken>();
            int inputTokenOrderInTextIndex = 0;

            int inputTokenOrderInSentenceIndex = 1;
            for (Match m = TokenRE.Match(inputText); m.Success; m = m.NextMatch()) {
                // Don't capture empty matches.
                if (m.Value.Length == 0) continue;
                // Don't capture URLs, emails and tags.
                if (m.Groups["url"].Success || m.Groups["email"].Success || m.Groups["tag"].Success) continue;

                var newInputToken = new InputToken(m.Value.ToLower(), m.Value, inputTokenOrderInTextIndex, m.Groups["word"].Success);
                inputTokens.Add(newInputToken);
                inputTokenOrderInSentenceIndex++;
                inputTokenOrderInTextIndex++;
            }
            return inputTokens;
        }
    }
}
