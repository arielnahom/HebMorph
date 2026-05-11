using System.Collections.Generic;
using HebMorph.Core.Linguistics;
using HebMorph.Dictionary.Interfaces;

namespace HebMorph.Dictionary
{
    public class DictionaryManager
    {
        private readonly IMorphologicalDictionary _dictionary;

        public DictionaryManager(IMorphologicalDictionary dictionary)
        {
            _dictionary = dictionary;
        }

        public void LoadAndBuild()
        {
            _dictionary.Build();
        }

        public List<MorphData> Lookup(string form)
        {
            return _dictionary.Lookup(form);
        }
    }
}
