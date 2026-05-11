using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using HebMorph.Dictionary.Interfaces;
using HebMorph.Dictionary.FST;
using HebMorph.Tokenizer;
using HebMorph.Lemmatizer;

namespace HebMorph.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMorphologicalDictionary, FstDictionary>();
                    services.AddSingleton<HebrewTokenizer>();
                    services.AddSingleton<HebrewLemmatizer>();
                    services.AddHostedService<Worker>();
                });
    }
}
