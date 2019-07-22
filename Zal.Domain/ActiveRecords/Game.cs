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

        private List<ScoreGroupedList> _scores;

        public int Id => Model.Id;
        public string Name => Model.Name;

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

        private void Score_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var tmpScore = sender as Score;
            if (e.PropertyName == "Id")
            {
                var tmpScores = Model.Scores.ToList();
                tmpScores.Add(tmpScore.Model);
                Model.Scores = tmpScores.ToArray();
            }
            else if (e.PropertyName == "Value")
            {
                var scoreModel = Model.Scores.Single(x => x.Id == tmpScore.Id);
                scoreModel.Value = tmpScore.Value;
            }
        }

        public List<ScoreGroupedList> GetCategorizedScores(Dictionary<string, int[]> categories)
        {
            if (_scores == null)
            {
                _scores = new List<ScoreGroupedList>();
                var AllScores = Model.GetScores().Select(x => new Score(x)).ToList();
                var AllScoreIds = AllScores.Select(x => x.IdUser);
                foreach (var cat in categories)
                {
                    var voidScoreIds = cat.Value.Where(id => !AllScoreIds.Contains(id));
                    var scores = AllScores.Where(x => cat.Value.Contains(x.IdUser));
                    scores = scores.Union(voidScoreIds.Select(x => new Score(this, x))).OrderBy(x => x.NickName);
                    foreach (var item in scores)
                    {
                        item.PropertyChanged += Score_PropertyChanged;
                    }
                    _scores.Add(new ScoreGroupedList(scores, cat.Key));
                }
            }
            return _scores;
        }

        internal static async Task<IEnumerable<Game>> Get(int id_multipartGame)
        {
            var respond = await Gateway.GetGameAsync(id_multipartGame);
            return respond.Select(x => new Game(x));
        }
    }
}
