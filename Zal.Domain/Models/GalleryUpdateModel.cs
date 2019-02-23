using System;
using System.Collections.Generic;
using System.Text;
using Zal.Bridge.Models;

namespace Zal.Domain.Models
{
    public class GalleryUpdateModel : IUpdatableModel
    {
        public string Name { get; set; }

        public void CopyInto(IModel apiModel)
        {
            GalleryModel model = apiModel as GalleryModel;
            model.Name = Name;
        }

        public void CopyFrom(IModel apiModel)
        {
            GalleryModel model = apiModel as GalleryModel;
            Name = model.Name;
        }
    }
}
