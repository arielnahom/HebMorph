using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using HebMorph.Core.Linguistics;
using HebMorph.Dictionary.FST;

namespace HebMorph.Dictionary.HSpell
{
    public class HSpellLoader
    {
        public static char Iso8859_To_Unicode(int c)
        {
            if (c >= 0xE0 && c <= 0xFA)
            {
                return (char)(c - 0xE0 + 0x05D0); // Alef
            }
            return (char)c;
        }

        public void LoadWgz(string directoryPath, FstDictionary fstDict)
        {
            string dictPath = Path.Combine(directoryPath, "hebrew.wgz");
            string prefixesPath = Path.Combine(directoryPath, "hebrew.wgz.prefixes");
            string descPath = Path.Combine(directoryPath, "hebrew.wgz.desc");
            string stemsPath = Path.Combine(directoryPath, "hebrew.wgz.stems");
            string sizesPath = Path.Combine(directoryPath, "hebrew.wgz.sizes");

            if (!File.Exists(dictPath))
            {
                Console.WriteLine($"Warning: WGZ files not found at {directoryPath}. Skipping HSpell binary load.");
                return;
            }

            try
            {
                using var dictStream = new FileStream(dictPath, FileMode.Open, FileAccess.Read);
                using var gzipDict = new GZipStream(dictStream, CompressionMode.Decompress);

                // Simplified mock loader that demonstrates the stream reading pattern for Hebrew characters
                // Full HSpell port requires matching the exact byte alignments of hspell-1.2
                char[] sbuf = new char[127]; // Max word length
                int slen = 0, c;

                while ((c = gzipDict.ReadByte()) > -1)
                {
                    if (c >= '0' && c <= '9')
                    {
                        // new word found
                        string word = new string(sbuf, 0, slen);
                        if (word.Length > 0)
                        {
                            var md = new MorphData();
                            md.AddLemma(new Lemma(word, DescFlag.D_NOUN, PrefixType.PS_EMPTY));
                            fstDict.AddEntry(word, md);
                        }

                        int n = 0;
                        do {
                            n *= 10;
                            n += (c - '0');
                        } while ((c = gzipDict.ReadByte()) > -1 && c >= '0' && c <= '9');
                        slen -= n;
                    }
                    if (c > -1) sbuf[slen++] = Iso8859_To_Unicode(c);
                }

                // Add the last word
                if (slen > 0)
                {
                    string word = new string(sbuf, 0, slen);
                    var md = new MorphData();
                    md.AddLemma(new Lemma(word, DescFlag.D_NOUN, PrefixType.PS_EMPTY));
                    fstDict.AddEntry(word, md);
                }

                Console.WriteLine($"Successfully streamed WGZ file from {dictPath}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load WGZ: {ex.Message}");
            }
        }
    }
}
