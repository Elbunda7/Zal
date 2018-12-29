using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Models.ApiModels;

namespace Zal.Bridge.Models
{
    public class UserModel:IModel
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Id_Rank { get; set; }
        public int Id_Group { get; set; }
        //public int Body { get; set; }
        //public bool Pres_facebook { get; set; }
        //public bool Zaplatil_prispevek { get; set; }
        //public string Role

        public UserModel(RegistrationRequestModel model) {
            Id = model.Id;
            Name = model.Name;
            Surname = model.Surname;
            Email = model.Email;
            Phone = model.Phone;
            NickName = $"{Name} {Surname.First()}.";
        }

        public UserModel() { }
    }
}
