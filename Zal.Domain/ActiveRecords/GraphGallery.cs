using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zal.Bridge.Gateways;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;

namespace Zal.Domain.ActiveRecords
{
    public class GraphGallery : IActiveRecord
    {
        private GraphGalleryModel Model;
        private List<GraphPhoto> photos;
        private List<string> images;

        public int Id => IdStr.GetHashCode();//todo vymyslet lepši id, asi z data vytvoření
        public string IdStr => Model.Id;
        public string Name => Model.Name;
        public int Year => Model.Year;
        public int ItemsCount => Model.ItemsCount;
        public string ThumbUrl => Model.ThumbUrl;
        //public DateTime Date => Model.Date;

        private static GraphGalleryGateway gateway;
        private static GraphGalleryGateway Gateway => gateway ?? (gateway = new GraphGalleryGateway());

        public async Task<IEnumerable<GraphPhoto>> ImagesLazyLoad()
        {
            //if (images == null)
            //{
            //    images = (await Gateway.GetAsync(Id)).ToList();
            //}
            return photos ?? (photos = (await Gateway.GetPhotosAsync(IdStr)).Select(x => new GraphPhoto(x)).ToList());
        }

        internal GraphGallery(GraphGalleryModel model)
        {
            Model = model;
        }

        //public async Task<bool> Upload(string imageName, byte[] rawImage, bool isMain = false)
        //{
        //    var model = new ImageUploadModel
        //    {
        //        Id = Id,
        //        SetAsMain = isMain,
        //        Name = imageName,
        //    };
        //    bool isUploaded = await Gateway.UploadImage(model, rawImage, "Zalesak.Session.Token");
        //    if (isUploaded) images?.Add(imageName);
        //    return isUploaded;
        //}

        internal static async Task<IEnumerable<GraphGallery>> GetGalleries(int year)
        {
            var respond = await Gateway.GetGalleriesAsync(year);
            var galleries = respond.Select(model => new GraphGallery(model));
            return galleries;
        }

        internal static Task<IEnumerable<string>> GetYears() => Gateway.GetYears();

    }

    public class GraphPhoto
    {
        private GraphPhotoModel Model;

        public string Id => Model.Id;
        public string Name => Model.Name;
        public string ThumbUrl => Model.ThumbUrl;

        internal GraphPhoto(GraphPhotoModel model)
        {
            Model = model;
        }
    }
}
