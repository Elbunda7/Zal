using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Gateways;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;

namespace Zal.Domain.ActiveRecords
{
    public class GameCollection : IActiveRecord
    {
        private GameCollectionRespondModel Model;

        private IEnumerable<Game> Games;
        private List<MultiGame> gameList;

        public int Id => Model.Id;
        public List<MultiGame> GameList => gameList ?? (gameList = Model.MultiGames.Select(x => new MultiGame(x)).ToList());
        public int NumOfGames => Model.MultiGames.Count();
        public string Name => Model.Name;

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

        public async Task<bool> AddMultiGame(string name)
        {
            var model = new MultiGameModel
            {
                Name = name,
                Id_Games_on_Action = Id,
                GamesCount = 0,
            };
            bool isSuccess = await Gateway.AddMultiGame(model, "todo token");
            if (isSuccess)
            {
                var tmp = Model.MultiGames.ToList();
                tmp.Add(model);
                Model.MultiGames = tmp.ToArray();
                GameList.Add(new MultiGame(model));
            }
            return isSuccess;
        }

        internal static async Task<IEnumerable<GameCollection>> GetAsync(int idAction)
        {
            var respond = await Gateway.GetCollectionAsync(idAction);
            return respond.Select(x => new GameCollection(x));
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

    public class MultiGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumOfGames { get; set; }

        public bool HasMultipleParts => NumOfGames > 1;
        public bool HasOnePart => NumOfGames == 1;

        private IEnumerable<Game> games;

        public MultiGame(MultiGameModel model)
        {
            Id = model.Id;
            Name = model.Name;
            NumOfGames = model.GamesCount;
        }

        public async Task<IEnumerable<Game>> GamesLazyLoad(bool reload = false)
        {
            if (reload || games == null)
            {
                games = await Game.Get(Id);
            }
            return games;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
