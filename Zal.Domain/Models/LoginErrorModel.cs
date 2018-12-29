using Zal.Bridge.Models.ApiModels;

namespace Zal.Domain.Models
{
    public class LoginErrorModel
    {
        public bool IsExist { get; set; }
        public bool IsPasswordCorrect { get; set; }
        public bool HasAnyErrors => !(IsExist && IsPasswordCorrect);

        public LoginErrorModel(LoginRespondModel model) {
            IsExist = model.isExist;
            IsPasswordCorrect = model.isPasswordCorrect;
        }
    }
}
