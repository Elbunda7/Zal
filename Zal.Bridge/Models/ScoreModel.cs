using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models
{
    public class ScoreModel : IModel
    {
        public int Id { get; set; }
        public int Id_Game { get; set; }
        public string Value { get; set; }
        public int? Order { get; set; }
        public int? _Users_Id { get; set; }
        public int? _Teams_Id { get; set; }
    }
}
