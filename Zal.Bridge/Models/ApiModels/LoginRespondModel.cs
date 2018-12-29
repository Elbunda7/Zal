using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Bridge.Models.ApiModels
{
    public class LoginRespondModel
    {
        public bool isExist { get; set; }
        public bool isPasswordCorrect { get; set; }
        public bool HasAnyErrors => !(isExist && isPasswordCorrect);

        public UserModel UserModel { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
