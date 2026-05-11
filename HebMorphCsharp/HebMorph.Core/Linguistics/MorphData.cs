using System;
using System.Collections.Generic;
using System.Linq;

namespace HebMorph.Core.Linguistics
{
    public class MorphData
    {
        public List<Lemma> Lemmas { get; set; }
        public short Prefixes { get; set; }
        public bool HaltIfFound { get; set; }

        public MorphData()
        {
            Lemmas = new List<Lemma>();
        }

        // Convenience legacy constructor for tests
        public MorphData(string lemma, ulong mask) : this()
        {
            AddLemma(new Lemma(lemma, DescFlag.D_EMPTY, PrefixType.PS_EMPTY));
        }

        public void AddLemma(Lemma lemma)
        {
            Lemmas.Add(lemma);
            Lemmas.Sort((l1, l2) => l1.DescFlag.CompareTo(l2.DescFlag));
        }

        public void SetLemmas(IEnumerable<Lemma> lemmas)
        {
            Lemmas = lemmas.ToList();
            Lemmas.Sort((l1, l2) => l1.DescFlag.CompareTo(l2.DescFlag));
        }

        public override bool Equals(object? obj)
        {
            if (obj is MorphData other)
            {
                if (Prefixes != other.Prefixes) return false;
                if (HaltIfFound != other.HaltIfFound) return false;
                if (Lemmas.Count != other.Lemmas.Count) return false;

                for (int i = 0; i < Lemmas.Count; i++)
                {
                    if (!Lemmas[i].Equals(other.Lemmas[i])) return false;
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = HashCode.Combine(Prefixes, HaltIfFound);
            foreach (var lemma in Lemmas)
            {
                hash = HashCode.Combine(hash, lemma.GetHashCode());
            }
            return hash;
        }

        public override string ToString()
        {
            return $"{{ prefix={Prefixes} lemmas=[{string.Join(", ", Lemmas)}] }}";
        }
    }
}
