using System.Collections.Generic;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Tools.ARSets;

namespace Zal.Domain.Tools.ARSets
{
    public class ActionObservableSortedSet : ObservableSortedSet<ActionEvent>
    {
        public ActionObservableSortedSet():base(new ActionComparer()) {}

        public ActionObservableSortedSet(IEnumerable<ActionEvent> enumerable) : base(enumerable, new ActionComparer()) { }        
    }
}
