using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOTFirst.Spellchecker
{
    interface ISpellChecker
    {
        #region Синхронные методы
        // Метод проверки текста.
        List<Mistake> CheckText(string language, string text);
        // Метод для полуучения списка доступных языков.
        Dictionary<string, string> GetAvailableLanguages();
        // Метод исправляющий ошибки.
        string CorrectMistakes(string originalText, List<Mistake> mistakes); 
        #endregion
        #region Асинхронные методы
        Task<Dictionary<string, string>> GetAvailableLanguagesAsync();
        Task<List<Mistake>> CheckTextAsync(string language, string text);
        #endregion
    }
    

}
