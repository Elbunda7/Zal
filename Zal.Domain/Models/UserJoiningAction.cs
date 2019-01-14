using System;
using System.Collections.Generic;
using System.Text;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Consts;

namespace Zal.Domain.Models
{
    public class UserJoiningAction
    {
        public User Member { get; set; }
        public bool IsGarant { get; set; }
        public ZAL.Joining Joining { get; set; }
    }
}
