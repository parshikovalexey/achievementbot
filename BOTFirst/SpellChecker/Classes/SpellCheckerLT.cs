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
        private List<IMistake> _mistakes;
        private Dictionary<string, string> _languages;
        private string _checkUrl;
        private string _getLanguagesUrl;
        private string _checkResult;
        private string _languagesresult;

        private void GetAvailableLanguagesHandler(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                _languages = new Dictionary<string, string>();
                JToken parsedJSON = JToken.Parse(e.Result);
                foreach (var lang in parsedJSON)
                {
                    _languages.Add(lang["name"].Value<string>(), lang["longCode"].Value<string>());
                }

                OnAvailableLanguagesGetComplete();
            }
            else
            {
                Error = e.Error;
                OnAvailableLanguagesGetComplete();
            }
        }
        private void CheckHandler(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                JToken parsedJSON = JToken.Parse(e.Result);
                JToken matches = parsedJSON["matches"];
                _mistakes = new List<IMistake>();
                if (matches.HasValues)
                {
                    foreach (var match in matches)
                    {
                        MistakeLT curMistake = new MistakeLT(match);
                        _mistakes.Add(curMistake);
                    }
                }
                OnCheckComplete();
            }
            else
            {
                Error = e.Error;
                OnCheckComplete();
            }
            
        }


        public Exception Error;
        public List<IMistake> Mistakes { get { return _mistakes; } }
        public Dictionary<string, string> Languages { get { return _languages; } }

        public SpellCheckerLT()
        {
            _checkUrl = "https://languagetool.org/api/v2/check";
            _getLanguagesUrl = "https://languagetool.org/api/v2/languages";
        }
        public List<IMistake> Check(string language, string checkedText)
        {
            try
            {
                using (WebClient wc = new WebClient()) // обращаемся к API методом POST
                {
                    wc.Encoding = Encoding.UTF8;
                    _checkResult = wc.UploadString(_checkUrl, "text=" + checkedText + "&language=" + language);
                }
            }
            catch (Exception ex)
            {
                Error = ex;
            }

            JToken parsedJSON = JToken.Parse(_checkResult);
            JToken matches = parsedJSON["matches"];
            _mistakes = new List<IMistake>();
            if (matches.HasValues)
            {
                foreach (var match in matches)
                {
                    MistakeLT curMistake = new MistakeLT(match);
                    _mistakes.Add(curMistake);
                }
            }
            return _mistakes;
        }

        public Dictionary<string, string> GetAvailableLanguages()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    _languagesresult = wc.DownloadString(_getLanguagesUrl);
                }
            }
            catch (Exception ex)
            {
                Error = ex;
            }

            JToken parsedJSON = JToken.Parse(_languagesresult);
            foreach (var lang in parsedJSON)
            {
                _languages.Add(lang["name"].Value<string>(), lang["longCode"].Value<string>());
            }
            return _languages;
        }

        public async void CheckAsync(string language, string checkedText)
        {
            using (WebClient wc = new WebClient()) // обращаемся к API методом POST
            {
                wc.Encoding = Encoding.UTF8;
                wc.UploadStringCompleted += CheckHandler;
                wc.UploadStringAsync(new Uri(_checkUrl), "text=" + checkedText + "&language=" + language);
            }
        }
        public void GetAvailableLanguagesAsync()
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadStringCompleted += GetAvailableLanguagesHandler;
                wc.DownloadStringAsync(new Uri(_getLanguagesUrl));
            }

        }

        public event GetAvailableLanguagesAsyncEventHandler OnAvailableLanguagesGetComplete;
        public event CheckAsyncContainerEventHandler OnCheckComplete;
    }

    struct MistakeLT : IMistake
    {
        public string Original { get; set; }
        public Position Position { get; set; }
        public List<string> Replacements { get; set; }
        public string Type { get; set; }
        public MistakeLT(JToken match)
        {
            JToken context = match["context"];
            JToken replacements = match["replacements"];
            int offset = context["offset"].Value<int>();
            int length = context["length"].Value<int>();
            string text = context["text"].Value<string>();
            string short_message = match["shortMessage"].Value<string>();
            Original = text.Substring(offset, length);
            Position = new Position(offset, length);
            Replacements = replacements.Values<string>("value").ToList();
            Type = short_message;


        }
    }

}

