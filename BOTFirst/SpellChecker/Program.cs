using System;
using BOTFirst.Spellchecker;
using System.Net;

namespace Spell_checker
{
    class Program
    {
        static SpellCheckerLT spellchkr;
        static void Main(string[] args)
        {
            DemonstrateAsync();
            Console.ReadKey();
        }

        static void DemonstrateNonAsync()
        {
            spellchkr = new SpellCheckerLT();
            string text = "Сешь ещё этх мягких француских булок, да выпей же чаю.";
            var languages = spellchkr.GetAvailableLanguages();
            var mistakes = spellchkr.Check(languages["Russian"], text);
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

        static void DemonstrateAsync()
        {
            spellchkr = new SpellCheckerLT();
            spellchkr.GetAvailableLanguagesAsync();
            spellchkr.OnAvailableLanguagesGetComplete += LanguagesDownloaded;
            for (int i = 0; i < 100; i++) // цикл для демонстрации асинхронности
            {
                System.Threading.Thread.Sleep(200);
                Console.WriteLine(i);
            }
        }
        static void LanguagesDownloaded()
        {
            if (spellchkr.Error == null)
            {
                foreach (var lang in spellchkr.Languages)
                {
                    Console.WriteLine("Название: " + lang.Key + " Код:" + lang.Value);
                }
                System.Threading.Thread.Sleep(1000);
                spellchkr.OnCheckComplete += MistakesDownloaded;
                string text = "Сешь ещё этх мягких француских булок, да выпей же чаю.";
                spellchkr.CheckAsync(spellchkr.Languages["Russian"], text);
            }
            else
            {
                Console.WriteLine(spellchkr.Error.Message);
            }
        }
        static void MistakesDownloaded()
        {
            if (spellchkr.Error == null)
            {
                Console.WriteLine("---------------ОШИБКИ--------------------");
                foreach (var mistake in spellchkr.Mistakes)
                {
                    Console.WriteLine("Ошибка в слове " + mistake.Original);
                    Console.WriteLine("Номер начального символа: " + mistake.Position.Begin + " Длина: " + mistake.Position.Length);
                    foreach (var replace in mistake.Replacements)
                    {
                        Console.WriteLine("                  " + replace);
                    }
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine(spellchkr.Error.Message);
            }
        }

    }

}
