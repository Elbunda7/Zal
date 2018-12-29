using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Models;

namespace Zal.Domain.Models
{
    public class BadgeUpdateModel : IUpdatableModel
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }

        public void CopyInto(IModel apiModel) {
            BadgeModel model = apiModel as BadgeModel;
            model.Name = Name;
            model.Text = Text;
            model.Image = Image;
        }

        public void CopyFrom(IModel apiModel) {
            BadgeModel model = apiModel as BadgeModel;
            Name = model.Name;
            Text = model.Text;
            Image = model.Image;
        }
    }
}
