namespace Zal.Bridge.Models.ApiModels
{
    public class GameRespondModel:GameModel
    {
        public ScoreModel[] Scores { get; set; }

        public ScoreModel[] GetScores()
        {
            return Scores ?? (Scores = new ScoreModel[0]);
        }

        public GameRespondModel(GameModel model)
        {
            Id = model.Id;
            Id_Multipart_Games = model.Id_Multipart_Games;
            Name = model.Name;
            Variables = model.Variables;
            RatingStyle = model.RatingStyle;
        }

        public GameRespondModel() { }
    }
}