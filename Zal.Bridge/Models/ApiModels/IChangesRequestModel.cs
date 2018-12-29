using System;

namespace Zal.Bridge.Models.ApiModels
{
    public interface IChangesRequestModel
    {
        int Count { get; set; }
        DateTime LastCheck { get; set; }
    }
}
