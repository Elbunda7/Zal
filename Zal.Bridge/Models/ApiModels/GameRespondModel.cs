namespace Zal.Bridge.Models.ApiModels
{
    public class GameRespondModel:GameModel
    {
        public ScoreModel[] Scores { get; set; }

        public GameRespondModel(GameModel model)
        {
            Id = model.Id;
            Id_Multipart_Games = model.Id_Multipart_Games;
            Name = model.Name;
            Variable = model.Variable;
            RatingStyle = model.RatingStyle;
        }

        public GameRespondModel() { }
    }
}