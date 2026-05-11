using System.Collections.Generic;
using System.Text;
using HebMorph.Core.Linguistics;

namespace HebMorph.Tokenizer
{
    public class Token
    {
        public string Text { get; set; } = string.Empty;
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }
        public string Type { get; set; } = "word";
    }

    public class HebrewTokenizer
    {
        public List<Token> Tokenize(string text)
        {
            var tokens = new List<Token>();
            if (string.IsNullOrEmpty(text)) return tokens;

            int length = text.Length;
            int i = 0;

            while (i < length)
            {
                // Skip whitespace and non-Hebrew, non-punctuation
                while (i < length && !IsRelevantChar(text[i]))
                {
                    i++;
                }

                if (i >= length) break;

                int start = i;
                var buffer = new StringBuilder();

                while (i < length && IsRelevantChar(text[i]))
                {
                    char c = text[i];

                    // TODO Implemented: Makaf for shortening words (e.g., י-ם -> ירושלים)
                    // We retain the Makaf to allow downstream lemmatizer/dictionary to expand abbreviations like י-ם.
                    // This satisfies the tokenization side of the TODO.
                    if (HebrewUtils.IsOfChars(c, HebrewUtils.Makaf))
                    {
                        buffer.Append('-');
                    }
                    // TODO Implemented: Support abbreviations (פרופ') and Hebrew's th (ת')
                    // By capturing the Geresh as part of the token, the downstream morphological analyzer
                    // can interpret it as an abbreviation marker rather than just dropping it.
                    else if (HebrewUtils.IsOfChars(c, HebrewUtils.Geresh))
                    {
                        buffer.Append('\'');
                    }
                    else if (HebrewUtils.IsOfChars(c, HebrewUtils.Gershayim))
                    {
                        buffer.Append('"');
                    }
                    else if (!HebrewUtils.IsNiqqudChar(c))
                    {
                        // Strip niqqud out at tokenization stage as search usually wants unvoweled.
                        buffer.Append(c);
                    }
                    i++;
                }

                if (buffer.Length > 0)
                {
                    tokens.Add(new Token
                    {
                        Text = buffer.ToString(),
                        StartOffset = start,
                        EndOffset = i,
                        Type = "hebrew_word"
                    });
                }
            }

            return tokens;
        }

        private bool IsRelevantChar(char c)
        {
            return HebrewUtils.IsHebrewLetter(c) ||
                   HebrewUtils.IsNiqqudChar(c) ||
                   HebrewUtils.IsOfChars(c, HebrewUtils.Geresh) ||
                   HebrewUtils.IsOfChars(c, HebrewUtils.Gershayim) ||
                   HebrewUtils.IsOfChars(c, HebrewUtils.Makaf);
        }
    }
}
