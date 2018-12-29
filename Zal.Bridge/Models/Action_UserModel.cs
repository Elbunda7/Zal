using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Bridge.Models
{
    public class ActionUserModel
    {
        public int Id_User { get; set; }
        public int Id_Action { get; set; }
    }

    public class ActionUserJoinModel : ActionUserModel
    {
        public bool IsGarant { get; set; }
    }
}
