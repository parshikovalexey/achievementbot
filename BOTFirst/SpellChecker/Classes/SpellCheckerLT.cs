using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace BOTFirst.Spellchecker
{

    public class SpellCheckerLT : ISpellChecker
    {
        #region Приватные поля
        private string checkUrl; // URL для проверки текста
        private string getLanguagesUrl; // URL для получения списка доступных языков
        private Exception error; // дублирующее поле для хранения ошибки полученной при выполнении методов, любое исключение полученное в ходе выполнения метода окажется здесь
        #endregion
        #region Приватные методы
        private List<Mistake> ParseCheckResult(string checkResult) // метод предназначен для парсинга реультата проверки текста
        {
            var mistakes = new List<Mistake>();
            try
            {
                JToken parsedJSON = JToken.Parse(checkResult);
                JToken matches = Utilities.JSON.GetObjectByKey(parsedJSON, "matches", ref error); // В LT ошибки лежат в объекте matches
                if (matches.HasValues)
                {
                    foreach (var match in matches)
                    {
                        MistakeLT curMistake = new MistakeLT(match);
                        if (!curMistake.HasErrors())
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
            return mistakes;
        }

        private Dictionary<string, string> ParseLanguages(string languagesResult) // метод предназначен для парсинга реультата получения доступных языков
        {
            var languages = new Dictionary<string, string>();
            try
            {
                JToken parsedJSON = JToken.Parse(languagesResult);
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
            return languages;
        }
        #endregion
        #region Свойства
        /// <summary> 
        /// Свойство возвращает любое исключение выброшенное при выполнении методов:
        /// <para><see cref="GetAvailableLanguages"/></para>  
        /// <para><see cref="GetAvailableLanguagesAsync"/> </para>
        /// <para><see cref="CheckText(string, string)"/> </para>
        /// <para><see cref="CheckAsync(string, string)"/> </para>
        /// <para><see cref="CorrectMistakes"/> </para>
        /// </summary>
        public Exception Error { get { return error; } }
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
        /// <param name="text">Проверяемый текст</param>
        public List<Mistake> CheckText(string language, string text)
        {
            var mistakes = new List<Mistake>();
            try
            {
                using (WebClient wc = new WebClient()) 
                {
                    wc.Encoding = Encoding.UTF8;
                    // обращаемся к API методом POST и получаем результат
                    var checkResult = wc.UploadString(checkUrl, "text=" + text + "&language=" + language);
                    mistakes = ParseCheckResult(checkResult);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            return mistakes;
        }
        /// <summary>
        /// Метод для получения списка доступных для проверки языков вместе с их кодами.
        /// Метод выполняется синхронно, поэтому текущий поток остановится на время выполнения этого метода.
        /// </summary>
        public Dictionary<string, string> GetAvailableLanguages()
        {
            var languages = new Dictionary<string, string>();
            try
            {
                using (WebClient wc = new WebClient())
                {
                    var languagesResult = wc.DownloadString(getLanguagesUrl); // обращаемся к API методом GET и получаем результат
                    languages = ParseLanguages(languagesResult);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            return languages;
        }
        public string CorrectMistakes(string originalText, List<Mistake> mistakes) {
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
        public async Task<List<Mistake>> CheckTextAsync(string language, string text) {
            using (WebClient wc = new WebClient()) {
                wc.Encoding = Encoding.UTF8;
                var checkResult = wc.UploadStringTaskAsync(new Uri(checkUrl), "text=" + text + "&language=" + language);
                return await Task.FromResult(ParseCheckResult(checkResult.Result));
            }
        }
        /// <summary>
        /// Метод для получения списка доступных для проверки языков вместе с их кодами.
        /// Метод выполняется асинхронно, поэтому текущий поток не остановится.
        /// </summary>
        public async Task<Dictionary<string, string>> GetAvailableLanguagesAsync() {
            using (WebClient wc = new WebClient()) {
                var languagesResult = wc.DownloadStringTaskAsync(new Uri(getLanguagesUrl));
                return await Task.FromResult(ParseLanguages(languagesResult.Result));
            }

        }
        #endregion
    }
    
    public class MistakeLT : Mistake
    {
        private Exception error;
        public bool HasErrors() {
            return error != null;
        }

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

