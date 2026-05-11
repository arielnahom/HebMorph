using System.Linq;

namespace HebMorph.Core.Linguistics
{
    public static class HebrewUtils
    {
        public static readonly char[] Geresh = { '\'', '\u05F3', '\u2018', '\u2019', '\u201B', '\uFF07' };
        public static readonly char[] Gershayim = { '\"', '\u05F4', '\u201C', '\u201D', '\u201F', '\u275E', '\uFF02' };
        public static readonly char[] Makaf = { '-', '\u2012', '\u2013', '\u2014', '\u2015', '\u05BE' };
        public static readonly char[] CharsFollowingPrefixes = Geresh.Concat(Gershayim).Concat(Makaf).ToArray();
        public static readonly char[] LettersAcceptingGeresh = { 'ז', 'ג', 'ץ', 'צ', 'ח' };

        public static bool IsOfChars(char c, char[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (c == options[i]) return true;
            }
            return false;
        }

        public static bool IsHebrewLetter(char c)
        {
            return c >= 1488 && c <= 1514;
        }

        public static bool IsFinalHebrewLetter(char c)
        {
            return c == 1507 || c == 1498 || c == 1501 || c == 1509 || c == 1503;
        }

        public static bool IsNiqqudChar(char c)
        {
            return (c >= 1456 && c <= 1465) || c == '\u05C1' || c == '\u05C2' || c == '\u05BB' || c == '\u05BC';
        }
    }
}
