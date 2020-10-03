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

        public enum UserAttribs
        {
            Registered = 1,
            ConfirmedEmail = 2,
            Parent = 4,
            Admin = 8,
            RegCompleted = 16,
            Approved = 32,

            All = 63,
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

        public static int AsIndex(this ZAL.Group group)
        {
            double value = Math.Log((int)group, 2);
            double index = Math.Floor(value);
            if (index != value) throw new InvalidOperationException("It has to be just one group!");
            return (int)index;
        }

        public static ZAL.Group AsGroup(this int index)
        {
            return (ZAL.Group)Math.Pow(2, index);
        }

        public static string GetColorCode(this ZAL.Group group)
        {
            switch (group)
            {
                case ZAL.Group.Non: return "#000000";
                case ZAL.Group.Casual:return "#757575";
                case ZAL.Group.Lisky:return "#9dcacd";
                case ZAL.Group.Bobri:return "#ffec00";
                case ZAL.Group.Jesterky:return "#e30016";
                case ZAL.Group.Svisti:return "#83c326";
                case ZAL.Group.Veverky:return "#0191db";
                case ZAL.Group.Trosky:return "#FFDEAD";
                default:return "#000000";
            }
        }
    }
}
