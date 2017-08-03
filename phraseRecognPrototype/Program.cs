using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WordClassTagger;

namespace phraseRecognPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter input text: ");
            string inputText = Console.ReadLine();

            // Loading and preparing
            ManagerOfInputText.LoadInput(inputText);
            DicOfTaggedWords.Load();

            Console.WriteLine("\r\nЧасти речи входных слов (отладка):");
            foreach (var token in ManagerOfInputText.GetTokens())
            {
                // Execute tagging of the token
                Tagger.DefineAndAppendTagToWord(token);
                // Only for debugging and showing recognited grammemes
                Console.WriteLine(token.ContentWithKeptCase + token.Tag);
            }

            Console.WriteLine("\r\nВыходные данные:");
            string achievement = "";
            List<string> amountByWords = new List<string> { "половину", "треть", "четверть" };
            // Examples: прочитал, прочитала, прочитали, подтянулся, подтянулись, подтянулась, прочитано, спасено, завершен(о/а), построил, спас, потерял/засеял/посеял, взлетел
            Regex ActionRegex = new Regex(".*(ал(а|и)?)|(лся)|(лись)|(лась)|(ано)|(ено)|(ше|ён(о|а)?)|(ил)|(спас)|(ял)|(ел)$");
            // 1 value is up to ACTION, 2 value is up to AMOUNT, 3 value is up to OBJECT
            int conditionalIndex = 0;
            foreach (var token in ManagerOfInputText.GetTokens())
            {
                if ((token.Tag == "<V>" || ActionRegex.IsMatch(token.Content)) && conditionalIndex == 0)
                {
                    achievement += "Ты сделал — " + token.ContentWithKeptCase + "\r\n";
                    conditionalIndex++;
                }
                else if ((token.Tag == "<NUM>" || amountByWords.Contains(token.Content)) && conditionalIndex == 1)
                {
                    achievement += "Сколько? — " + token.ContentWithKeptCase + "\r\n";
                    conditionalIndex++;

                }
                else if (token.Tag == "<S>" && conditionalIndex == 2)
                {
                    achievement += "Чего? — " + token.ContentWithKeptCase + "\r\n";
                    conditionalIndex++;
                }
                else if (conditionalIndex == 3)
                {
                    achievement = "";
                    conditionalIndex = 0;
                }
            }

            if (achievement == "")
                Console.WriteLine("Некорректный ввод либо не удалось распознать фразу.");
            else
                Console.WriteLine(achievement);

            // TODO idea - if achievement - reading a book then bot would ask what exactly the book is in order to clarify the achievement
        }
    }
}
