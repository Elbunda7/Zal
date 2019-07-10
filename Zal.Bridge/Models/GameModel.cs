using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models
{
    public class GameModel:IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Variable { get; set; }
        public int RatingStyle { get; set; }
        public int Id_Multipart_Games { get; set; }
    }
}
