using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;
using Zal.Bridge.Tools;

namespace Zal.Bridge.Gateways
{
    [Obsolete]
    public class GalleryGateway : Gateway
    {
        public GalleryGateway() : base(API.ENDPOINT.GALLERY) { }

        public async Task<IEnumerable<string>> GetAsync(int id)
        {
            var respond = await SendRequestForNullable<IEnumerable<string>>(API.METHOD.GET, id);
            return respond ?? new List<string>();
        }

        public async Task<IEnumerable<GalleryModel>> GetAllAsync()
        {
            var respond = await SendRequestForNullable<IEnumerable<GalleryModel>>(API.METHOD.GET_ALL);
            return respond ?? new List<GalleryModel>();
        }

        public Task<GalleryCreateRespondModel> AddAsync(GalleryModel model, string token)
        {
            return SendRequestFor<GalleryCreateRespondModel>(API.METHOD.ADD, model, token);
        }

        public Task<bool> UpdateAsync(GalleryModel model, string token)
        {
            //return SendRequestFor<bool>(API.METHOD.UPDATE, model, token);
            throw new NotImplementedException();
        }

        public async Task<bool> UploadImage(ImageUploadModel model, byte[] rawImage, string token)
        {
            string respond = await SendImageUploadRequestFor(API.METHOD.UPLOAD_IMAGE, rawImage, model, token);
            return respond == "1";
        }
    }
}
