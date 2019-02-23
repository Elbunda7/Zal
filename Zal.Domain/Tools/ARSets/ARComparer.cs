using System.Collections.Generic;
using Zal.Domain.ActiveRecords;

namespace Zal.Domain.Tools.ARSets
{
    internal class ARComparer : Comparer<IActiveRecord>
    {
        public override int Compare(IActiveRecord x, IActiveRecord y)
        {
            return Comparer<int>.Default.Compare(x.Id, y.Id);
        }
    }
}