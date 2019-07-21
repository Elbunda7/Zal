using System;
using System.Collections.Generic;
using System.Text;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Tools.ARSets;

namespace Zal.Domain.Models
{
    public class UserGroupedList : UserObservableSortedSet
    {
        public string GroupTitle { get; set; }
        public string GroupValue { get; set; }

        public UserGroupedList(IEnumerable<User> users, string group) : base(users)
        {
            GroupTitle = group;
            GroupValue = group;
        }
    }
}
