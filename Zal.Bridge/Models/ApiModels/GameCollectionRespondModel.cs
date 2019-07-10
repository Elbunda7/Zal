using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models.ApiModels
{
    public class GameCollectionRespondModel: Games_ActionModel
    {
        public MultiGameModel[] MultiGames { get; set; }
    }
}
