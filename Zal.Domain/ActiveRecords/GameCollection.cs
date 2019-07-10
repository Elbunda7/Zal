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

        public int Id => Model.Id;
        public List<MultiGame> GameList => Model.MultiGames.Select(x => new MultiGame(x)).ToList();
        public int NumOfGames => Model.MultiGames.Count();
        public string Name => Model.Name;

        private static GameGateway gateway;
        private static GameGateway Gateway => gateway ?? (gateway = new GameGateway());

        public GameCollection(GameCollectionRespondModel model)
        {
            Model = model;
        }

        internal static async Task<IEnumerable<GameCollection>> GetAsync(int idAction)
        {
            var respond = await Gateway.GetCollectionAsync(idAction);
            return respond.Select(x => new GameCollection(x));
        }

    }

    public class MultiGame
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public MultiGame(MultiGameModel model)
        {
            Id = model.Id;
            Name = model.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
