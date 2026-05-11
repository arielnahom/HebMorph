using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using HebMorph.Core.Linguistics;
using HebMorph.Dictionary.FST;

namespace HebMorph.Dictionary
{
    public class CustomDictionaryRecord
    {
        public string Form { get; set; } = string.Empty;
        public string Lemma { get; set; } = string.Empty;
        public byte DescFlagValue { get; set; }
        public byte PrefixTypeValue { get; set; }
    }

    public class CsvLoader
    {
        public void LoadCustomDictionary(string csvFilePath, FstDictionary fstDict)
        {
            if (!File.Exists(csvFilePath))
            {
                Console.WriteLine($"CSV file not found: {csvFilePath}. Skipping custom load.");
                return;
            }

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                };

                using var reader = new StreamReader(csvFilePath);
                using var csv = new CsvReader(reader, config);

                var records = csv.GetRecords<CustomDictionaryRecord>();
                foreach (var record in records)
                {
                    var morphData = new MorphData();
                    var lemma = new Lemma(
                        record.Lemma,
                        (DescFlag)record.DescFlagValue,
                        (PrefixType)record.PrefixTypeValue
                    );

                    morphData.AddLemma(lemma);
                    fstDict.AddEntry(record.Form, morphData);
                }
                Console.WriteLine($"Successfully loaded custom dictionary from {csvFilePath}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading CSV dictionary: {ex.Message}");
            }
        }
    }
}
