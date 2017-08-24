using System;
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

            
                TaggedWordsDictionary.Load();
                InputTokens = new List<InputToken>();
                InputTokens.Clear();
                InputTokens.AddRange(Tokenizer.GetTokensListFromInputText(inputText));
                
                // Tagging execution.
                foreach (var token in InputTokens)
                {
                    // Execute tagging of the token.
                    Tagger.DefineAndAppendTagToWord(token);
                }
                
            
        }
    }
}
