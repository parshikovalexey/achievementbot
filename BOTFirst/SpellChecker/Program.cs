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
            //DemonstrateNonAsync();
            Console.ReadKey();
        }

        static void DemonstrateNonAsync() // метод для демонстрации синхронных методов
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

        static void DemonstrateAsync() // метод для демонстрации асинхронных методов
        {
            spellchkr = new SpellCheckerLT();
            spellchkr.GetAvailableLanguagesAsync(); // получаем список языков(вернее только посылаем запрос)
            spellchkr.OnAvailableLanguagesGetComplete += LanguagesDownloaded; //подписываемся на событие о получении списка языков
            for (int i = 0; i < 100; i++) // цикл для демонстрации асинхронности
            {
                System.Threading.Thread.Sleep(200); // итерация цикла раз в 200 миллисекунд
                Console.WriteLine(i);
            }
        }
        static void LanguagesDownloaded() // обработчик события загрузки доступных языков
        {
            if (spellchkr.Error == null)
            {
                foreach (var lang in spellchkr.Languages)
                {
                    Console.WriteLine("Название: " + lang.Key + " Код:" + lang.Value);
                }
                System.Threading.Thread.Sleep(1000); // ждем немного
                spellchkr.OnCheckComplete += MistakesDownloaded; // подписываемся на событие полученя результатов проверки 
                string text = "Сешь ещё этх мягких француских булок, да выпей же чаю."; // проверяемый текст
                spellchkr.CheckAsync(spellchkr.Languages["Russian"], text); // посылаем запрос на проверку текста
            }
            else
            {
                Console.WriteLine(spellchkr.Error.Message); // выведем сообщение об ошибке, если она возникла

            }
        }
        static void MistakesDownloaded() // обработчик события получения результатов проверки
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
