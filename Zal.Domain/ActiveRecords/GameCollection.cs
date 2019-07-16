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

        public async Task<MultiGame> AddMultiGame(string name, params GameBaseModel[] games)
        {
            MultiGame multiGame = await AddMultiGame(name);
            await multiGame.AddGames(games);
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
                var tmp = Model.MultiGames.ToList();
                tmp.Add(model);
                Model.MultiGames = tmp.ToArray();
                multiGame = new MultiGame(model);
                GameList.Add(multiGame);
            }
            return multiGame;
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

        internal static Task<bool> AddGames(List<GameModel> models)
        {
            return Gateway.AddGames(models.ToArray(), "todo token");
        }
    }

    public class MultiGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumOfGames { get; set; }

        public bool HasMultipleParts => NumOfGames > 1;
        public bool HasOnePart => NumOfGames == 1;

        private IEnumerable<Game> _games;

        public MultiGame(MultiGameModel model)
        {
            Id = model.Id;
            Name = model.Name;
            NumOfGames = model.GamesCount;
        }

        public async Task<bool> AddGames(params GameBaseModel[] games)
        {
            var models = new List<GameModel>();
            foreach (GameBaseModel game in games)
            {
                models.Add(new GameModel
                {
                    Name = game.Name,
                    Variable = game.Variable,
                    RatingStyle = game.RatingStyle,
                    Id_Multipart_Games = Id,
                });
            }
            bool isSuccess = await GameCollection.AddGames(models);
            if (isSuccess)
            {
               _games = _games.Union(models.Select(x => new Game(x)));
                NumOfGames += models.Count;
            }
            return isSuccess;
        }

        public async Task<IEnumerable<Game>> GamesLazyLoad(bool reload = false)
        {
            if (reload || _games == null)
            {
                _games = await Game.Get(Id);
            }
            return _games;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
