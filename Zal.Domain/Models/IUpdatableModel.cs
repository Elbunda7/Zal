using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Models;

namespace Zal.Domain.Models
{
    public interface IUpdatableModel
    {
        void CopyFrom(IModel apiModel);
        void CopyInto(IModel apiModel);
    }
}
