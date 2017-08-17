using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;

namespace BOTFirst.Spellchecker
{

    public class SpellCheckerLT : ISpellChecker
    {
        #region Приватные поля
        private List<Mistake> mistakes; // дублирующее поле содержащее список ошибок в проверенном тексте
        private Dictionary<string, string> languages; // словарь доступных для проверки языков где ключ-название языка(на английском), значение - код языка
        private string checkUrl; // URL для проверки текста
        private string getLanguagesUrl; // URL для получения списка доступных языков
        private string checkResult; // строка(JSON) полученная от LanguageTool API при проверке текста
        private string languagesresult; // строка(JSON) полученная от LanguageTool API при получении списка доступных языков
        private Exception error; // дублирующее поле для хранения ошибки полученной при выполнении методов, любое исключение полученное в ходе выполнения метода окажется здесь
        #endregion
        #region Приватные методы
        private void GetAvailableLanguagesHandler(object sender, DownloadStringCompletedEventArgs e) // данный метод вызывается при срабатывании события асинхронной загрузки строки(DownloadStringCompleted)
        {
            if (e.Error == null) 
            {
                languagesresult = e.Result;
                ParseLanguages(); 
            }
            else
            {
                error = e.Error; // при возникновении ошибки помещаем ее в специально выделенное поле
            }
            OnAvailableLanguagesGetComplete(); // активируем событие завершения асинхронной загрузки языков
        }

        private void CheckHandler(object sender, UploadStringCompletedEventArgs e) // данный метод вызывается при срабатывании события асинхронной загрузки строки(UploadStringCompleted) 
        {
            if (e.Error == null)
            {
                checkResult = e.Result;
                ParseCheck();
            }
            else
            {
                error = e.Error;
            }
            OnCheckComplete(); // активируем событие завершения асинхронной проверки текста

        }

        private void ParseCheck() // метод предназначен для парсинга реультата проверки текста
        {
            try
            {
                JToken parsedJSON = JToken.Parse(checkResult);
                JToken matches = Utilities.JSON.GetObjectByKey(parsedJSON, "matches", ref error); // В LT ошибки лежат в объекте matches
                mistakes = new List<Mistake>();
                if (matches.HasValues)
                {
                    foreach (var match in matches)
                    {
                        MistakeLT curMistake = new MistakeLT(match);
                        if (curMistake.Error == null)
                        {
                            mistakes.Add(curMistake);
                        }
                        else
                        {
                            error = curMistake.Error;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }

        private void ParseLanguages() // метод предназначен для парсинга реультата получения доступных языков
        {
            languages = new Dictionary<string, string>();
            try
            {
                JToken parsedJSON = JToken.Parse(languagesresult);
                foreach (var lang in parsedJSON)
                {
                    languages.Add(Utilities.JSON.GetValueByKey<string>(lang, "name", ref error),
                        Utilities.JSON.GetValueByKey<string>(lang, "longCode", ref error));
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }
        #endregion
        #region Свойства
        /// <summary> 
        /// Свойство возвращает любое исключение выброшенное при выполнении методов:
        /// <para><see cref="GetAvailableLanguages"/></para>  
        /// <para><see cref="GetAvailableLanguagesAsync"/> </para>
        /// <para><see cref="Check(string, string)"/> </para>
        /// <para><see cref="CheckAsync(string, string)"/> </para>
        /// <para><see cref="CorrectMistakes"/> </para>
        /// </summary>
        public Exception Error { get { return error; } }
        /// <summary>
        /// Возвращает ошибки в тексте после выполнения метода <see cref="Check(string, string)"/> 
        /// или <see cref="CheckAsync(string, string)"/>
        /// </summary>
        public List<Mistake> Mistakes { get { return mistakes; } }
        /// <summary>
        /// Языки доступные для проверки с помощью LT доступен после выполнения метода <see cref="GetAvailableLanguages"/> 
        /// или <see cref="GetAvailableLanguagesAsync"/>
        /// </summary>
        public Dictionary<string, string> Languages { get { return languages; } }
        #endregion
        #region Конструкторы
        public SpellCheckerLT()
        {
            checkUrl = "https://languagetool.org/api/v2/check";
            getLanguagesUrl = "https://languagetool.org/api/v2/languages";

        }
        #endregion
        #region Публичные методы
        /// <summary>
        /// Метод проверяет текст на наличие ошибок. 
        /// Код выполняется синхронно, поэтому текущий поток остановится на время выполнения этого метода.
        /// </summary>
        /// <param name="language">Код языка для проверки(коды языков можно получить, выполнив методы:
        /// <see cref="GetAvailableLanguages"/> или <see cref="GetAvailableLanguagesAsync"/>)</param>
        /// <param name="checkedText">Проверяемый текст</param>
        public List<Mistake> Check(string language, string checkedText)
        {
            try
            {
                using (WebClient wc = new WebClient()) 
                {
                    wc.Encoding = Encoding.UTF8;
                    checkResult = wc.UploadString(checkUrl, "text=" + checkedText + "&language=" + language); // обращаемся к API методом POST и получаем результат
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            ParseCheck(); // парсим результат
            return mistakes;
        }
        /// <summary>
        /// Метод для получения списка доступных для проверки языков вместе с их кодами.
        /// Метод выполняется синхронно, поэтому текущий поток остановится на время выполнения этого метода.
        /// </summary>
        public Dictionary<string, string> GetAvailableLanguages()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    languagesresult = wc.DownloadString(getLanguagesUrl); // обращаемся к API методом GET и получаем результат
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            ParseLanguages(); // парсим результат запроса
            return languages;
        }
        public string CorrectMistakes()
        {
            return ""; // не реализован
        }
        /// <summary>
        /// Метод проверяет текст на наличие ошибок. 
        /// Код выполняется асинхронно, поэтому текущий поток не останавливается на время выполнения этого метода.
        /// <para>Результаты проверки можно узнать после активации события <see cref="OnCheckComplete"/> с помощью свойства <see cref="Mistakes"/></para>
        /// </summary>
        /// <param name="language">Код языка для проверки(коды языков можно получить, выполнив методы:
        /// <see cref="GetAvailableLanguages"/> или <see cref="GetAvailableLanguagesAsync"/>)</param>
        /// <param name="checkedText">Проверяемый текст</param>
        public void CheckAsync(string language, string checkedText)
        {
            using (WebClient wc = new WebClient()) 
            {
                wc.Encoding = Encoding.UTF8;
                wc.UploadStringCompleted += CheckHandler; // добавляем обработчик события выполнения http запроса(подписываемся на событие)
                wc.UploadStringAsync(new Uri(checkUrl), "text=" + checkedText + "&language=" + language); // обращаемся к API методом POST
            }
        }
        /// <summary>
        /// Метод для получения списка доступных для проверки языков вместе с их кодами.
        /// Метод выполняется асинхронно, поэтому текущий поток не остановится.
        /// </summary>
        public void GetAvailableLanguagesAsync()
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadStringCompleted += GetAvailableLanguagesHandler; // подписывемся на событие
                wc.DownloadStringAsync(new Uri(getLanguagesUrl)); // обращаемся к API методом GET
            }

        }
        #endregion
        #region События
        /// <summary>
        /// Событие получения списка доступных языков
        /// </summary>
        public event GetAvailableLanguagesAsyncEventHandler OnAvailableLanguagesGetComplete;
        /// <summary>
        /// Событие получения результатов проверки
        /// </summary>
        public event CheckAsyncContainerEventHandler OnCheckComplete;
        #endregion
    }
    
    public class MistakeLT : Mistake
    {
        private Exception error;
        /// <summary>
        /// Исключение которое может возникнуть при создании ошибки 
        /// </summary>
        public Exception Error { get { return error; } }
        /// <summary>
        /// Создает объект ошибки на основе переданного в конструктор JSON
        /// </summary>
        /// <param name="match"></param>
        public MistakeLT(JToken match) // парсим JSON ошибки прямо при ее создании 
        {
            JToken context = Utilities.JSON.GetObjectByKey(match, "context", ref error);
            JToken replacements = Utilities.JSON.GetObjectByKey(match, "replacements",ref error);
            int offset = Utilities.JSON.GetValueByKey<int>(context,"offset",ref error);
            int length = Utilities.JSON.GetValueByKey<int>(context, "length", ref error); 
            string text = Utilities.JSON.GetValueByKey<string>(context, "text", ref error);
            string short_message = Utilities.JSON.GetValueByKey<string>(match, "shortMessage", ref error);
            Position = new Position(offset, length);
            Replacements = replacements.Values<string>("value").ToList();
            Original = !String.IsNullOrEmpty(text) ? text.Substring(offset, length) : "";
        }
    }
    
}

