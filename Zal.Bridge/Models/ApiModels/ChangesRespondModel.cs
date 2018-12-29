using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Bridge.Models.ApiModels
{
    public class BaseChangesRespondFlags
    {
        public bool IsChanged { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class FullChangesRespondFlags : BaseChangesRespondFlags
    {
        public bool IsHardChanged { get; set; }

        private int[] deleted;
        public int[] Deleted {
            get {
                return deleted ?? (deleted = new int[0]);
            }
            set {
                deleted = value;
            }
        }
    }

    public class BaseChangesRespondModel<M> : BaseChangesRespondFlags where M : IModel
    {
        private IEnumerable<M> changed;
        public IEnumerable<M> Changed {
            get {
                return changed ?? (changed = new List<M>());
            }
            set {
                changed = value;
            }
        }
    }

    public class FullChangesRespondModel<M> : FullChangesRespondFlags where M : IModel
    {
        private IEnumerable<M> changed;
        public IEnumerable<M> Changed {
            get {
                return changed ?? (changed = new List<M>());
            }
            set {
                changed = value;
            }
        }
    }
}
