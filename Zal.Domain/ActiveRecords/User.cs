using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Zal.Bridge;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;
using Zal.Domain.Consts;
using Newtonsoft.Json;
using Zal.Domain.Tools;
using Zal.Domain.Models;
using Zal.Bridge.Tools;

namespace Zal.Domain.ActiveRecords
{
    public class User : IActiveRecord {

        private UserModel Model;

        private List<Badge> badges = new List<Badge>();

        public int Id => Model.Id;
        public string Email => Model.Email;
        public string NickName => Model.NickName;
        public string Name => Model.Name;
        public string Surname => Model.Surname;
        public bool IsBoy => Model.IsBoy;
        public string Phone => Model.Phone;
        public string Image => Model.Image?? "";
        public UserImageInfo ImageInfo => new UserImageInfo { ImageName = Model.Image, IsBoy = Model.IsBoy };
        public ZAL.Rank Rank => (ZAL.Rank)Model.Id_Rank;
        public ZAL.Group Group => (ZAL.Group)Model.Id_Group;
        public string GroupColor => Group.GetColorCode();
        public string RankAsString => ZAL.RANK_NAME[Model.Id_Rank];
        public string GroupAsString => ZAL.GROUP_NAME_SINGULAR[Model.Id_Group];
        public DateTime? DateOfBirth => Model.BirthDate;
        public int Age => Model.BirthDate.HasValue ? DateTime.Now.Year - Model.BirthDate.Value.Year : -1;
        //public string Role { get { return model.Role; } }
        //public int Points { get { return model.Body; } }//todo

        //public bool PaidForMembership { get { return model.Zaplatil_prispevek; } }
        //public Collection<Badge> Budges { get { return BudgesLazyLoad(); } private set { budgets = value; } }

        private static UserGateway gateway;
        private static UserGateway Gateway => gateway ?? (gateway = new UserGateway());

        private UnitOfWork<UserUpdateModel> unitOfWork;

        public UnitOfWork<UserUpdateModel> UnitOfWork => unitOfWork ?? (unitOfWork = new UnitOfWork<UserUpdateModel>(Model, OnUpdateCommited));

        private Task<bool> OnUpdateCommited() {
            return Gateway.UpdateAsync(Model, Zalesak.Session.Token);
        }

        internal static async Task<ChangedActiveRecords<User>> GetChanged(UserFilterModel filter, DateTime lastCheck, int count) {
            var requestModel = new UserChangesRequestModel {
                Groups = (int)filter.Groups,
                Ranks = (int)filter.Ranks,
                Roles = (int)filter.Roles,
                LastCheck = lastCheck,
                Count = count
            };
            var respond = await Gateway.GetAllChangedAsync(requestModel, Zalesak.Session.Token);
            var items = respond.Changed.Select(x => new User(x));
            return new ChangedActiveRecords<User>(respond, items);
        }

        public bool Match(UserFilterModel filter) {
            return filter.CanContains(Group, Rank, (ZAL.UserRole)7);//todo role
        }

        internal static async Task<User> AddNewUser(string name, string surname, int group, string nickname = null, string phone = null, string email = null, DateTime? birthDate = null) {
            UserModel model = new UserModel {
                NickName = nickname ?? $"{name} {surname[0]}.",
                Name = name,
                Surname = surname,
                Id_Group = group,
                BirthDate = birthDate,
                Email = email,
                Phone = phone,
            }; 
            if (await Gateway.AddAsync(model, Zalesak.Session.Token)) {
                return new User(model);
            }
            return null;
        }

        private async Task<IEnumerable<Badge>> BudgesLazyLoad() {
            if (badges == null) {
                badges = await Zalesak.Badges.GetAcquired(this) as List<Badge>;
            }
            return badges;
        }

        public User(UserModel model) {
            this.Model = model;
        }

        public static User Empty => new User(new UserModel());

        public static async Task<AllActiveRecords<User>> GetMore(UserFilterModel extendingFilter) {
            var requestModel = new UserRequestModel() {
                Groups = (int)extendingFilter.Groups,
                Ranks = (int)extendingFilter.Ranks,
                Roles = (int)extendingFilter.Roles,//todo not ready jet
            };
            var respond = await Gateway.GetMoreAsync(requestModel);
            return new AllActiveRecords<User>() {
                Timestamp = respond.Timestamp,
                ActiveRecords = respond.GetItems().Select(model => new User(model)),
            };
        }

        public static async Task<User> GetAsync(int id) {
            return new User(await Gateway.GetAsync(id));
        }

        internal static async Task<IEnumerable<User>> GetAsync(IEnumerable<int> ids) {
            IEnumerable<UserModel> rawModels = await Gateway.GetAsync(ids);
            IEnumerable<User> users = rawModels.Select(model => new User(model));
            return users;
        }

        public async Task<bool> AddBadge(Badge badge) {
            var model = new User_BadgeModel {
                Id_User = Id,
                Id_Badge = badge.Id,
            };
            bool wasAdded = await Gateway.AddBadgeAsync(model);
            if (wasAdded) badges.Add(badge);
            return wasAdded;
        }

        public async Task<bool> UploadProfileImage(byte[] rawImage)
        {
            var model = new ImageUploadModel
            {
                Id = Id,
            };
            Model.Image = await Gateway.UploadProfileImage(model, rawImage, Zalesak.Session.Token);
            return string.IsNullOrEmpty(Model.Image);
        }

        public void BecomeMember(DateTime dateOfBirthDay, int group, string prezdivka = null) {
            //if (model.Role == ZAL.MEMBERSHIP.NECLEN) {
                Model.BirthDate = dateOfBirthDay;
            if (prezdivka != null) Model.NickName = prezdivka;
                //model.Role = ZAL.MEMBERSHIP.CLEN;
                Model.Id_Group = group;
                Gateway.BecomeMember(Model);
            //}
            //else {
            //    throw new Exception("user already is a member");
            //}
        }

        /*rivate void ChangeIsPaid(bool value) {
            if (model.Zaplatil_prispevek != value) {
                model.Zaplatil_prispevek = value;
                Gateway.Update(model.Email, value);
            }
        }

        private void ChangePoints(int value) {
            throw new NotImplementedException();
        }

        private void ChangeEmail(string value) {
            throw new NotImplementedException();
        }

        private void ChangeRole(string value) {
            if (model.Role != value) {
                if (value == ZAL.MEMBERSHIP.CLEN) {
                    if (model.Datum_narozeni == DateTime.MinValue) {
                        return;
                    }
                }
                model.Role = value;
                IsChanged = true;
            }
        }

        private void ChangeGroup(int druzina) {
            if (druzina != model.Id_druzina) {
                if (druzina == ZAL.GROUP.LISKY) {
                    model.Id_druzina = druzina;
                    model.Hodnost = ZAL.RANK.LISKA;
                }
                else if (druzina == ZAL.GROUP.TROSKY) {
                    model.Id_druzina = druzina;
                    if (model.Hodnost < ZAL.RANK.VEDOUCI) {
                        model.Hodnost = ZAL.RANK.VEDOUCI;
                    }
                }
                else {
                    model.Id_druzina = druzina;
                    if (model.Hodnost < ZAL.RANK.NOVACEK || model.Hodnost > ZAL.RANK.RADCE) {
                        model.Hodnost = ZAL.RANK.NOVACEK;
                    }
                }
                IsChanged = true;
            }
        }

        private void ChangeRank(int hodnost) {
            if (hodnost != model.Hodnost) {
                if (hodnost == ZAL.RANK.LISKA) {
                    model.Id_druzina = ZAL.GROUP.LISKY;
                    model.Hodnost = hodnost;
                }
                else if (hodnost >= ZAL.RANK.VEDOUCI) {
                    model.Id_druzina = ZAL.GROUP.TROSKY;
                    model.Hodnost = hodnost;
                }
                else {
                    model.Hodnost = hodnost;
                    if (model.Id_druzina == ZAL.GROUP.LISKY || model.Id_druzina == ZAL.GROUP.TROSKY) {
                        model.Id_druzina = ZAL.GROUP.BOBRI;
                    }
                }
                IsChanged = true;
            }
        }

        private void SetShortName() {
            if (model.Prezdivka != null) {
                model.ShortName = model.Prezdivka;
            }
            else {
                if (model.Prijmeni.Length == 0) {
                    model.ShortName = model.Jmeno;
                }
                else {
                    model.ShortName = model.Jmeno + model.Prijmeni[0] + '.';
                }
            }
        }

        public bool ChangePassword(string userEmail, string oldPass, string newPass) {
            return Gateway.UpdatePassword(userEmail, oldPass, newPass);
        }*/

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString() {
            return NickName;
        }

        [Obsolete]
        public bool IsLeader() {
            return Rank >= ZAL.Rank.Vedouci;
        }

        internal static User LoadFrom(JToken jToken) {
            UserModel model = jToken.ToObject<UserModel>();
            return new User(model);
        }

        internal JToken GetJson() {
            return JToken.FromObject(Model);
        }
    }
}
