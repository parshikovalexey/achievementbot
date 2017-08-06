using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WordClassTagger;

namespace PhraseRecognPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter input text: ");
            string inputText = Console.ReadLine();

            // Loading and preparing
            InputTextManager.LoadInput(inputText);
            TaggedWordsDictionary.Load();

            // Tagging of input tokens and showing them for debugging
            Console.WriteLine("\r\nЧасти речи входных слов (отладка):");
            foreach (var token in InputTextManager.GetTokens())
            {
                // Execute tagging of the token
                Tagger.DefineAndAppendTagToWord(token);
                // Only for debugging and showing recognited parts of speech
                Console.WriteLine(token.ContentWithKeptCase + token.GetStringTag());
            }

            string monthsPattern = @"(?:(января)|(февраля)|(марта)|(апреля)|(мая)|(июня)|(июля)|(августа)|(сентября)|(октября)|(ноября)|(декабря))";
            // examples: осенью 2012 года, летом 12 года
            string seasonsPattern = @"(?:(летом?)|(осень)|(осенью)|(зима)|(зимой)|(весна)|(весной))";
            // NOTE users must type time only after date, also date and time must be at the beginning or at the end of a message
            string timePattern = @"(?<time>([ \t\b]*в[ \t\b]*\d{1,2}[:_ ]\d{1,2})|([ \t\b]*в[ \t\b]*\d{0,2}([ \t\b]*час((а)|(ов))?)?([ \t\b]*\d{1,2} м\w*.?)?)([ \t\b]*((дня)|(вечера)|(утра)|(ночи)))?)";

            // older patterns for time, if it would be turned out that current pattern works incorrectly
            // v1 string timePattern = @"(?<time>([в \t\b]*\d{1,2}[:_ ]\d{1,2})|([в \t\b]*\d{1,2}([ \t\b]*час((а)|(ов)))?([ \t\b]*((дня)|(вечера)|(утра)|(ночи)))?([ \t\b]*\d{1,2} мин)?))";
            // v2 string timePattern = @"(?<time>([в \t\b]*\d{1,2}[:_ ]\d{1,2})|([в \t\b]*\d{1,2}([ \t\b]*час((а)|(ов)))?([ \t\b]*\d{1,2} м\w*.?)?)([ \t\b]*((дня)|(вечера)|(утра)|(ночи)))?)";

            string[] datePatterns = new string[]
            {
                // date as word
                @"(?<date>((сегодня)|(вчера)|(позавчера)|(на этой неделе)|(в этом месяце)|(на днях)|(понедельник)|(вторник)|(сред(а|у))|(четверг)|(пятниц(а|у))|(суббот(а|у))|(воскресенье)))",

                // dayAndMonth - some day (ex: 20, 2, 02, десятое) and some month as a word and, maybe, some year
                @"(?<date>((((\d{1,2})|(\w+(ое)|(го)))[ \t\b]*" + monthsPattern + @")|" + seasonsPattern + @")[ \t\b]*(\d{1,4}(\s*год(а|у)?)?)?)",

                // examples: 20[. /-]12, 20[. /-]12, 11[. /-]12[. /-]2012, 2[. /-]10 and maybe time
                @"(?<date>\d{1,2}([./\- ]\d{1,4}){1,3})"
            };

            // Common pattern
            string commonDatePattern = "(" + string.Join("|", datePatterns);
            commonDatePattern += ")?" + timePattern + "?";
            Regex dateRE = new Regex(commonDatePattern, RegexOptions.IgnoreCase);

            // Try to get date and time match from input
            Match m = dateRE.Match(inputText);
            // Removing date and time from input message in order to make further recognition easier
            if (m.Value != null)
            {
                if (m.Groups["date"].Value != null)
                    inputText = inputText.Replace(m.Groups["date"].Value, "").Trim(new Char[] { ' ' });
                if (m.Groups["time"].Value != null)
                    inputText = inputText.Replace(m.Groups["time"].Value, "").Trim(new Char[] { ' ' });
            }

            // 1/2, 1/10 etc. will be recognized by tagger
            List<string> amountByWords = new List<string> { "половину", "треть", "четверть", "много", "немного", "уйму", "кучу", "тучу" };
            
            // Examples: прочитал, прочитала, прочитали, подтянулся, подтянулись, подтянулась, прочитано, спасено, завершен(о/а), построил, спас, потерял/засеял/посеял, взлетел
            Regex actionRegex = new Regex(".*((ал(а|и)?)|(лся)|(лись)|(лась)|(ано)|(ено)|(ше|ён(о|а)?)|(ил)|(спас)|(ял)|(ел))$");
            
            Console.WriteLine("\r\nВыходные данные:");
            // 1 value is up to ACTION, 2 value is up to AMOUNT, 3 value is up to OBJECT
            int conditionalIndex = 0;
            string achievement = "";
            foreach (var token in InputTextManager.GetTokens())
            {
                // Action
                if ((token.Tag == (int)TagsManager.TagsEnum.V || actionRegex.IsMatch(token.Content)) && conditionalIndex == 0)
                {
                    achievement += "Ты сделал — " + token.ContentWithKeptCase + ".\r\n";
                    conditionalIndex++;
                    // Break the loop if there is only one word, eg. "выспался"
                    if (InputTextManager.GetTokens().Count == 1) break;
                    // To handle "нашел клад, помог другу, сдал тест" cases
                    // There is checking whether there is some next words in order to take into account "стал счастливым" case
                    if (InputTextManager.GetTokens()[token.OrderInTextIndex + 1].Tag != (int)TagsManager.TagsEnum.NUM && token.OrderInTextIndex < InputTextManager.GetTokens().Count)
                    {
                        // Because there is no amount in such cases
                        conditionalIndex++;
                    }
                }
                // Amount
                else if ((token.Tag == (int)TagsManager.TagsEnum.NUM || amountByWords.Contains(token.Content)) && conditionalIndex == 1)
                {
                    achievement += "Сколько? — " + token.ContentWithKeptCase + ".\r\n";
                    conditionalIndex++;
                }
                // Object
                else if (conditionalIndex == 2)
                {
                    string tempObj = "";
                    // Get the rest of composite object (if it is composite, eg. суть ООП, тест по информатике)
                    for (int i = token.OrderInTextIndex; i < InputTextManager.GetTokens().Count; i++)
                        tempObj += " " + InputTextManager.GetTokens()[i].ContentWithKeptCase;
                    achievement += "Чего?/Что?/Кому?/Каким? — " + tempObj + ".\r\n";
                    break;
                }
            }

            if (achievement == "")
                Console.WriteLine("Некорректный ввод либо не удалось распознать фразу.");
            else
            {
                // There is using of trim because I failed correct regexp for time so that it does not capture spaces and "в" letter
                if (m.Groups["date"].Value != "" && m.Groups["time"].Value.Trim(new Char[] { 'в', ' ' }) != "")
                    achievement += String.Format("Когда? — Дата: {0}. Время: {1}.\r\n", m.Groups["date"].Value, m.Groups["time"].Value.Trim(new Char[] { 'в', ' ' }));
                else if (m.Groups["date"].Value == "" && m.Groups["time"].Value.Trim(new Char[] { 'в', ' ' }) != "")
                    achievement += String.Format("Когда? — Дата: не указана, будет считаться, что сегодня. Время: {0}.\r\n", m.Groups["time"].Value.Trim(new Char[] { 'в', ' ' }));
                else if (m.Groups["date"].Value != "" && m.Groups["time"].Value.Trim(new Char[] { 'в', ' ' }) == "")
                    achievement += String.Format("Когда? — Дата: {0}. Время: не указано, будет использовано текущее.\r\n", m.Groups["date"].Value);
                else
                    achievement += "Когда? — дата не была указана либо не была распознана, будут использованы текущие дата и время\r\n";
                Console.WriteLine(achievement);
            }

            // NOTE idea - if achievement - reading a book then bot would ask what exactly the book is in order to clarify the achievement
        }
    }
}
