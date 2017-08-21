using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOTFirst.Spellchecker
{
    interface ISpellChecker
    {
        // синхронные методы
        List<Mistake> CheckText(string language, string text); // метод проверки текста
        Dictionary<string, string> GetAvailableLanguages(); // метод для полуучения списка доступных языков
        string CorrectMistakes(string originalText, List<Mistake> mistakes); // метод исправляющий ошибки
        // асинхронные методы
        Task<Dictionary<string, string>> GetAvailableLanguagesAsync();
        Task<List<Mistake>> CheckTextAsync(string language, string text);

    }
    

}
