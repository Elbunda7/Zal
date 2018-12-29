using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Bridge.Models
{
    public class BadgeModel : IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
    }
}
