using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Zal.Domain.ActiveRecords;

namespace Zal.Domain.Models
{
    public class ScoreGroupedList : ObservableCollection<Score>
    {
        public string GroupTitle { get; set; }
        public string GroupValue { get; set; }

        public ScoreGroupedList(IEnumerable<Score> scores, string group) : base(scores)
        {
            GroupTitle = group;
            GroupValue = group;
        }
        
    }
}
