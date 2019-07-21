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
    public class Game : IActiveRecord
    {
        private GameRespondModel Model;

        public int Id => Model.Id;
        public string Name => Model.Name;
        public List<Score> Scores => Model.GetScores().Select(x => new Score(x)).ToList();

        private static GameGateway gateway;
        private static GameGateway Gateway => gateway ?? (gateway = new GameGateway());

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

        internal static async Task<IEnumerable<Game>> Get(int id_multipartGame)
        {
            var respond = await Gateway.GetGameAsync(id_multipartGame);
            return respond.Select(x => new Game(x));
        }
    }
}
