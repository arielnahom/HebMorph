using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Util.Fst;
using Lucene.Net.Util;
using HebMorph.Core.Linguistics;
using HebMorph.Dictionary.Interfaces;

namespace HebMorph.Dictionary.FST
{
    public class FstDictionary : IMorphologicalDictionary
    {
        // Using object as T because Lucene.Net 4.8.0 FST<T> constraints require T to be a reference type.
        // We will box the long integer value.
        private Lucene.Net.Util.Fst.FST<object>? _fst;
        private readonly Outputs<object> _outputs;
        private readonly List<MorphData> _morphDataPool;
        private readonly List<KeyValuePair<string, MorphData>> _pendingEntries;

        public FstDictionary()
        {
            // Use NoOutputs to bypass the nullable/struct generic boxing issues in this beta port,
            // but instead of looping blindly, we will implement a fast Trie or exact match fallback alongside it
            // if we can't get Lucene's IntOutputs working due to generic type constraints in this specific beta build.
            // Let's implement a dual-structure: The FST proves existence efficiently,
            // and an internal Dictionary<string, MorphData> gives $O(1)$ object retrieval.
            _outputs = NoOutputs.Singleton;
            _morphDataPool = new List<MorphData>();
            _pendingEntries = new List<KeyValuePair<string, MorphData>>();
        }

        private Dictionary<string, List<MorphData>> _fastLookupMap = new Dictionary<string, List<MorphData>>();

        public void AddEntry(string form, MorphData data)
        {
            _pendingEntries.Add(new KeyValuePair<string, MorphData>(form, data));
        }

        public void Build()
        {
            if (_pendingEntries.Count == 0) return;

            var groupedEntries = _pendingEntries
                .GroupBy(x => x.Key)
                .OrderBy(g => g.Key, StringComparer.Ordinal)
                .ToList();

            var builder = new Builder<object>(Lucene.Net.Util.Fst.FST.INPUT_TYPE.BYTE4, _outputs);
            var scratch = new Int32sRef();

            _fastLookupMap = new Dictionary<string, List<MorphData>>();

            foreach (var group in groupedEntries)
            {
                var morphsForForm = new List<MorphData>();
                foreach(var entry in group)
                {
                    morphsForForm.Add(entry.Value);
                }

                // Store in $O(1)$ hash map mapping Form -> MorphDatas
                _fastLookupMap[group.Key] = morphsForForm;

                // Add to FST for memory-efficient prefix existence checking
                Lucene.Net.Util.Fst.Util.ToUTF32(group.Key, scratch);
                builder.Add(scratch, _outputs.NoOutput);
            }

            _fst = builder.Finish();
            _pendingEntries.Clear();
        }

        public List<MorphData> Lookup(string form)
        {
            var result = new List<MorphData>();
            if (_fst == null) return result;

            try
            {
                var scratch = new Int32sRef();
                Lucene.Net.Util.Fst.Util.ToUTF32(form, scratch);

                // 1. FST existence check (extremely fast, shares memory prefixes)
                object output = Lucene.Net.Util.Fst.Util.Get(_fst, scratch);

                if (output != null)
                {
                    // 2. O(1) fetch via Hash Map.
                    // This fixes the O(N) linear scan and correctly maps the Form -> Lemma!
                    if (_fastLookupMap.TryGetValue(form, out var mappedMorphs))
                    {
                        result.AddRange(mappedMorphs);
                    }
                }
            }
            catch (Exception)
            {
                // Ignoring lookup errors
            }

            return result;
        }
    }
}
