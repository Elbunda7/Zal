using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge;
using Zal.Bridge.Gateways;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;

namespace Zal.Domain.ActiveRecords
{
    public class Game : IActiveRecord
    {
        private GameRespondModel Model;

        public int Id => Model.Id;
        public string Name => Model.Name;
        public List<Score> Scores => Model.Scores.Select(x => new Score(x)).ToList();

        public Game()
        {
            test();
        }

        public Game(GameRespondModel model)
        {
            Model = model;
        }

        public Game(GameModel model)
        {
            Model = new GameRespondModel(model);
        }

        public async void test()
        {
            var a = await Gateway.GetCollectionAsync(122);
            int id = a.First().Id;
            var b = await Gateway.GetGameAsync(id);
        }

        private static GameGateway gateway;
        private static GameGateway Gateway => gateway ?? (gateway = new GameGateway());

        internal static async Task<IEnumerable<Game>> Get(int id_multipartGame)
        {
            var respond = await Gateway.GetGameAsync(id_multipartGame);
            return respond.Select(x => new Game(x));
        }
    }

    public class Score
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string UserName { get; set; }

        public Score(ScoreModel model)
        {
            Id = model.Id;
            Value = model.Value;
            try
            {
                UserName = Zalesak.Users.Users.Single(x => x.Id == model._Users_Id).NickName;
            }
            catch (Exception)
            {
                UserName = "Exception";
            }
           
        }
    }
}
