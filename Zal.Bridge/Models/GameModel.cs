using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models
{
    public class GameModel:IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Variables { get; set; }
        public bool FromBestToDown { get; set; } //todo rename
        public int Id_Multipart_Games { get; set; }
    }
}
