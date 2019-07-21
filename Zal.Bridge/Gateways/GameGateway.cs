using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<GameCollectionRespondModel>> GetCollectionAsync(int id_action)
        {
            var respond = await SendRequestForNullable<IEnumerable<GameCollectionRespondModel>>(API.METHOD.GET_ALL, id_action);
            return respond ?? new List<GameCollectionRespondModel>();
        }

        public Task<IEnumerable<GameRespondModel>> GetGameAsync(int id_multipartGame)
        {
            return SendRequestFor<IEnumerable<GameRespondModel>>(API.METHOD.GET, id_multipartGame);
        }

        public async Task<bool> AddGameCollection(Games_ActionModel model, string token)
        {
            model.Id = await SendRequestFor<int>(API.METHOD.ADD_GAME_COLL, model, token);
            return model.Id != -1;
        }

        public async Task<bool> AddMultiGame(MultiGameModel model, string token)
        {
            model.Id = await SendRequestFor<int>(API.METHOD.ADD_MULTI_GAME, model, token);
            return model.Id != -1;
        }

        public async Task<bool> AddGames(GameModel[] model, string token)
        {
            int lastId = await SendRequestFor<int>(API.METHOD.ADD, model, token);
            bool isSuccess = lastId != -1;
            if (isSuccess)
            {
                int firstId = lastId - model.Length + 1;
                for (int i = 0; i < model.Length; i++)
                {
                    model[i].Id = firstId + i;
                }
            }
            return isSuccess;
        }

        public async Task<bool> AddCategories(GameCategoryModel[] model, string token)
        {
            int lastId = await SendRequestFor<int>(API.METHOD.ADD_CATEGORY, model, token);
            bool isSuccess = lastId != -1;
            if (isSuccess)
            {
                int firstId = lastId - model.Length + 1;
                for (int i = 0; i < model.Length; i++)
                {
                    model[i].Id = firstId + i;
                }
            }
            return isSuccess;
        }

        public async Task<bool> AddScoreAsync(ScoreModel model, string token)
        {
            model.Id = await SendRequestFor<int>(API.METHOD.ADD_SCORE, model, token);
            return model.Id != -1;
        }

        public Task<bool> UpdateScoreAsync(ScoreModel model, string token)
        {
            return SendRequestFor<bool>(API.METHOD.UPDATE_SCORE, model, token);
        }
        /*
public async Task<IEnumerable<GalleryModel>> GetAllAsync()
{
var respond = await SendRequestForNullable<IEnumerable<GalleryModel>>(API.METHOD.GET_ALL);
return respond ?? new List<GalleryModel>();
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
