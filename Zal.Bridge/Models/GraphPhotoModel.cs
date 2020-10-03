using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zal.Bridge.Models.ApiModels;

namespace Zal.Bridge.Models
{
    public class GraphPhotoModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ThumbUrl { get; set; }

        public static IEnumerable<GraphPhotoModel> From(GraphFilesModel graphFilesModel)
        {
            var galleryModels = graphFilesModel.FileItems.Where(x => x.thumbnails.Count >= 1).Select(y => new GraphPhotoModel
            {
                Id = y.id,
                Name = y.name,
                ThumbUrl = y.thumbnails.First().thumb.url,
            });
            return galleryModels;
        }
    }
}
