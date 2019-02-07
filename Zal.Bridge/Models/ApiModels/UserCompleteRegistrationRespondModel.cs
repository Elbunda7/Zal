using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models.ApiModels
{
    public class UserCompleteRegistrationRespondModel
    {
        public bool RequestOrdered { get; set; }
        public bool SuccesfullyMerged { get; set; }
        public UserModel UserUpdated { get; set; }
        public int IdDeleted { get; set; }
    }
}
