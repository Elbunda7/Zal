using System;
using System.Collections.Generic;
using System.Text;
using Zal.Domain.ActiveRecords;

namespace Zal.Domain.Tools.ARSets
{
    public class GraphGalleryObservableSortedSet : ObservableSortedSet<GraphGallery>
    {
        public GraphGalleryObservableSortedSet() : base(new ARComparer()) { }

        public GraphGalleryObservableSortedSet(IEnumerable<GraphGallery> enumerable) : base(enumerable, new ARComparer()) { }
    }
}
