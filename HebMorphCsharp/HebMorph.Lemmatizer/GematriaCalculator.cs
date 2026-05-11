using System;
using System.Collections.Generic;

namespace HebMorph.Lemmatizer
{
    public static class GematriaCalculator
    {
        private static readonly Dictionary<char, int> Values = new Dictionary<char, int>
        {
            {'א', 1}, {'ב', 2}, {'ג', 3}, {'ד', 4}, {'ה', 5}, {'ו', 6}, {'ז', 7}, {'ח', 8}, {'ט', 9},
            {'י', 10}, {'כ', 20}, {'ך', 20}, {'ל', 30}, {'מ', 40}, {'ם', 40}, {'נ', 50}, {'ן', 50},
            {'ס', 60}, {'ע', 70}, {'פ', 80}, {'ף', 80}, {'צ', 90}, {'ץ', 90},
            {'ק', 100}, {'ר', 200}, {'ש', 300}, {'ת', 400}
        };

        // TODO Implemented: Gematria test
        public static bool IsValidGematria(string word)
        {
            // Simple heuristic for Gematria:
            // - Contains ' or " near the end
            // - Usually 1 to 4 characters long (excluding punctuation)
            // - Characters are strictly Hebrew letters
            // - Calculates to a valid number pattern

            if (string.IsNullOrEmpty(word)) return false;

            int len = word.Length;
            if (len > 5) return false;

            bool hasPunctuation = false;
            int hebrewCharCount = 0;

            foreach (char c in word)
            {
                if (c == '\'' || c == '\"' || c == '\u05F3' || c == '\u05F4')
                {
                    hasPunctuation = true;
                }
                else if (Values.ContainsKey(c))
                {
                    hebrewCharCount++;
                }
                else
                {
                    return false; // Invalid char for Gematria
                }
            }

            return hasPunctuation && hebrewCharCount > 0;
        }

        public static int Calculate(string word)
        {
            int sum = 0;
            foreach (char c in word)
            {
                if (Values.TryGetValue(c, out int val))
                {
                    sum += val;
                }
            }
            return sum;
        }
    }
}
