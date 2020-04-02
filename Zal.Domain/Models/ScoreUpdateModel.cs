using System;
using System.Collections.Generic;
using System.Text;
using Zal.Bridge.Models;

namespace Zal.Domain.Models
{
    public class ScoreUpdateModel : IUpdatableModel
    {
        public double Value { get; set; }

        public void CopyInto(IModel apiModel)
        {
            ScoreModel model = apiModel as ScoreModel;
            model.Value = Value;
        }

        public void CopyFrom(IModel apiModel)
        {
            ScoreModel model = apiModel as ScoreModel;
            Value = model.Value ?? 0;
        }
    }
}
