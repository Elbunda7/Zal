using System;
using Zal.Bridge.Models;
using Zal.Domain.Consts;

namespace Zal.Domain.Models
{
    public class ActionUpdateModel:IUpdatableModel
    {
        public string Name { get; set; }
        public DateTime Date_start { get; set; }
        public DateTime Date_end { get; set; }
        public string EventType { get; set; }
        public ZAL.Rank FromRank { get; set; }
        public bool IsOfficial { get; set; }

        public void CopyInto(IModel apiModel) {
            ActionModel model = apiModel as ActionModel;
            model.Name = Name;
            model.Date_start = Date_start;
            model.Date_end = Date_end;
            model.EventType = EventType;
            model.FromRank = (int)FromRank;
            model.IsOfficial = IsOfficial;
        }

        public void CopyFrom(IModel apiModel) {
            ActionModel model = apiModel as ActionModel;
            Name = model.Name;
            Date_start = model.Date_start;
            Date_end = model.Date_end;
            EventType = model.EventType;
            FromRank = (ZAL.Rank)model.FromRank;
            IsOfficial = model.IsOfficial;
        }
    }
}
