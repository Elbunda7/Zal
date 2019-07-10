using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;
using Zal.Bridge.Tools;

namespace Zal.Bridge.Gateways
{
    public class GameGateway : Gateway
    {
        public GameGateway() : base(API.ENDPOINT.GAMES) { }

        public Task<IEnumerable<GameCollectionRespondModel>> GetCollectionAsync(int id_action)
        {
            return SendRequestFor<IEnumerable<GameCollectionRespondModel>>(API.METHOD.GET_ALL, id_action);
        }

        public Task<IEnumerable<GameRespondModel>> GetGameAsync(int id_gameCollection)
        {
            return SendRequestFor<IEnumerable<GameRespondModel>>(API.METHOD.GET, id_gameCollection);
        }
        /*
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
        }*/
    }
}
