namespace HebMorph.Core.Linguistics
{
    public class HebrewCharacters
    {
        public const char ALEF = '\u05d0';
        public const char BET = '\u05d1';
        public const char GIMEL = '\u05d2';
        public const char DALET = '\u05d3';
        public const char HE = '\u05d4';
        public const char VAV = '\u05d5';
        public const char ZAYIN = '\u05d6';
        public const char HET = '\u05d7';
        public const char TET = '\u05d8';
        public const char YUD = '\u05d9';
        public const char KAF_SOFIT = '\u05da';
        public const char KAF = '\u05db';
        public const char LAMED = '\u05dc';
        public const char MEM_SOFIT = '\u05dd';
        public const char MEM = '\u05de';
        public const char NUN_SOFIT = '\u05df';
        public const char NUN = '\u05e0';
        public const char SAMECH = '\u05e1';
        public const char AYIN = '\u05e2';
        public const char PE_SOFIT = '\u05e3';
        public const char PE = '\u05e4';
        public const char TZADI_SOFIT = '\u05e5';
        public const char TZADI = '\u05e6';
        public const char KOF = '\u05e7';
        public const char RESH = '\u05e8';
        public const char SHIN = '\u05e9';
        public const char TAV = '\u05ea';

        public const char NIKKUD_SHEVA = '\u05b0';
        public const char NIKKUD_HATAF_SEGOL = '\u05b1';
        public const char NIKKUD_HATAF_PATAH = '\u05b2';
        public const char NIKKUD_HATAF_QAMATS = '\u05b3';
        public const char NIKKUD_HIRIQ = '\u05b4';
        public const char NIKKUD_TSERE = '\u05b5';
        public const char NIKKUD_SEGOL = '\u05b6';
        public const char NIKKUD_PATAH = '\u05b7';
        public const char NIKKUD_QAMATS = '\u05b8';
        public const char NIKKUD_HOLAM = '\u05b9';
        public const char NIKKUD_QUBUTS = '\u05bb';
        public const char NIKKUD_DAGESH = '\u05bc';
        public const char NIKKUD_METEG = '\u05bd';
        public const char NIKKUD_MAQAF = '\u05be';
        public const char NIKKUD_RAFE = '\u05bf';
        public const char NIKKUD_SHIN_DOT = '\u05c1';
        public const char NIKKUD_SIN_DOT = '\u05c2';

        public const char GERESH = '\u05f3';
        public const char GERSHAYIM = '\u05f4';

        public static bool IsHebrewLetter(char c)
        {
            return c >= ALEF && c <= TAV;
        }

        public static bool IsSofit(char c)
        {
            return c == KAF_SOFIT || c == MEM_SOFIT || c == NUN_SOFIT || c == PE_SOFIT || c == TZADI_SOFIT;
        }
    }
}
