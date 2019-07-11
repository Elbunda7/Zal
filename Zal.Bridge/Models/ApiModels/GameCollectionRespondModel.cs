using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models.ApiModels
{
    public class GameCollectionRespondModel: Games_ActionModel
    {
        public MultiGameModel[] MultiGames { get; set; }

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
