using System.Collections.Generic;


namespace BOTFirst.Spellchecker
{
    public delegate void GetAvailableLanguagesAsyncEventHandler(); // делегаты созданные для событий асинхронных методов
    public delegate void CheckAsyncContainerEventHandler();
    interface ISpellChecker
    {
        // синхронные методы
        List<Mistake> Check(string language, string checkedText); // метод проверки текста
        Dictionary<string, string> GetAvailableLanguages(); // метод для полуучения списка доступных языков
        string CorrectMistakes(); // метод исправляющий ошибки
        // асинхронные методы
        void CheckAsync(string language, string checkedText); // асинхронные версии методов
        void GetAvailableLanguagesAsync();

        //события для асинхронных методов
        event GetAvailableLanguagesAsyncEventHandler OnAvailableLanguagesGetComplete;
        event CheckAsyncContainerEventHandler OnCheckComplete;
    }
    

}
