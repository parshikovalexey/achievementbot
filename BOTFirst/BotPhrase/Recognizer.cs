﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WordClassTagger;
using EntityModel;



namespace BotPhrase
{
 public class Recognizer
    {
       
       public static EntityModel.Phrase GetModelPhraseFromMessage(string inputText)
        {
            // Объект из BotEngine для получени и выводя информации пользователю.
            // Comment out these lines for activating user input.
            //Console.Write("Enter input text: ");
            // Получение текста из PhraseDialog.
            

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
           /* Console.WriteLine("\r\nЧасти речи входных слов (отладка):");
            foreach (var token in tokens)
            {
                // Only for debugging and showing recognited parts of speech.
                Console.WriteLine(token.ContentWithKeptCase + token.GetStringTag());
            }*/

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

            // Создаём объект описывающий фразу.
            Phrase phrase = new Phrase();
             
            string action = "";
            string unit = "";
            string amount = "";
            foreach (var token in tokens)
            {
                // Phrase ACTION retreaving.
                if ((token.Tag == TagsManager.TagsEnum.V || actionRegex.IsMatch(token.Content)))
                {
                    action = token.ContentWithKeptCase;
                    phrase.YouDid += "Ты сделал — " + token.ContentWithKeptCase + ".\r\n";
                    // Break the loop if there is only one word, eg. "выспался".
                    if (tokens.Count == 1) break;
                }
                // Amount.
                if ((token.Tag == TagsManager.TagsEnum.NUM || ConstantValues.AmountByWords.Contains(token.Content)))
                {
                    amount = token.ContentWithKeptCase;
                    phrase.HowMuch += "Сколько? — " + token.ContentWithKeptCase + ".\r\n";
                    // Try to get phrase UNIT of measure.
                    if (token.OrderInTextIndex < tokens.Count && unitsRegex.IsMatch(tokens[token.OrderInTextIndex + 1].Content))
                    {
                        unit = tokens[token.OrderInTextIndex + 1].ContentWithKeptCase;
                        phrase.Units += "Чего? — " + unit + ".\r\n";
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
                phrase.WhatWhere += "Что?/Кому?/Каким?/Куда? — " + inputText + ".\r\n";
            }

            date = date.Trim(new Char[] { ' ', ',' });
            time = time.Trim(new Char[] { ' ', ',' });

            if (phrase.YouDid == null && phrase.Units == null && phrase.WhatWhere == null)
                //
                return  new EntityModel.Phrase
                {
                    OriginalMessage = inputText,
                    WasRecognized = false,
                    Date = date,
                    Time = time,
                    Amount =decimal.Parse(amount),
                    MeasureUnit =new EntityModel.MeasureUnit() { Text = unit},
                    CorrectedMessage = "Коррекция правописания еще не реализована"                  
                };
            else
            {
                DateTime dateTime = GetDateTimeFromString(date, time);
                // In case of time it needs to remove "в" when checking of presence, because the letter might be left alone.
                if (date != "" && time.Trim(new Char[] { 'в' }) != "")
                    phrase.Date += String.Format("Когда? — Дата: {0}. Время: {1}. DateTime: {2}\r\n", date, time, dateTime);
                else if (date == "" && time.Trim(new Char[] { 'в' }) != "")
                    phrase.Date += String.Format("Когда? — Дата: не указана, будет считаться, что сегодня. Время: {0}. DateTime: {1}\r\n", time, dateTime);
                else if (date != "" && time.Trim(new Char[] { 'в' }) == "")
                    phrase.Date += String.Format("Когда? — Дата: {0}. Время: не указано, будет использовано текущее. DateTime: {1}\r\n", date, dateTime);
                else
                    phrase.Date += "Когда? — дата не была указана либо не была распознана, будут использованы текущие дата и время. DateTime: " + dateTime + "\r\n";
                return new EntityModel.Phrase
                {
                    Action = new EntityModel.Action() { Text = action },
                    OriginalMessage = inputText,
                    WasRecognized = true,
                    Date = date,
                    Time = time,
                    Amount = decimal.Parse(amount),
                    MeasureUnit = new EntityModel.MeasureUnit() { Text = unit },
                    CorrectedMessage = "Коррекция правописания еще не реализована",
                    AdditionalText = new EntityModel.AdditionalText() { Text = phraseObject }
                };
            }
            //Console.ReadLine();
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

            if (earlier1RE.IsMatch(date))
            {
                switch (dateUnit)
                {
                    // Taking into account weeks
                    case ConstantValues.DateUnits.WEEKS: parsedDateTime = parsedDateTime.AddDays(-7); break;
                    // Taking into account months
                    case ConstantValues.DateUnits.MONTHS: parsedDateTime = parsedDateTime.AddMonths(-1); break;
                    // Taking into account years
                    case ConstantValues.DateUnits.YEARS: parsedDateTime = parsedDateTime.AddYears(-1); break;
                }
            }
            if (earlier2RE.IsMatch(date))
            {
                switch (dateUnit)
                {
                    // Taking into account weeks
                    case ConstantValues.DateUnits.WEEKS: parsedDateTime = parsedDateTime.AddDays(-14); break;
                    // Taking into account months
                    case ConstantValues.DateUnits.MONTHS: parsedDateTime = parsedDateTime.AddMonths(-2); break;
                    // Taking into account years
                    case ConstantValues.DateUnits.YEARS: parsedDateTime = parsedDateTime.AddYears(-2); break;
                }
            }

            var daysOfWeek = new List<string> { "понедельник", "вторник", "сред", "четверг", "пятниц", "суббот", "воскресен" };
            int i = 0;
            foreach (var day in daysOfWeek)
            {
                if (date.Contains(day))
                {
                    var subDays = ((int)parsedDateTime.DayOfWeek - 1) % 7;
                    //Mod for negative numbers return negaitve number here: because it we increase it to 7 manually to switch days numeration from Monday as start.
                    if (subDays < 0) subDays += 7;
                    //Make a day to start of week
                    parsedDateTime = parsedDateTime.AddDays((double)-subDays);
                    //Add necessary days
                    parsedDateTime = parsedDateTime.AddDays((double)i);
                    break;
                }
                i++;
            }

            // TODO Implement the method completely, including time parsing.
            return parsedDateTime;
        }
    }
}


