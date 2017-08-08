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
            
            // Try to get date and time match from input
            Regex dateRE = new Regex(Tokenizer.CommonDatePattern, RegexOptions.IgnoreCase);
            Match m = dateRE.Match(inputText);

            // HACK string type is used temporarily, there will be DateTime type after implementation "string date and time" -> DateTime parsing
            string date = m.Groups["date"].Value;
            string time = m.Groups["time"].Value;
            date = date.Trim(new Char[] { ' ' });
            time = time.Trim(new Char[] { ' ' });
            RemoveDateAndTimeFromMessage(date, time, ref inputText);

            // Examples: прочитал, прочитала, прочитали, подтянулся, подтянулись, подтянулась, прочитано, спасено, завершен(о/а), построил, спас, потерял/засеял/посеял, взлетел
            Regex actionRegex = new Regex(".*((ал(а|и)?)|(лся)|(лись)|(лась)|(ано)|(ено)|(ше|ён(о|а)?)|(ил)|(спас)|(ял)|(ел))$");
            Regex unitsRegex = new Regex(string.Join("|", ConstantValues.Units));

            // Loading and preparing
            InputTextManager.LoadInput(inputText);
            List<InputToken> GotTokens = InputTextManager.GetTokens();

            // Showing tokens for debugging
            Console.WriteLine("\r\nЧасти речи входных слов (отладка):");
            foreach (var token in GotTokens)
            {
                // Only for debugging and showing recognited parts of speech
                Console.WriteLine(token.ContentWithKeptCase + token.GetStringTag());
            }

            // Getting phrase AMOUNT position by means of using tokens list
            int amountPosition = -1;
            foreach (var token in GotTokens)
            {
                if (token.Tag == TagsManager.TagsEnum.NUM)
                    amountPosition = token.OrderInTextIndex;
            }

            // Getting action and amount
            string achievement = "";
            string action = "";
            string unit = "";
            string amount = "";
            foreach (var token in GotTokens)
            {
                // Phrase ACTION retreaving
                if ((token.Tag == TagsManager.TagsEnum.V || actionRegex.IsMatch(token.Content)))
                {
                    action = token.ContentWithKeptCase;
                    achievement += "Ты сделал — " + token.ContentWithKeptCase + ".\r\n";
                    // Break the loop if there is only one word, eg. "выспался"
                    if (GotTokens.Count == 1) break;
                }
                // Amount
                if ((token.Tag == TagsManager.TagsEnum.NUM || ConstantValues.AmountByWords.Contains(token.Content)))
                {
                    // Try to get phrase UNIT of measure
                    if (token.OrderInTextIndex < GotTokens.Count && unitsRegex.IsMatch(GotTokens[token.OrderInTextIndex + 1].Content))
                    {
                        amount = token.ContentWithKeptCase;
                        unit = GotTokens[token.OrderInTextIndex + 1].ContentWithKeptCase;
                        achievement += "Сколько? — " + token.ContentWithKeptCase + ".\r\nЧего? — " + unit + ".\r\n";
                    }
                    else
                    {
                        amount = token.ContentWithKeptCase;
                        achievement += "Сколько? — " + token.ContentWithKeptCase + ".\r\n";
                    }
                }
            }

            // Filter action, unit and amount out of phrase in order to get phrase OBJECT
            if (action != "")
                inputText = inputText.Replace(action, "");
            if (unit != "")
                inputText = inputText.Replace(unit, "");
            if (amount != "")
                inputText = inputText.Replace(amount, "");

            inputText = inputText.Trim(new Char[] { ' ', ',' });
            // Getting phrase OBJECT
            if (inputText != "")
                achievement += "Что?/Кому?/Каким? — " + inputText + ".\r\n";

            // There is using of trim because I failed correct regexp for time so that it does not capture spaces and "в" letter
            date = date.Trim(new Char[] { ' ', ',' });
            time = time.Trim(new Char[] { ' ', ',', 'в' });

            if (achievement == "")
                Console.WriteLine("Некорректный ввод либо не удалось распознать фразу.");
            else
            {
                if (date != "" && time != "")
                    achievement += String.Format("Когда? — Дата: {0}. Время: {1}.\r\n", date, time);
                else if (date == "" && time != "")
                    achievement += String.Format("Когда? — Дата: не указана, будет считаться, что сегодня. Время: {0}.\r\n", time);
                else if (date != "" && time == "")
                    achievement += String.Format("Когда? — Дата: {0}. Время: не указано, будет использовано текущее.\r\n", date);
                else
                    achievement += "Когда? — дата не была указана либо не была распознана, будут использованы текущие дата и время\r\n";
                Console.WriteLine("\r\nВыходные данные:");
                Console.WriteLine(achievement);
            }
            ParseDate(date, time);
            // NOTE idea - if achievement - reading a book then bot would ask what exactly the book is in order to clarify the achievement
        }

        private static DateTime ParseDate(string date, string time)
        {
            DateTime parsedDateTime = DateTime.UtcNow;

            switch (date)
            {
                case "вчера":
                    parsedDateTime = DateTime.UtcNow;
                    parsedDateTime = parsedDateTime.AddDays(-1);
                    break;
                case "позавчера":
                    parsedDateTime = DateTime.UtcNow;
                    parsedDateTime = parsedDateTime.AddDays(-2);
                    break;
                // ...
                default:
                    break;
            }
            // TODO Implement the method completely
            return parsedDateTime;
        }

        /// <summary>
        /// Method for removing date and time from input message in order to make further recognition easier
        /// </summary>
        private static void RemoveDateAndTimeFromMessage(string date, string time, ref string inputText)
        {
            if (date != "" || time != "")
            {
                if (date != "")
                    inputText = inputText.Replace(date, " ").Trim(new Char[] { ' ' });
                if (time != "")
                    inputText = inputText.Replace(time, " ").Trim(new Char[] { ' ' });
            }
        }
    }
}
