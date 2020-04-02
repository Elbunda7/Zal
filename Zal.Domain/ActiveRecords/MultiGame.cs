using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Gateways;
using Zal.Bridge.Models;
using Zal.Domain.Models;

namespace Zal.Domain.ActiveRecords
{
    public class MultiGame
    {
        private MultiGameModel Model;

        public int Id => Model.Id;
        public string Name => Model.Name;
        public int NumOfGames => Model.GamesCount;

        public bool HasMultipleParts => NumOfGames > 1;
        public bool HasOnePart => NumOfGames == 1;

        private List<Game> _games;

        private static GameGateway gateway;
        private static GameGateway Gateway => gateway ?? (gateway = new GameGateway());

        public MultiGame(MultiGameModel model)
        {
            Model = model;
        }

        internal Task<bool> AddGamesFirstTime(params GameBaseModel[] games)
        {
            _games = new List<Game>();
            return AddGames(games);
        }

        public async Task<bool> AddGames(params GameBaseModel[] games)
        {
            var models = new List<GameModel>();
            foreach (GameBaseModel game in games)
            {
                models.Add(new GameModel
                {
                    Name = game.Name,
                    Variables = game.Variable,
                    FromBestToDown = game.RatingStyle,
                    Id_Multipart_Games = Id,
                });
            }
            bool isSuccess = await Gateway.AddGames(models.ToArray(), "todo token");
            if (isSuccess)
            {
                foreach (GameModel model in models)
                {
                    _games.Add(new Game(model));
                }
                Model.GamesCount += models.Count;
            }
            return isSuccess;
        }

        public async Task<IEnumerable<Game>> GamesLazyLoad(bool reload = false)
        {
            if (reload || _games == null)
            {
                _games = (await Game.Get(Id)).ToList();
            }
            return _games;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
