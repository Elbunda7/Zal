using System.Collections.Generic;
using Zal.Domain.ActiveRecords;

namespace Zal.Domain.Tools.ARSets
{
    public class ActiveRecordEqualityComparer : IEqualityComparer<IActiveRecord>
    {
        private static ActiveRecordEqualityComparer instance;
        public static ActiveRecordEqualityComparer Instance => instance ?? (instance = new ActiveRecordEqualityComparer());

        public bool Equals(IActiveRecord x, IActiveRecord y) {
            return x.Id == y.Id;
        }

        public int GetHashCode(IActiveRecord obj) {
            return obj.GetHashCode();
        }
    }
}
