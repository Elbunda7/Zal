using Zal.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Domain.ActiveRecords;

namespace Zal.Domain.Tools
{
    internal static class UserPermision
    {
        internal static void HasRank(User user, ZAL.Rank expectedRank) {
            if (user == null) {
                throw new UserPermisionException("null exception!");
            }
            if (user.Rank < expectedRank) {
                throw new UserPermisionException($"Low user Rank - expected: [{ZAL.RANK_NAME[(int)expectedRank]}], current: [{ZAL.RANK_NAME[(int)user.Rank]}]");
            }
        }
    }
}
