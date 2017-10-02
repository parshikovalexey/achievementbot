using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using SolarixLemmatizatorEngineNET;

namespace BOTFirst.Spellchecker
{

    public class SpellCheckerLT : ISpellChecker
    {
        #region Приватные поля
        // URL для проверки текста.
        private readonly string checkUrl;
        // URL для получения списка доступных языков.
        private readonly string getLanguagesUrl;
        private readonly string getSemanticSimularityUrl = "http://rusvectores.org/web/{0}__{1}/api/similarity/";
        // Дублирующее поле для хранения ошибки полученной при выполнении методов, любое исключение полученное в ходе выполнения метода окажется здесь.
        private Exception error;
        #endregion
        #region Приватные методы
        /// <summary>
        /// Метод предназначен для парсинга реультата проверки текста
        /// </summary>
        /// <param name="checkResult">Результат полученный от API</param>
        private List<Mistake> ParseCheckResult(string checkResult)
        {
            var mistakes = new List<Mistake>();
            try
            {
                var parsedJSON = JToken.Parse(checkResult);
                var matches = Utilities.JSON.GetObjectByKey(parsedJSON, "matches", ref error); // В LT ошибки лежат в объекте matches
                if (matches.HasValues)
                {
                    foreach (var match in matches)
                    {
                        var currentMistake = new MistakeLT(match);
                        if (!currentMistake.HasErrors())
                        {
                            mistakes.Add(currentMistake);
                        }
                        else
                        {
                            error = currentMistake.Error;
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
        /// <summary>
        /// Метод предназначен для парсинга реультата получения доступных языков
        /// </summary>
        /// <param name="languagesResult">Результат полученный от API</param>
        private Dictionary<string, string> ParseLanguages(string languagesResult)
        {
            var languages = new Dictionary<string, string>();
            try
            {
                var parsedJSON = JToken.Parse(languagesResult);
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
        private List<string> GetLemmasFromPtr(IntPtr pointer)
        {
            var countLemmas = LemmatizatorEngine.sol_CountLemmas(pointer);
            var stringBuilder = new StringBuilder();
            var result = new List<string>();
            for (int i = 0; i < countLemmas; i++)
            {
                LemmatizatorEngine.sol_GetLemmaStringW(pointer, i, stringBuilder, 30);
                result.Add(stringBuilder.ToString());
            }
            return result;
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
                    // Обращаемся к API методом POST и получаем результат.
                    var checkResult = wc.UploadString(checkUrl, "text=" + text + "&language=" + language +
                        "&enabledCategories=TYPOS&enabledOnly=true");
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
                    // Обращаемся к API методом GET и получаем результат.
                    var languagesResult = wc.DownloadString(getLanguagesUrl);
                    languages = ParseLanguages(languagesResult);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            return languages;
        }
        public string CorrectMistakes(string originalText, List<Mistake> mistakes)
        {
            mistakes.Reverse();
            var wordsWithoutMistakes = originalText;
            foreach (var mistake in mistakes)
            {
                wordsWithoutMistakes = wordsWithoutMistakes.Remove(mistake.Position.Begin, mistake.Position.Length + 1);
            }
            var lemmatizator = LemmatizatorEngine.sol_LoadLemmatizatorW(Environment.CurrentDirectory+"\\lemmatizer.db", LemmatizatorEngine.LEME_DEFAULT);
            var lemmasInTextPtr = LemmatizatorEngine.sol_LemmatizePhraseW(lemmatizator, wordsWithoutMistakes, 0, ' ');
            var lemmasInText = GetLemmasFromPtr(lemmasInTextPtr);
            var originalTextStringBuilder = new StringBuilder(originalText);
            foreach (var mistake in mistakes)
            {
                var lemmasInReplacesPtr = LemmatizatorEngine.sol_LemmatizePhraseW(
                    lemmatizator, 
                    mistake.Replacements.Count > 1 ? String.Join("*",mistake.Replacements) : mistake.Replacements[0],
                    0, '*');
                var lemmasInReplaces = GetLemmasFromPtr(lemmasInReplacesPtr);
                var maxSimularity = -1.0;
                var bestReplace = mistake.Replacements[0];
                foreach (var lemmaReplace in lemmasInReplaces)
                {
                    var simularitySum = 0.0;
                    using (WebClient wc = new WebClient())
                    {
                        foreach (var lemmaInText in lemmasInText)
                        {
                            var requestResult = wc.DownloadString(String.Format(getSemanticSimularityUrl, lemmaReplace,lemmaInText));
                            if (requestResult != "Unknown")
                            {
                                simularitySum += Double.Parse(requestResult.Split('\t')[0].Replace('.', ','));
                            }
                        }
                    }
                    if (simularitySum > maxSimularity)
                    {
                        maxSimularity = simularitySum;
                        bestReplace = mistake.Replacements[lemmasInReplaces.IndexOf(lemmaReplace)];
                    }
                }
                originalTextStringBuilder = originalTextStringBuilder.Replace(mistake.Original, bestReplace, mistake.Position.Begin, mistake.Position.Length);
            }
            return originalTextStringBuilder.ToString();
        }
        /// <summary>
        /// Метод проверяет текст на наличие ошибок. 
        /// Код выполняется асинхронно, поэтому текущий поток не останавливается на время выполнения этого метода.
        /// <para>Результаты проверки можно узнать после активации события <see cref="OnCheckComplete"/> с помощью свойства <see cref="Mistakes"/></para>
        /// </summary>
        /// <param name="language">Код языка для проверки(коды языков можно получить, выполнив методы:
        /// <see cref="GetAvailableLanguages"/> или <see cref="GetAvailableLanguagesAsync"/>)</param>
        /// <param name="checkedText">Проверяемый текст</param>
        public async Task<List<Mistake>> CheckTextAsync(string language, string text)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                var checkResult = wc.UploadStringTaskAsync(new Uri(checkUrl), "text=" + text + "&language=" + language +
                    "&enabledCategories=TYPOS&enabledOnly=true");
                return await Task.FromResult(ParseCheckResult(checkResult.Result));
            }
        }
        /// <summary>
        /// Метод для получения списка доступных для проверки языков вместе с их кодами.
        /// Метод выполняется асинхронно, поэтому текущий поток не остановится.
        /// </summary>
        public async Task<Dictionary<string, string>> GetAvailableLanguagesAsync()
        {
            using (WebClient wc = new WebClient())
            {
                var languagesResult = wc.DownloadStringTaskAsync(new Uri(getLanguagesUrl));
                return await Task.FromResult(ParseLanguages(languagesResult.Result));
            }

        }
        #endregion
    }

    public class MistakeLT : Mistake
    {
        private Exception error;
        public bool HasErrors()
        {
            return error != null;
        }

        /// <summary>
        /// Исключение которое может возникнуть при создании ошибки 
        /// </summary>
        public Exception Error { get { return error; } }
        /// <summary>
        /// Создает объект ошибки на основе переданного в конструктор JSON
        /// </summary>
        /// <param name="match">Описание ошибки в JSON</param>
        public MistakeLT(JToken match)
        {
            // Парсим JSON ошибки прямо при ее создании. 
            var context = Utilities.JSON.GetObjectByKey(match, "context", ref error);
            var replacements = Utilities.JSON.GetObjectByKey(match, "replacements", ref error);
            var offset = Utilities.JSON.GetValueByKey<int>(context, "offset", ref error);
            var length = Utilities.JSON.GetValueByKey<int>(context, "length", ref error);
            var text = Utilities.JSON.GetValueByKey<string>(context, "text", ref error);
            var short_message = Utilities.JSON.GetValueByKey<string>(match, "shortMessage", ref error);
            this.Position = new Position(offset, length);
            this.Replacements = replacements.Values<string>("value").ToList();
            this.Original = String.IsNullOrEmpty(text) ? "" : text.Substring(offset, length);
        }
    }

}

