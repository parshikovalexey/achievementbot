using System.Collections.Generic;

namespace WordClassTagger
{
    public static class InputTextManager
    {
        private static List<InputToken> InputTokens = new List<InputToken>();

        public static List<InputToken> GetTokens()
        {
            return InputTokens;
        }

        public static void LoadInput(string inputText)
        {
            InputTokens.AddRange(Tokenizer.GetTokensListFromInputText(inputText));
        }
    }
}
