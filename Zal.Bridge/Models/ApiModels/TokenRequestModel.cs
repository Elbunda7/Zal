using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Bridge.Models.ApiModels
{
    public class TokenRequestModel
    {
        public int IdUser { get; set; }
        public string RefreshToken { get; set; }
    }
}
