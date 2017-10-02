using System;
using BOTFirst.Spellchecker;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spell_checker
{
    class Program
    {
        static SpellCheckerLT spellchkr;
        static void Main(string[] args)
        {
            //DemonstrateAsync();
            //DemonstrateNonAsync();
            var text = "Пробижал пять килметров вчера";
            spellchkr = new SpellCheckerLT();
            var mistakes = spellchkr.CheckText("ru-RU",text);
            var result = spellchkr.CorrectMistakes(text, mistakes);
            Console.WriteLine(result);
            Console.ReadKey();
        }

        static void DemonstrateNonAsync() // метод для демонстрации синхронных методов
        {
            spellchkr = new SpellCheckerLT();
            string text = "Сешь ещё этх мягких француских булок, да выпей же чаю.";
            var languages = spellchkr.GetAvailableLanguages();
            var mistakes = spellchkr.CheckText(languages["Russian"], text);
            Console.WriteLine(text);
            Console.WriteLine("------------Ошибки---------------");
            foreach (var mistake in mistakes)
            {
                Console.WriteLine("Позиция: Начало - " + mistake.Position.Begin + ", Длина - " + mistake.Position.Length);
                Console.WriteLine("Слово: " + mistake.Original);
                Console.WriteLine("Воможные замены: ");
                foreach (var replace in mistake.Replacements)
                {
                    Console.WriteLine("                 " + replace);
                }
                Console.WriteLine("----------------------------");
            }
        }

        static void DemonstrateAsync() // метод для демонстрации асинхронных методов
        {
            spellchkr = new SpellCheckerLT();
            ExecLanguages();
            while (true)
                PrintLanguageCounter();
        }

        static private int languageCounter = 0;
        static private int disableLanguageCounterLimit = int.MaxValue;

        static void PrintLanguageCounter() {
            System.Threading.Thread.Sleep(1000);
            languageCounter++;
            if (languageCounter < disableLanguageCounterLimit)
                Console.WriteLine("Language " + languageCounter.ToString());
        }

        static void DemonstratePhraseAsync(string language, string text) // метод для демонстрации асинхронных методов
        {
            ExecMistakes(language, text);
            while (true)
                PrintMistakeCounter();
        }

        static private int mistakeCounter = 0;
        static private int disableMistakeCounterLimit = int.MaxValue;

        static void PrintMistakeCounter() {
            mistakeCounter++;
            if (mistakeCounter < disableMistakeCounterLimit)
                Console.WriteLine("Mistake " + mistakeCounter.ToString());
        }

        static async Task ExecLanguages() {
            await Task.Run(() => {
                var task = spellchkr.GetAvailableLanguagesAsync();
                LanguagesDownloaded(task.Result);
            });
        }

        static void LanguagesDownloaded(Dictionary<string, string> languages) // обработчик события загрузки доступных языков
        {
            disableLanguageCounterLimit = languageCounter + 10;
            if (spellchkr.Error == null) {
                foreach (var lang in languages) {
                    Console.WriteLine("Название: " + lang.Key + " Код:" + lang.Value);
                }
                string text = "Сешь ещё этх мягких француских булок, да выпей же чаю."; // проверяемый текст
                DemonstratePhraseAsync(languages["Russian"], text); // посылаем запрос на проверку текста
            } else {
                Console.WriteLine(spellchkr.Error.Message); // выведем сообщение об ошибке, если она возникла
            }
        }

        static async Task ExecMistakes(string language, string text) {
            await Task.Run(() => {
                var task = spellchkr.CheckTextAsync(language, text);
                MistakesDownloaded(task.Result);
            });
        }
        static void MistakesDownloaded(List<Mistake> mistakes) // обработчик события получения результатов проверки
        {
            disableMistakeCounterLimit = mistakeCounter + 10;
            if (spellchkr.Error == null) {
                Console.WriteLine("---------------ОШИБКИ--------------------");
                foreach (var mistake in mistakes) {
                    Console.WriteLine("Ошибка в слове " + mistake.Original);
                    Console.WriteLine("Номер начального символа: " + mistake.Position.Begin + " Длина: " + mistake.Position.Length);
                    foreach (var replace in mistake.Replacements) {
                        Console.WriteLine("                  " + replace);
                    }
                    Console.WriteLine("-----------------------------------");
                }
            } else {
                Console.WriteLine(spellchkr.Error.Message);
            }
        }

    }

}
