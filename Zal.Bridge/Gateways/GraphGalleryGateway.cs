using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zal.Bridge.Models;
using Zal.Bridge.Tools;

namespace Zal.Bridge.Gateways
{
    public class GraphGalleryGateway
    {

        public async Task<IEnumerable<GraphGalleryModel>> GetGalleriesAsync(int year)
        {
            var respond = await GraphApiClient.GetFiles(year);
            var model = GraphGalleryModel.From(respond, year);
            return model;
        }

        public async Task<IEnumerable<GraphPhotoModel>> GetPhotosAsync(int year, string galleryName)
        {
            var respond = await GraphApiClient.GetFiles(year, galleryName);
            var model = GraphPhotoModel.From(respond);
            return model;
        }

        public async Task<IEnumerable<GraphPhotoModel>> GetPhotosAsync(string id)
        {
            var respond = await GraphApiClient.GetFiles(id);
            var model = GraphPhotoModel.From(respond);
            return model;
        }

        public async Task<string> GetSharingLink(string id)
        {
            var respond = await GraphApiClient.GetSharingLink(id);
            return respond.link.webUrl;
        }

        public async Task<IEnumerable<string>> GetYears()
        {
            var respond = await GraphApiClient.GetFiles();
            var years = respond.FileItems.Select(x => x.name);
            return years;
        }

        public async Task<string> CreateFolder(int year)
        {
            var respond = await GraphApiClient.CreateFolder(year.ToString());
            var id = respond.id;
            return id;
        }

        public async Task<GraphGalleryModel> CreateFolder(int year, string name)
        {
            var respond = await GraphApiClient.CreateFolder(year.ToString(), name);
            var model = GraphGalleryModel.From(respond, year);
            return model;
        }

        //public async Task<IEnumerable<string>> GetAsync(int id)
        //{
        //    var respond = await SendRequestForNullable<IEnumerable<string>>(API.METHOD.GET, id);
        //    return respond ?? new List<string>();
        //}

        //public async Task<IEnumerable<GalleryModel>> GetAllAsync()
        //{
        //    var respond = await SendRequestForNullable<IEnumerable<GalleryModel>>(API.METHOD.GET_ALL);
        //    return respond ?? new List<GalleryModel>();
        //}

        //public Task<GalleryCreateRespondModel> AddAsync(GalleryModel model, string token)
        //{
        //    return SendRequestFor<GalleryCreateRespondModel>(API.METHOD.ADD, model, token);
        //}

        //public Task<bool> UpdateAsync(GalleryModel model, string token)
        //{
        //    //return SendRequestFor<bool>(API.METHOD.UPDATE, model, token);
        //    throw new NotImplementedException();
        //}

        //public async Task<bool> UploadImage(ImageUploadModel model, byte[] rawImage, string token)
        //{
        //    string respond = await SendImageUploadRequestFor(API.METHOD.UPLOAD_IMAGE, rawImage, model, token);
        //    return respond == "1";
        //}
    }
}
