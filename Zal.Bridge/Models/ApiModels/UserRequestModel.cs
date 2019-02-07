using System;

namespace Zal.Bridge.Models.ApiModels
{
    public class UserRequestModel
    {
        public int Groups { get; set; }
        public int Ranks { get; set; }
        public int Attribs { get; set; }
    }

    public class UserChangesRequestModel : UserRequestModel, IChangesRequestModel
    {
        public int Count { get; set; }
        public DateTime LastCheck { get; set; }
    }
}
