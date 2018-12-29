using System;

namespace Zal.Bridge.Models.ApiModels
{
    public class ActionRequestModel
    {
        public int Rank { get; set; }
        public int Year { get; set; }
    }

    public class ActionChangesRequestModel : ActionRequestModel, IChangesRequestModel
    {
        public int Count { get; set; }
        public DateTime LastCheck { get; set; }
    }
}
