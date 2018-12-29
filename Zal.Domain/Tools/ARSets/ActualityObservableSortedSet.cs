using Zal.Domain.ActiveRecords;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Zal.Domain.Tools.ARSets;

namespace Zal.Domain.Tools.ARSets
{
    public class ActualityObservableSortedSet:ObservableSortedSet<Article>
    {
        public ActualityObservableSortedSet() : base(new ActualityComparer()) { }        

        public ActualityObservableSortedSet(IEnumerable<Article> enumerable) : base(enumerable, new ActualityComparer()) { }
    }
}