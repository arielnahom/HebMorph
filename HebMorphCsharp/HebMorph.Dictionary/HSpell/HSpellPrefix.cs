using System.Collections.Generic;

namespace HebMorph.Dictionary.HSpell
{
    public class HSpellPrefix
    {
        public string Text { get; set; } = string.Empty;
        public short Hint { get; set; }

        public HSpellPrefix(string text, short hint)
        {
            Text = text;
            Hint = hint;
        }
    }
}
