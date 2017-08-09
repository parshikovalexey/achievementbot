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
            // Comment out these lines for activating user input.
            Console.Write("Enter input text: ");
            string inputText = Console.ReadLine();

            // TODO It would be well to use TestPhrases.txt and create corresponding method in order to cyclically test lines of TestPhrases.txt

            string date = "";
            string time = "";
            // Try to get date and time match from input.
            ParseDateAndTime(inputText, ref date, ref time);
            inputText = RemoveDateAndTimeFromMessage(inputText, date, time);

            // Loading and preparing of tokens.
            InputTextManager.LoadInput(inputText);
            List<InputToken> tokens = InputTextManager.GetTokens();

            // Showing tokens for debugging.
            Console.WriteLine("\r\nЧасти речи входных слов (отладка):");
            foreach (var token in tokens)
            {
                // Only for debugging and showing recognited parts of speech.
                Console.WriteLine(token.ContentWithKeptCase + token.GetStringTag());
            }

            // Getting phrase AMOUNT position by means of using tokens list.
            int amountPosition = -1;
            foreach (var token in tokens)
            {
                if (token.Tag == TagsManager.TagsEnum.NUM)
                    amountPosition = token.OrderInTextIndex;
            }

            // Examples: прочитал, прочитала, прочитали, подтянулся, подтянулись, подтянулась, прочитано, спасено, завершен(о/а), построил, спас, потерял/засеял/посеял, взлетел.
            Regex actionRegex = new Regex(".*((ал(а|и)?)|(лся)|(лись)|(лась)|(ано)|(ено)|(ше|ён(о|а)?)|(ил)|(спас)|(ял)|(ел))$", RegexOptions.IgnoreCase);
            Regex unitsRegex = new Regex(string.Join("|", ConstantValues.Units), RegexOptions.IgnoreCase);
            // Getting action and amount.
            string achievement = "";
            string action = "";
            string unit = "";
            string amount = "";
            foreach (var token in tokens)
            {
                // Phrase ACTION retreaving.
                if ((token.Tag == TagsManager.TagsEnum.V || actionRegex.IsMatch(token.Content)))
                {
                    action = token.ContentWithKeptCase;
                    achievement += "Ты сделал — " + token.ContentWithKeptCase + ".\r\n";
                    // Break the loop if there is only one word, eg. "выспался".
                    if (tokens.Count == 1) break;
                }
                // Amount.
                if ((token.Tag == TagsManager.TagsEnum.NUM || ConstantValues.AmountByWords.Contains(token.Content)))
                {
                    amount = token.ContentWithKeptCase;
                    achievement += "Сколько? — " + token.ContentWithKeptCase + ".\r\n";
                    // Try to get phrase UNIT of measure.
                    if (token.OrderInTextIndex < tokens.Count && unitsRegex.IsMatch(tokens[token.OrderInTextIndex + 1].Content))
                    {
                        unit = tokens[token.OrderInTextIndex + 1].ContentWithKeptCase;
                        achievement += "Чего? — " + unit + ".\r\n";
                    }
                }
            }

            // Filter action, unit and amount out of phrase in order to get phrase OBJECT.
            if (action != "")
                inputText = inputText.Replace(action, "");
            if (unit != "")
                inputText = inputText.Replace(unit, "");
            if (amount != "")
                inputText = inputText.Replace(amount, "");

            // Getting phrase OBJECT.
            inputText = inputText.Trim(new Char[] { ' ', ',', '-', '—', '–', ';' });
            string phraseObject = "";
            if (inputText != "")
            {
                phraseObject = inputText;
                achievement += "Что?/Кому?/Каким?/Куда? — " + inputText + ".\r\n";
            }

            date = date.Trim(new Char[] { ' ', ',' });
            time = time.Trim(new Char[] { ' ', ',' });

            if (achievement == "")
                Console.WriteLine("Некорректный ввод либо не удалось распознать фразу.");
            else
            {
                // In case of time it needs to remove "в" when checking of presence, because the letter might be left alone.
                if (date != "" && time.Trim(new Char[] { 'в' }) != "")
                    achievement += String.Format("Когда? — Дата: {0}. Время: {1}. DateTime: {2}\r\n", date, time, GetDateTimeFromString(date, time));
                else if (date == "" && time.Trim(new Char[] { 'в' }) != "")
                    achievement += String.Format("Когда? — Дата: не указана, будет считаться, что сегодня. Время: {0}. DateTime: {1}\r\n", time, GetDateTimeFromString(date, time));
                else if (date != "" && time.Trim(new Char[] { 'в' }) == "")
                    achievement += String.Format("Когда? — Дата: {0}. Время: не указано, будет использовано текущее. DateTime: {1}\r\n", date, GetDateTimeFromString(date, time));
                else
                    achievement += "Когда? — дата не была указана либо не была распознана, будут использованы текущие дата и время. DateTime: " + GetDateTimeFromString(date, time) + "\r\n";
                Console.WriteLine("\r\nВыходные данные:");
                Console.WriteLine(achievement);
            }
            GetDateTimeFromString(date, time);
            // NOTE idea - if achievement - reading a book then bot would ask what exactly the book is in order to clarify the achievement.
        }

        /// <summary>
        /// Method is aimed for trying to get date and time match from input text message.
        /// </summary>
        private static void ParseDateAndTime(string inputText, ref string date, ref string time)
        {
            Regex dateRE = new Regex(Tokenizer.CommonDatePattern, RegexOptions.IgnoreCase);
            for (Match m = dateRE.Match(inputText); m.Success; m = m.NextMatch())
                if (m.Groups["date"].Success)
                {
                    date = m.Groups["date"].Value;
                    date.ToLower();
                }
                else if (m.Groups["time"].Success)
                {
                    time = m.Groups["time"].Value;
                    date.ToLower();
                }
        }

        /// <summary>
        /// Method for removing date and time from input message in order to make further recognition easier.
        /// </summary>
        private static string RemoveDateAndTimeFromMessage(string inputText, string date, string time)
        {
            if (date != "" || time != "")
            {
                if (date != "")
                    inputText = inputText.Replace(date, " ");
                if (time != "")
                    inputText = inputText.Replace(time, " ");
            }
            return inputText;
        }

        /// <summary>
        /// Method receives corresponding strings and returns DateTime object.
        /// </summary>
        private static DateTime GetDateTimeFromString(string date, string time)
        {
            // Not implemented date cases: на днях, 12.12.2012, 2 сентября 2011 года, 2 сентября

            DateTime parsedDateTime = DateTime.UtcNow;
            ConstantValues.DateUnits dateUnit = ConstantValues.DateUnits.DAYS;

            // Get subtracting word or part of word, eg.: "вчера, прошлом, поза(прошлом|вчера), позапрошлом".
            Regex earlier1RE = new Regex(@"\P{L}прошл", RegexOptions.IgnoreCase);
            Regex earlier2RE = new Regex(@"позапрошл", RegexOptions.IgnoreCase);
            
            // Get unit of date.
            Regex monthsRE = new Regex(@"месяц", RegexOptions.IgnoreCase);
            Regex weeksRE = new Regex(@"недел", RegexOptions.IgnoreCase);
            Regex yearsRE = new Regex(@"год", RegexOptions.IgnoreCase);
            if (monthsRE.IsMatch(date))
                dateUnit = ConstantValues.DateUnits.MONTHS;
            if (weeksRE.IsMatch(date))
                dateUnit = ConstantValues.DateUnits.WEEKS;
            if (yearsRE.IsMatch(date))
                dateUnit = ConstantValues.DateUnits.YEARS;

            if (date == "вчера")
                parsedDateTime = parsedDateTime.AddDays(-1);
            else if (date == "позавчера")
                parsedDateTime = parsedDateTime.AddDays(-2);

            // Taking into account weeks
            if (earlier1RE.IsMatch(date) && dateUnit == ConstantValues.DateUnits.WEEKS)
                parsedDateTime = parsedDateTime.AddDays(-7);
            if (earlier2RE.IsMatch(date) && dateUnit == ConstantValues.DateUnits.WEEKS)
                parsedDateTime = parsedDateTime.AddDays(-14);

            // Taking into account months
            if (earlier1RE.IsMatch(date) && dateUnit == ConstantValues.DateUnits.MONTHS)
                parsedDateTime = parsedDateTime.AddMonths(-1);
            if (earlier2RE.IsMatch(date) && dateUnit == ConstantValues.DateUnits.MONTHS)
                parsedDateTime = parsedDateTime.AddMonths(-2);

            // Taking into account years
            if (earlier1RE.IsMatch(date) && dateUnit == ConstantValues.DateUnits.YEARS)
                parsedDateTime = parsedDateTime.AddYears(-1);
            if (earlier2RE.IsMatch(date) && dateUnit == ConstantValues.DateUnits.YEARS)
                parsedDateTime = parsedDateTime.AddYears(-2);

            // NOTE There is no implementation for "на прошлой неделе в воскресенье" cases, days of the week are considered only as days of current week! Exactly by the reason there is no condition for sunday
            if (date.Contains("понедельник") && (int)parsedDateTime.DayOfWeek > 1)
                parsedDateTime = parsedDateTime.AddDays((double)parsedDateTime.DayOfWeek * (-1.0) + 1);
            if (date.Contains("вторник") && (int)parsedDateTime.DayOfWeek > 2)
                parsedDateTime = parsedDateTime.AddDays((double)parsedDateTime.DayOfWeek * (-1.0));
            if (date.Contains("сред") && (int)parsedDateTime.DayOfWeek > 3)
                parsedDateTime = parsedDateTime.AddDays((double)parsedDateTime.DayOfWeek * (-1.0) - 1);
            if (date.Contains("четверг") && (int)parsedDateTime.DayOfWeek > 4)
                parsedDateTime = parsedDateTime.AddDays((double)parsedDateTime.DayOfWeek * (-1.0) - 2);
            if (date.Contains("пятниц") && (int)parsedDateTime.DayOfWeek > 5)
                parsedDateTime = parsedDateTime.AddDays((double)parsedDateTime.DayOfWeek * (-1.0) - 3);
            if (date.Contains("суббот") && (int)parsedDateTime.DayOfWeek > 6)
                parsedDateTime = parsedDateTime.AddDays((double)parsedDateTime.DayOfWeek * (-1.0) - 4);

            // TODO Implement the method completely, including time parsing.
            return parsedDateTime;
        }
    }
}
