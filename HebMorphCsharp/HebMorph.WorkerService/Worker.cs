using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HebMorph.Dictionary.Interfaces;
using HebMorph.Dictionary.FST;
using HebMorph.Dictionary.HSpell;
using HebMorph.Dictionary;
using HebMorph.Tokenizer;
using HebMorph.Lemmatizer;
using HebMorph.Core.Linguistics;

namespace HebMorph.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMorphologicalDictionary _dictionary;
        private readonly HebrewTokenizer _tokenizer;
        private readonly HebrewLemmatizer _lemmatizer;

        public Worker(ILogger<Worker> logger, IMorphologicalDictionary dictionary, HebrewTokenizer tokenizer, HebrewLemmatizer lemmatizer)
        {
            _logger = logger;
            _dictionary = dictionary;
            _tokenizer = tokenizer;
            _lemmatizer = lemmatizer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("HebMorph Standalone Worker Service starting...");

            // 1. Build Dictionary using extracted Gold data (CSV)
            _logger.LogInformation("Building Morphological Dictionary from Gold Data...");

            if (_dictionary is FstDictionary fstDict)
            {
                // Load Gold CSV generated from the original Java engine's WGZ parser
                var csvLoader = new CsvLoader();
                string goldCsvPath = Path.Combine(AppContext.BaseDirectory, "gold_dictionary.csv");

                if (!File.Exists(goldCsvPath))
                {
                    // Fallback to sample logic to not crash if gold isn't in output dir
                    _logger.LogWarning("Gold dictionary not found in output path. Creating sample.");
                    File.WriteAllText(goldCsvPath, "Form,Lemma,DescFlagValue,PrefixTypeValue\nכלבים,כלב,1,0\nמחשב,מחשב,1,0\n");
                }

                _logger.LogInformation("Loading Gold CSV...");
                csvLoader.LoadCustomDictionary(goldCsvPath, fstDict);
            }

            _dictionary.Build();
            _logger.LogInformation("Dictionary Built and Ready. Awaiting input...");

            string[] realDataTestInputs =
            {
                "הכלבים נבחו ברחוב",
                "י-ם של זהב",
                "פרופ' כהן",
                "תשפ\"ג",
                "המחשבים",
                "וכשמחשב"
            };

            foreach (var input in realDataTestInputs)
            {
                if (stoppingToken.IsCancellationRequested) break;

                _logger.LogInformation($"\n--- Analyzing: '{input}' ---");

                var tokens = _tokenizer.Tokenize(input);
                var analyzed = _lemmatizer.Analyze(tokens);

                foreach (var item in analyzed)
                {
                    string info = $"Token: [{item.OriginalToken.Text}]";
                    if (item.IsGematria)
                    {
                        info += $" -> GEMATRIA (Value: {item.GematriaValue})";
                    }
                    else if (item.IsStopWord)
                    {
                        info += $" -> STOP WORD";
                    }
                    else
                    {
                        info += " -> Lemmas: ";
                        var lemmaStrings = new List<string>();
                        foreach (var data in item.Lemmas)
                        {
                            if (data.Lemmas.Any())
                            {
                                lemmaStrings.Add(data.Lemmas.First().Text);
                            }
                        }
                        info += lemmaStrings.Any() ? string.Join(", ", lemmaStrings) : "UNKNOWN";
                    }
                    _logger.LogInformation(info);
                }

                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("Analysis complete. Worker standing by.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
