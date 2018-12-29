using System.Collections.Generic;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Tools.ARSets;

namespace Zal.Domain.Tools.ARSets
{
    public class UserObservableSortedSet:ObservableSortedSet<User>
    {
        public UserObservableSortedSet() : base(new UserComparer()) { }

        public UserObservableSortedSet(IEnumerable<User> enumerable) : base(enumerable, new UserComparer()) { }
    }
}
