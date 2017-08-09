using System.Collections.Generic;


namespace BOTFirst.Spellchecker
{
    public delegate void GetAvailableLanguagesAsyncEventHandler();
    public delegate void CheckAsyncContainerEventHandler();
    interface ISpellChecker
    {
        // синхронные методы
        List<IMistake> Check(string language, string checkedText);
        Dictionary<string, string> GetAvailableLanguages();
        // асинхронные методы
        void CheckAsync(string language, string checkedText);
        void GetAvailableLanguagesAsync();

        //события
        event GetAvailableLanguagesAsyncEventHandler OnAvailableLanguagesGetComplete;
        event CheckAsyncContainerEventHandler OnCheckComplete;
    }
    

}
