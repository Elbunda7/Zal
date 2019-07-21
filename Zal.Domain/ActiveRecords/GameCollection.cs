using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Gateways;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;
using Zal.Domain.Models;

namespace Zal.Domain.ActiveRecords
{
    public class GameCollection : IActiveRecord
    {
        private GameCollectionRespondModel Model;

        private List<MultiGame> gameList;
        private List<UserGroupedList> categorizedUsers;

        public int Id => Model.Id;
        public List<MultiGame> GameList => gameList ?? (gameList = Model.GetMultiGames().Select(x => new MultiGame(x)).ToList());
        public List<UserGroupedList> CategorizedUsers => categorizedUsers ?? ParseCategoriesFromModel();
        public int NumOfGames => Model.GetMultiGames().Count();
        public string Name => Model.Name;
        public Dictionary<string, int[]> Categories => Model.Categories.ToDictionary(x => x.Name, y => y.MembersArray);

        public bool HasManyGames => NumOfGames > 1;
        public bool HasOneMultiGame => NumOfGames == 1 && GameList[0].HasMultipleParts;
        public bool HasOneSimpleGame => NumOfGames == 1 && GameList[0].HasOnePart;

        private static GameGateway gateway;
        private static GameGateway Gateway => gateway ?? (gateway = new GameGateway());

        public GameCollection(Games_ActionModel model) : this(new GameCollectionRespondModel(model)) { }
        public GameCollection(GameCollectionRespondModel model)
        {
            Model = model;
        }

        private List<UserGroupedList> ParseCategoriesFromModel()
        {
            categorizedUsers = Model.GetCategories().Select(
                    x => new UserGroupedList(Zalesak.Users.GetAvailable(x.MembersArray), x.Name)
                ).ToList();
            return categorizedUsers;
        }
        
        public async Task<MultiGame> AddMultiGame(string name, params GameBaseModel[] games)
        {
            MultiGame multiGame = await AddMultiGame(name);
            await multiGame.AddGamesFirstTime(games);
            return multiGame;
        }

        public async Task<MultiGame> AddMultiGame(string name)
        {
            MultiGame multiGame = null;
            var model = new MultiGameModel
            {
                Name = name,
                Id_Games_on_Action = Id,
                GamesCount = 0,
            };
            bool isSuccess = await Gateway.AddMultiGame(model, "todo token");
            if (isSuccess)
            {
                var tmp = Model.GetMultiGames().ToList();
                tmp.Add(model);
                Model.MultiGames = tmp.ToArray();
                multiGame = new MultiGame(model);
                GameList.Add(multiGame);//todo někde to vytváří 2 hry najednou ?
            }
            return multiGame;
        }

        internal static async Task<IEnumerable<GameCollection>> GetAsync(int idAction)
        {
            var respond = await Gateway.GetCollectionAsync(idAction);
            return respond.Select(x => new GameCollection(x));
        }

        public async Task<bool> AddCategories(Dictionary<string, User[]> categories)
        {
            var model = categories.Select(x => new GameCategoryModel
            {
                Id_Games_on_Action = Id,
                Name = x.Key,
                MembersArray = x.Value.Select(y => y.Id).ToArray(),
            });
            bool isSuccess = await Gateway.AddCategories(model.ToArray(), "todo token");
            if (isSuccess)
            {
                Model.Categories = model.ToArray();
                categorizedUsers = null;
            }
            return isSuccess;
        }

        internal static async Task<GameCollection> Add(int idAction, string name, bool isPointRated, bool isIndividuals)
        {
            var model = new Games_ActionModel
            {
                IsIndividuals = isIndividuals,
                IsPointRated = isPointRated,
                Name = name,
                _Actions_Id = idAction,
            };
            bool isSuccess = await Gateway.AddGameCollection(model, "todo token");
            return isSuccess ? new GameCollection(model) : null;
        }
    }
}
