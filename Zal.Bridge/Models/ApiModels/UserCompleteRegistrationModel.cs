using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models.ApiModels
{
    public class UserCompleteRegistrationModel
    {
        public string Phone { get; set; }
        public bool IsBoy { get; set; }
        public DateTime BirthDate { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string NickName { get; set; }
        public int Id { get; set; }
    }
}
