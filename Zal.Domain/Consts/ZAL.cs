using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Domain.Consts
{
    public abstract class ZAL
    {
        public static DateTime DATE_OF_ORIGIN = new DateTime(1997, 2, 1);

        public readonly static string[] GROUP_NAME_PLURAL = { "Neoddíloví", "Zbloudilí jedinci", "Lišky", "Bobři", "Ještěrky", "Svišti", "Veverky", "Trosky" };   //todo out of range exception     
        public readonly static string[] GROUP_NAME_SINGULAR = { "Neoddílový", "Nestálý člen", "Liška", "Bobr", "Ještěrka", "Svišť", "Veverka", "Troska" };
        public readonly static string[] RANK_NAME = { "Liška", "Nováček", "Člen", "Kadet", "Podrádce", "Rádce", "Vedoucí", "Vedoucí", "Hlavní vedoucí" };

        public enum Group
        {
            Non = 1,
            Casual = 2,
            Lisky = 4,
            Bobri = 8,
            Jesterky = 16,
            Svisti = 32,
            Veverky = 64,
            Trosky = 128,

            BasicMemberGroups = 120,
            AllMemberGroups = 124,
            AllClub = 252,
            CompletlyAll = 255,
        }

        public enum UserRole
        {
            Registered = 1,
            Parent = 2,
            Admin = 4,

            All = 7,
        }

        public enum Rank
        {
            Liska = 0,
            Novacek = 1,
            Clen = 2,
            Kadet = 4,
            Podradce = 8,
            Radce = 16,
            Vedouci = 32,
            VedouciRada = 64,
            VedouciHlavni = 128,

            AllLeaders = 224,
            All = 255,
        }

        public enum ArticleType
        {
            Article = 1,
            Info = 2,
            Record = 3,
            Link = 4,
        }

        public enum Joining
        {
            True = 1,
            False = 2,
            Maybe = 3,
            Unknow = 4,
        }

        public abstract class MEMBERSHIP
        {
            public static string NECLEN = "nečlen";
            public static string CLEN = "člen";
        }

        public abstract class YEAR
        {
            public const int UPCOMING = 9999;
        }

        public enum ActionUserRole
        {
            Garant, Member, Any
        }
    }

    public static class ZalEnumsExtension
    {
        public static string AsString(this ZAL.Group group, bool isPlural = true)
        {
            return isPlural ? ZAL.GROUP_NAME_PLURAL[(int)group / 2] : ZAL.GROUP_NAME_SINGULAR[(int)group / 2];
        }

        public static string AsString(this ZAL.Rank rank)
        {
            return ZAL.RANK_NAME[(int)rank / 2];
        }
    }
}
