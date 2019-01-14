using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Bridge.Models
{
    public class ActionModel : IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date_start { get; set; }
        public DateTime Date_end { get; set; }
        public string EventType { get; set; }
        public int FromRank { get; set; }
        public bool IsOfficial { get; set; }
        public int? Id_Gallery { get; set; }
        public int? Id_Info { get; set; }
        public int? Id_Report { get; set; }

        private List<int> members;
        public List<int> Members {
            get {
                return members ?? (members = new List<int>());
            }
            set {
                members = value;
            }
        }

        private List<int> isJoining;
        public List<int> IsJoining {
            get {
                return isJoining ?? (isJoining = new List<int>());
            }
            set {
                isJoining = value;
            }
        }

        public IModel Copy() {
            return new ActionModel {
                Id = Id,
                Name = Name,
                Date_start = Date_start,
                Date_end = Date_end,
                EventType = EventType,
                FromRank = FromRank,
                Members = Members,
                IsJoining = IsJoining,
                Id_Gallery = Id_Gallery,
                Id_Info = Id_Info,
                Id_Report = Id_Report,
                IsOfficial = IsOfficial,
            };
        }
    }
}
