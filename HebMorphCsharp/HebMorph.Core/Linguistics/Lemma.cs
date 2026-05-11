using System;

namespace HebMorph.Core.Linguistics
{
    public class Lemma
    {
        public string Text { get; set; } = string.Empty;
        public DescFlag DescFlag { get; set; }
        public PrefixType Prefix { get; set; }

        public Lemma() {}

        public Lemma(string text, DescFlag descFlag, PrefixType prefix)
        {
            Text = text;
            DescFlag = descFlag;
            Prefix = prefix;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Lemma other)
            {
                return Text == other.Text && DescFlag == other.DescFlag && Prefix == other.Prefix;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, DescFlag, Prefix);
        }

        public override string ToString()
        {
            return $"{Text}:{DescFlag}:{Prefix}";
        }
    }
}
