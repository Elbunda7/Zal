using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Domain.Consts
{
    internal abstract class CONST
    {
        internal abstract class CHANGES
        {
            internal static string MAJOR = "major";
            internal static string MINOR = "minor";
            internal static string NONE = "none";
        }

        public abstract class AKCE
        {
            public const int ID = 1;
            public const int JMENO = 2;
            public const int TYP = 3;
            public const int POCET_DNI = 4;
            public const int EMAIL_VEDOUCI = 5;
            public const int OD_HODNOSTI = 6;
            public const int JE_OFICIALNI = 7;
            public const int DATUM_OD = 8;
            public const int DATUM_DO = 9;
            public const int DATUM = 10;
        }

        public abstract class USER
        {
            public const int EMAIL = 1;
            public const int JMENO = 2;
            public const int PRIJMENI = 3;
            public const int DRUZINA = 4;
            public const int PREZDIVKA = 5;
            public const int ROLE = 6;
            public const int HODNOST = 7;
            public const int KONTAKT = 8;
            public const int BODY = 9;
            public const int ZAPLATIL_PRISPEVEK = 10;
            public const int DATUM_NAROZENI = 11;
        }
    }
}
