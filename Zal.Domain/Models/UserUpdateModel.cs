using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Models;
using Zal.Domain.Consts;

namespace Zal.Domain.Models
{
    public class UserUpdateModel :IUpdatableModel
    {
        public string NickName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public ZAL.Rank Id_Rank { get; set; }
        public ZAL.Group Id_Group { get; set; }
        public bool IsBoy { get; set; }
        //public bool Zaplatil_prispevek { get; set; }
        //public string Role { get; set; }

        public void CopyFrom(IModel apiModel) {
            UserModel model = apiModel as UserModel;
            NickName = model.NickName;
            Name = model.Name;
            Surname = model.Surname;
            Phone = model.Phone;
            BirthDate = model.BirthDate;
            Id_Rank = (ZAL.Rank)model.Id_Rank;
            Id_Group = (ZAL.Group)model.Id_Group;
            IsBoy = model.IsBoy;
        }

        public void CopyInto(IModel apiModel) {
            UserModel model = apiModel as UserModel;
            model.NickName = NickName;
            model.Name = Name;
            model.Surname = Surname;
            model.Phone = Phone;
            model.BirthDate = BirthDate;
            model.Id_Rank = (int)Id_Rank;
            model.Id_Group = (int)Id_Group;
            model.IsBoy = IsBoy;
        }
    }
}
