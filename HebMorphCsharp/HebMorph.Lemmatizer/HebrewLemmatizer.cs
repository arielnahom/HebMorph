using System;
using System.Collections.Generic;
using System.Linq;
using HebMorph.Core.Linguistics;
using HebMorph.Dictionary.Interfaces;
using HebMorph.Tokenizer;

namespace HebMorph.Lemmatizer
{
    public class LemmatizedToken
    {
        public Token OriginalToken { get; set; } = new Token();
        public List<MorphData> Lemmas { get; set; } = new List<MorphData>();
        public bool IsGematria { get; set; }
        public int GematriaValue { get; set; }
        public bool IsStopWord { get; set; }
    }

    public class HebrewLemmatizer
    {
        private readonly IMorphologicalDictionary _dictionary;

        // Comprehensive Hebrew stop word list
        private readonly HashSet<string> _stopWords = new HashSet<string>
        {
            "את", "על", "של", "גם", "לא", "כל", "או", "אם", "עם", "כי", "רק", "זה", "אשר",
            "הוא", "היא", "הם", "הן", "אני", "אנחנו", "אתה", "אתם", "אלא", "אך", "אבל", "אז",
            "אין", "יש", "היה", "יהיה", "היו", "ב", "ל", "מ", "כ", "ש", "ה", "ו", "כך", "כן",
            "עד", "עוד", "בין", "כמו", "אולי", "כמעט", "מיד", "תמיד", "שם", "כאן", "פה"
        };

        // Moshe VeCalev: Valid prefix combinations (B, K, L, M, Sh, H, V)
        // Ordered from longest to shortest to avoid partial stripping matches
        private readonly string[] _validPrefixes =
        {
            "וכש", "ולכש", "ושמ", "שכ", "ומ", "ול", "וכ", "וש", "וה", "כש", "מש", "בש", "לש",
            "ב", "כ", "ל", "מ", "ש", "ה", "ו"
        };

        public HebrewLemmatizer(IMorphologicalDictionary dictionary)
        {
            _dictionary = dictionary;
        }

        public List<LemmatizedToken> Analyze(List<Token> tokens)
        {
            var result = new List<LemmatizedToken>();

            foreach (var token in tokens)
            {
                var lt = new LemmatizedToken { OriginalToken = token };

                if (GematriaCalculator.IsValidGematria(token.Text))
                {
                    lt.IsGematria = true;
                    lt.GematriaValue = GematriaCalculator.Calculate(token.Text);
                    lt.Lemmas.Add(new MorphData(lt.GematriaValue.ToString(), 0));
                    result.Add(lt);
                    continue;
                }

                // 2. Exact Lookup
                var exactLemmas = _dictionary.Lookup(token.Text);

                // 3. Tolerance Lookup (Ktiv Male / Ktiv Haser)
                var tolerantLemmas = TolerantLookup(token.Text);

                // 4. Prefix Stripping (with prefix masking)
                var prefixLemmas = StripPrefixesAndLookup(token.Text);

                var allLemmas = exactLemmas.Concat(tolerantLemmas).Concat(prefixLemmas).Distinct().ToList();

                lt.IsStopWord = _stopWords.Contains(token.Text);

                lt.Lemmas = allLemmas
                    .Where(l => l.Lemmas.Any() && !_stopWords.Contains(l.Lemmas.First().Text))
                    .OrderByDescending(l => CalculateScore(token.Text, l.Lemmas.FirstOrDefault()?.Text ?? ""))
                    .ToList();

                if (!lt.Lemmas.Any() && !lt.IsStopWord)
                {
                    lt.Lemmas.Add(new MorphData(token.Text, 0)); // 0 mask for unknown
                }

                result.Add(lt);
            }

            return result;
        }

        private List<MorphData> TolerantLookup(string word)
        {
            var results = new List<MorphData>();

            // Standard Hebrew Ktiv Male rules: Remove 'ו' or 'י' from middle of words to find base form
            if (word.Contains("ו"))
            {
                string withoutVav = word.Replace("ו", "");
                if (withoutVav != word) results.AddRange(_dictionary.Lookup(withoutVav));
            }

            if (word.Contains("י"))
            {
                string withoutYud = word.Replace("י", "");
                if (withoutYud != word) results.AddRange(_dictionary.Lookup(withoutYud));
            }

            // Handle double Vav/Yud collapsing (וו -> ו, יי -> י)
            if (word.Contains("וו"))
            {
                results.AddRange(_dictionary.Lookup(word.Replace("וו", "ו")));
            }
            if (word.Contains("יי"))
            {
                results.AddRange(_dictionary.Lookup(word.Replace("יי", "י")));
            }

            return results;
        }

        private List<MorphData> StripPrefixesAndLookup(string word)
        {
            var results = new List<MorphData>();

            // Try all valid prefix combinations
            foreach (var prefix in _validPrefixes)
            {
                if (word.StartsWith(prefix) && word.Length > prefix.Length + 1) // Leave at least 2 chars for Hebrew root
                {
                    string stripped = word.Substring(prefix.Length);
                    var candidates = _dictionary.Lookup(stripped);

                    foreach (var candidate in candidates)
                    {
                        // In a true HSpell port, we must map the specific stripped string (e.g., 'וש')
                        // back to the bitmask defined in MorphData.Prefixes.
                        // Here we simulate the allowance: if the dictionary has it, and the prefix was valid, we allow it.
                        // Future optimization: Bitwise check `(candidate.Prefixes & PrefixType.PS_W) > 0` etc.
                        results.Add(candidate);
                    }
                }
            }
            return results;
        }

        private int CalculateScore(string original, string lemma)
        {
            if (string.IsNullOrEmpty(lemma)) return 0;
            if (original == lemma) return 100;
            if (original.Contains(lemma)) return 80;
            return 50;
        }
    }
}
