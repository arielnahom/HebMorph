using System.Collections.Generic;
using HebMorph.Core.Linguistics;

namespace HebMorph.Dictionary.Interfaces
{
    /// <summary>
    /// Represents the Morphological Dictionary logic decoupled from Elastic/Lucene analyzer specifics.
    /// Supports retrieving lemmas for given word forms.
    /// </summary>
    public interface IMorphologicalDictionary
    {
        /// <summary>
        /// Looks up a specific string form in the dictionary.
        /// </summary>
        /// <param name="form">The Hebrew word to lookup.</param>
        /// <returns>A list of MorphData objects (lemmas + masks) associated with the form.</returns>
        List<MorphData> Lookup(string form);

        /// <summary>
        /// Loads the dictionary from internal sources (like HSpell binary or JSON/CSV files).
        /// </summary>
        void Build();
    }
}
