using System;
using Zal.Domain.Consts;

namespace Zal.Domain.Models
{
    public class UserFilterModel
    {
        public ZAL.Group Groups { get; set; }
        public ZAL.Rank Ranks { get; set; }
        public ZAL.UserAttribs Attribs { get; set; }

        public static UserFilterModel Default {
            get {
                return new UserFilterModel() {
                    Groups = ZAL.Group.AllClub,
                    Ranks = ZAL.Rank.All,
                    Attribs = ZAL.UserAttribs.All,
                };
            }
        }

        public UserFilterModel() {
            Clear();
        }

        public void Set(ZAL.Group groups, ZAL.Rank ranks, ZAL.UserAttribs attribs) {
            Groups = groups;
            Ranks = ranks;
            Attribs = attribs;
        }

        internal void Clear() {
            Groups = 0;
            Ranks = 0;
            Attribs = 0;
        }

        public static bool operator ==(UserFilterModel a, UserFilterModel b)
        {
            bool value = true;
            value &= a.Groups == b.Groups;
            value &= a.Ranks == b.Ranks;
            value &= a.Attribs == b.Attribs;
            return value;
        }

        public static bool operator !=(UserFilterModel a, UserFilterModel b)
        {
            bool value = false;
            value |= a.Groups != b.Groups;
            value |= a.Ranks != b.Ranks;
            value |= a.Attribs != b.Attribs;
            return value;
        }

        internal bool IsExtending(UserFilterModel filter)
        {
            bool value = false;
            value |= (Groups & ~filter.Groups) != 0;
            value |= (Ranks & ~filter.Ranks) != 0;
            value |= (Attribs & ~filter.Attribs) != 0;
            return value;
        }

        internal UserFilterModel GetOnlyExtendigFilterFrom(UserFilterModel filter) {
            return new UserFilterModel() {
                Groups = ~Groups & filter.Groups,
                Ranks = ~Ranks & filter.Ranks,
                Attribs = ~Attribs & filter.Attribs,
            };
        }

        internal void CombineWith(UserFilterModel filter) {
            Groups |= filter.Groups;
            Ranks |= filter.Ranks;
            Attribs |= filter.Attribs;
        }

        internal bool CanContains(ZAL.Group group, ZAL.Rank rank, ZAL.UserAttribs attribs) {
            bool value = true;
            value &= (Groups & group) != 0;
            value &= (Ranks & rank) != 0;
            value &= (Attribs & attribs) != 0;
            return value;
        }
    }
}
