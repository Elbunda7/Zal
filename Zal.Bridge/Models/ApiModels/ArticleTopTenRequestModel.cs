using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Bridge.Models.ApiModels
{
    public class ArticleTopTenRequestModel
    {
        public int[] Ids { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
