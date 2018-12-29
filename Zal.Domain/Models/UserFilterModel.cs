using System;
using Zal.Domain.Consts;

namespace Zal.Domain.Models
{
    public class UserFilterModel
    {
        public ZAL.Group Groups { get; set; }
        public ZAL.Rank Ranks { get; set; }
        public ZAL.UserRole Roles { get; set; }

        public static UserFilterModel Default {
            get {
                return new UserFilterModel() {
                    Groups = ZAL.Group.AllClub,
                    Ranks = ZAL.Rank.All,
                    Roles = ZAL.UserRole.All,
                };
            }
        }

        public UserFilterModel() {
            Clear();
        }

        public void Set(ZAL.Group groups, ZAL.Rank ranks, ZAL.UserRole roles) {
            Groups = groups;
            Ranks = ranks;
            Roles = roles;
        }

        internal void Clear() {
            Groups = 0;
            Ranks = 0;
            Roles = 0;
        }

        internal bool WillBeExtendedWith(UserFilterModel filter) {
            bool value = false;
            value |= (~Groups & filter.Groups) != 0;
            value |= (~Ranks & filter.Ranks) != 0;
            value |= (~Roles & filter.Roles) != 0;
            return value;
        }

        internal UserFilterModel GetOnlyExtendigFilterFrom(UserFilterModel filter) {
            return new UserFilterModel() {
                Groups = ~Groups & filter.Groups,
                Ranks = ~Ranks & filter.Ranks,
                Roles = ~Roles & filter.Roles,
            };
        }

        internal void CombineWith(UserFilterModel filter) {
            Groups |= filter.Groups;
            Ranks |= filter.Ranks;
            Roles |= filter.Roles;
        }

        internal bool CanContains(ZAL.Group group, ZAL.Rank rank, ZAL.UserRole roles) {
            bool value = true;
            value &= (Groups & group) != 0;
            value &= (Ranks & rank) != 0;
            value &= (Roles & roles) != 0;
            return value;
        }
    }
}
