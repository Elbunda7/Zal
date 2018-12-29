using System;
using System.Collections.Generic;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;
using Zal.Domain.ActiveRecords;

namespace Zal.Domain.Models
{
    public class BaseChangedActiveRecords<AR> where AR : IActiveRecord
    {
        public bool IsChanged { get; set; }
        public DateTime Timestamp { get; set; }
        public IEnumerable<AR> Changed { get; set; }

        public BaseChangedActiveRecords(BaseChangesRespondFlags rawModel, IEnumerable<AR> activeRecords) {
            IsChanged = rawModel.IsChanged;
            Timestamp = rawModel.Timestamp;
            Changed = activeRecords;
        }
    }

    public class ChangedActiveRecords<AR> : BaseChangedActiveRecords<AR> where AR : IActiveRecord 
    {
        public bool IsHardChanged { get; set; }
        public bool HasAnyChanges => IsChanged || IsHardChanged;
        public int[] Deleted { get; set; }

        public ChangedActiveRecords(FullChangesRespondFlags rawModel, IEnumerable<AR> activeRecords) : base(rawModel, activeRecords) {
            IsHardChanged = rawModel.IsHardChanged;
            Deleted = rawModel.Deleted;
        }
    }
}
