using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Bridge.Models.ApiModels
{
    public class MembersOnActionModel
    {
        public UserModel Member { get; set; }
        public bool IsGarant { get; set; }
        public int Joining { get; set; }
    }
}
