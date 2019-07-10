using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models
{
    public class Games_ActionModel:IModel
    {
        public int Id { get; set; }
        public bool IsPointRated { get; set; }
        public int _Actions_Id { get; set; }
        public string Name { get; set; }
        public bool IsIndividuals { get; set; }
    }
}
