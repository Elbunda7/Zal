using System;
using System.Collections.Generic;
using System.Text;
using Zal.Bridge.Models.ApiModels;
using System.Linq;

namespace Zal.Bridge.Models
{
    public class GraphGalleryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int ItemsCount { get; set; }
        public int Year { get; set; }
        public string ThumbUrl { get; set; }

        public static IEnumerable<GraphGalleryModel> From(GraphFilesModel graphFilesModel, int year)
        {
            var thumbItems = graphFilesModel.FileItems.Where(x => x.thumbnails.Count >= 1);
            var galleryModels = graphFilesModel.FileItems.Where(x => x.folder != null).Select(y => new GraphGalleryModel
            {
                Id = y.id,
                Name = y.name,
                Year = year,
                ItemsCount = y.folder.childCount,
                ThumbUrl = thumbItems.FirstOrDefault(x => x.name.Substring(0, x.name.LastIndexOf('.')).Equals(y.name))?.thumbnails.First().thumb.url,
            });
            return galleryModels;
        }
    }
}
