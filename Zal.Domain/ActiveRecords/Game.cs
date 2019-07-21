using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge;
using Zal.Bridge.Gateways;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;
using Zal.Domain.Models;
using Zal.Domain.Tools;

namespace Zal.Domain.ActiveRecords
{
    public class Game : IActiveRecord
    {
        private GameRespondModel Model;

        public int Id => Model.Id;
        public string Name => Model.Name;
        public List<Score> Scores => Model.GetScores().Select(x => new Score(x)).ToList();

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

        public List<ScoreGroupedList> GetCategorizedScores(Dictionary<string, int[]> categories)
        {
            var list = new List<ScoreGroupedList>();
            var AllScores = Scores;
            var AllScoreIds = AllScores.Select(x => x.Id);
            foreach (var cat in categories)
            {
                var voidScoreIds = cat.Value.Where(id => !AllScoreIds.Contains(id));
                var scores = AllScores.Where(x => cat.Value.Contains(x.Id));
                scores = scores.Union(voidScoreIds.Select(x => new Score(this, x))).OrderBy(x=>x.NickName);
                list.Add(new ScoreGroupedList(scores, cat.Key));
            }
            return list;
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
        private ScoreModel Model;

        public int Id => Model.Id;
        public string Value => Model.Value;
        public bool HasValue => Value != null;
        public int IdUser => Model._Users_Id.Value;
        public string NickName { get; private set; }
        public string Variable { get; private set; }

        private static GameGateway gateway;
        private static GameGateway Gateway => gateway ?? (gateway = new GameGateway());

        public Score(ScoreModel model)
        {
            Model = model;
            TrySetNickName();

        }

        internal Score(Game game, int idUser)
        {
            Model = new ScoreModel
            {
                Id_Game = game.Id,
                Value = null,
                _Users_Id = idUser,
            };
            TrySetNickName();
        }

        private void TrySetNickName()
        {
            try
            {
                NickName = Zalesak.Users.GetAvailable(IdUser).NickName;
            }
            catch (Exception)
            {
                NickName = "Exception";
            }
        }

        private UnitOfWork<ScoreUpdateModel> unitOfWork;
        public UnitOfWork<ScoreUpdateModel> UnitOfWork => unitOfWork ?? (unitOfWork = new UnitOfWork<ScoreUpdateModel>(Model, OnUpdateCommited));

        private Task<bool> OnUpdateCommited()
        {
            if (Id != 0)
            {
                return Gateway.UpdateScoreAsync(Model, Zalesak.Session.Token);
            }
            else
            {
                return Gateway.AddScoreAsync(Model, Zalesak.Session.Token);
            }
        }
    }
}
