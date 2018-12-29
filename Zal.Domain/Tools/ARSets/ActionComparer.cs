using Zal.Domain.ActiveRecords;
using System.Collections.Generic;


namespace Zal.Domain.Tools.ARSets
{
    public class ActionComparer : Comparer<ActionEvent>
    {
        public override int Compare(ActionEvent x, ActionEvent y) {
            int comparison = Comparer<long>.Default.Compare(x.DateFrom.Ticks, y.DateFrom.Ticks);
            if (comparison == 0) {
                comparison = Comparer<int>.Default.Compare(x.Days, y.Days);
            }
            if (comparison == 0) {
                comparison = Comparer<int>.Default.Compare(x.Id, y.Id);
            }
            return comparison;
        }
    }
}