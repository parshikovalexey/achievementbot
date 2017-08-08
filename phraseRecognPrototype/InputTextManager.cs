using System.Collections.Generic;

namespace WordClassTagger
{
    public static class InputTextManager
    {
        private static List<InputToken> InputTokens = null;

        public static List<InputToken> GetTokens()
        {
            return InputTokens;
        }

        public static void LoadInput(string inputText)
        {
            // In order to prevent multiple range adding
            if (InputTokens == null)
            {
                // Dictionary preparing
                TaggedWordsDictionary.Load();

                InputTokens = new List<InputToken>();
                InputTokens.AddRange(Tokenizer.GetTokensListFromInputText(inputText));

                // Tagging execution
                foreach (var token in InputTokens)
                {
                    // Execute tagging of the token
                    Tagger.DefineAndAppendTagToWord(token);
                }
            }
        }
    }
}
