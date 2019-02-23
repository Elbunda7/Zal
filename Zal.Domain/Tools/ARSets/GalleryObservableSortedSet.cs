using System.Collections.Generic;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Tools.ARSets;

namespace Zal.Domain.Tools.ARSets
{
    public class GalleryObservableSortedSet : ObservableSortedSet<Gallery>
    {
        public GalleryObservableSortedSet() : base(new ARComparer()) { }

        public GalleryObservableSortedSet(IEnumerable<Gallery> enumerable) : base(enumerable, new ARComparer()) { }
    }
}
