using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models
{
    public class MultiGameModel:IModel
    {
        public int Id { get; set; }
        public int Id_Games_on_Action { get; set; }
        public string Name { get; set; }
        public int GamesCount { get; set; }
    }
}
