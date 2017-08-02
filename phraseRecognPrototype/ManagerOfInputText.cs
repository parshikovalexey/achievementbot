using System.Collections.Generic;

namespace WordClassTagger
{
    public static class ManagerOfInputText
    {
        private static List<InputToken> LInputTokens = new List<InputToken>();

        public static List<InputToken> GetTokens()
        {
            return LInputTokens;
        }

        public static void LoadInput(string inputText)
        {
            LInputTokens.AddRange(Tokenizer.GetTokensListFromInputText(inputText));
        }
    }
}
