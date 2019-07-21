using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models.ApiModels
{
    public class GameCollectionRespondModel: Games_ActionModel
    {
        public MultiGameModel[] MultiGames { get; set; }
        public GameCategoryModel[] Categories { get; set; }

        public MultiGameModel[] GetMultiGames()
        {
            return MultiGames ?? (MultiGames = new MultiGameModel[0]);
        }

        public GameCategoryModel[] GetCategories()
        {
            return Categories ?? (Categories = new GameCategoryModel[0]);
        }

        public GameCollectionRespondModel(Games_ActionModel model)
        {
            Id = model.Id;
            Name = model.Name;
            _Actions_Id = model._Actions_Id;
            IsIndividuals = model.IsIndividuals;
            IsPointRated = model.IsPointRated;
        }

        public GameCollectionRespondModel() { }

    }
}
