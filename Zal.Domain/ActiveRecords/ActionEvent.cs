using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Zal.Domain.Consts;
using System.Xml.Linq;
using Zal.Domain.Tools;
using Zal.Bridge.Models;
using Zal.Bridge;
using System.Threading.Tasks;
using Zal.Bridge.Models.ApiModels;
using Newtonsoft.Json.Linq;
using Zal.Domain.Models;
using Zal.Domain.Tools.ARSets;
using System.ComponentModel;

namespace Zal.Domain.ActiveRecords
{
    public class ActionEvent : IActiveRecord, INotifyPropertyChanged
    {
        private ActionModel Model;

        private List<UserJoiningAction> users;
        private Article report;
        private Article info;
        private List<GameCollection> games;
        
        public int Id => Model.Id;
        public string Type => Model.EventType;
        public string Name => Model.Name;
        public int FromRank => Model.FromRank;//todo (ZAL.Rank)
        public DateTime DateFrom => Model.Date_start;
        public DateTime DateTo => Model.Date_end;
        public int MembersCount => Model.IsJoining.Where(x => x == (int)ZAL.Joining.True).Count();
        public bool IsOfficial => Model.IsOfficial;//todo přejmenovat nebo přidat IsPublished
        public bool HasInfo => Model.Id_Info.HasValue;
        public bool HasReport => Model.Id_Report.HasValue;
        public bool HasGallery => Model.Id_Gallery.HasValue;
        public int Days {
            get {
                TimeSpan ts = Model.Date_end - Model.Date_start;
                return (int)ts.TotalDays;
            }
        }
        public ZAL.Joining DoIParticipate {//todo jak aktualizovat?
            get {
                if (Model.Members.Contains(Zalesak.Session.CurrentUser.Id))
                {
                    int index = Model.Members.IndexOf(Zalesak.Session.CurrentUser.Id);
                    return (ZAL.Joining)Model.IsJoining[index];
                }
                return ZAL.Joining.Unknow;
            }
        }

        [Obsolete]
        public async Task<IEnumerable<User>> Members(ZAL.Joining isJoining = ZAL.Joining.True)
        {
            var members = new List<User>();
            for (int i = 0; i < Model.Members.Count; i++)
            {
                if (Model.IsJoining[i] == (int)isJoining)
                {
                    int id = Model.Members[i];
                    members.Add(await Zalesak.Users.Get(id));
                }
            }
            return members;
        }

        public List<int> RawMembers(ZAL.Joining isJoining = ZAL.Joining.True)
        {
            var members = new List<int>();
            for (int i = 0; i < Model.Members.Count; i++)
            {
                if (Model.IsJoining[i] == (int)isJoining)
                {
                    int id = Model.Members[i];
                    members.Add(id);
                }
            }
            return members;
        }

        //public int Price { get { return Data.Price; } }
        //public decimal GPS_lon { get { return Data.GPS_lon; } }
        //public decimal GPS_lat { get { return Data.GPS_lat; } }

        private static ActionGateway gateway;
        private static ActionGateway Gateway => gateway ?? (gateway = new ActionGateway());

        public ActionEvent(IModel model) {
            Model = model as ActionModel;
        }

        private UnitOfWork<ActionUpdateModel> unitOfWork;

        public event PropertyChangedEventHandler PropertyChanged;

        public UnitOfWork<ActionUpdateModel> UnitOfWork => unitOfWork ?? (unitOfWork = new UnitOfWork<ActionUpdateModel>(Model, OnUpdateCommited));

        private Task<bool> OnUpdateCommited() {
            return Gateway.UpdateAsync(Model, Zalesak.Session.Token);
        }

        public async Task<Article> InfoLazyLoad() {
            if (HasInfo && info == null) {
                info = await Zalesak.Actualities.GetArticleAsync(Model.Id_Info.Value);
            }
            return info;
        }

        public async Task<Article> ReportLazyLoad() {
            if (HasReport && report == null) {
                report = await Zalesak.Actualities.GetArticleAsync(Model.Id_Report.Value);
            }
            return report;
        }

        public async Task<IEnumerable<UserJoiningAction>> MembersLazyLoad(bool reload = false) {
            if (reload || users == null)
            {
                users = await Zalesak.Users.GetUsersOnActionAsync(Model.Id);
            }
            return users;
        }

        public async Task<IEnumerable<GameCollection>> GamesLazyLoad(bool reload = false)
        {
            if (reload || games == null)
            {
                games = (await GameCollection.GetAsync(Id)).ToList();
            }
            return games;
        }

        public static async Task<ActionEvent> AddAsync(string name, string type, DateTime start, DateTime end, int fromRank, bool isOfficial = true) {
            ActionModel model = new ActionModel {
                Id = -1,
                Name = name,
                EventType = type,
                Date_start = start,
                Date_end = end,
                FromRank = fromRank,
                IsOfficial = isOfficial,
            };
            if (await Gateway.AddAsync(model, Zalesak.Session.Token)) {
                return new ActionEvent(model);
            }
            return null;
        }

        public Task<bool> DeleteAsync() {
            return Gateway.DeleteAsync(Model.Id, Zalesak.Session.Token);
        }

        public async Task<bool> AddNewInfoAsync(string title, string text) {
            return await AddNewInfoAsync(Zalesak.Session.CurrentUser, title, text);
        }

        public async Task<bool> AddNewInfoAsync(User author, string title, string text) {
            info = await Zalesak.Actualities.CreateNewArticle(author, title, text, FromRank, ZAL.ArticleType.Info, Id);
            bool wasAdded = info != null;
            if (wasAdded) {
                Model.Id_Info = info.Id;
            }
            return wasAdded;
        }

        public async Task<bool> AddNewReportAsync(string title, string text) {
            //přepsat stávající?
            return await AddNewReportAsync(Zalesak.Session.CurrentUser, title, text);
        }

        public async Task<bool> AddNewReportAsync(User author, string title, string text) {
            report = await Zalesak.Actualities.CreateNewArticle(author, title, text, FromRank, ZAL.ArticleType.Record, Id);
            bool wasAdded = report != null;
            if (wasAdded) {
                Model.Id_Report = report.Id;
            }
            return wasAdded;
        }

        public async Task<GameCollection> AddNewGameCollection(string name, bool isPointRated, bool isIndividuals)
        {
            var respondItem = await GameCollection.Add(Id, name, isPointRated, isIndividuals);
            bool wasAdded = respondItem != null;
            if (wasAdded)
            {
                games.Add(respondItem);
            }
            return respondItem;
        }

        public async Task<bool> Join(IEnumerable<int> selected, IEnumerable<int> unselected)
        {
            //UserPermision.Validate(Zalesak.Session.CurrentUser, user, ZAL.Rank.Vedouci);
            await MembersLazyLoad();
            var requestModel = new List<ActionUserJoinModel>();
            foreach (int id in selected)
            {
                requestModel.Add(new ActionUserJoinModel
                {
                    Id_User = id,
                    Id_Action = Model.Id,
                    IsGarant = false,
                    IsJoining = (int)ZAL.Joining.True,
                });
            }
            foreach (int id in unselected)
            {
                requestModel.Add(new ActionUserJoinModel
                {
                    Id_User = id,
                    Id_Action = Model.Id,
                    IsGarant = false,
                    IsJoining = (int)ZAL.Joining.False,
                });
            }
            bool respond = await Gateway.JoinManyAsync(requestModel);

            if (respond)
            {
                foreach (int id in selected)
                {
                    int index = Model.Members.IndexOf(id);
                    if (index != -1)
                    {
                        Model.IsJoining[index] = (int)ZAL.Joining.True;
                        users[index].Joining = ZAL.Joining.True;
                    }
                    else
                    {
                        Model.Members.Add(id);
                        Model.IsJoining.Add((int)ZAL.Joining.True);
                        users.Add(new UserJoiningAction
                        {
                            IsGarant = false,
                            Joining = ZAL.Joining.True,
                            Member = await Zalesak.Users.Get(id),
                        });
                    }
                }
                foreach (int id in unselected)
                {
                    int index = Model.Members.IndexOf(id);
                    Model.IsJoining[index] = (int)ZAL.Joining.False;
                    users[index].Joining = ZAL.Joining.False;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MembersCount"));
            }
            return respond;
        }

        public Task<bool> Join(ZAL.Joining joining, bool asGarant = false) {
            //token uživatele
            UserPermision.HasRank(Zalesak.Session.CurrentUser, (ZAL.Rank)FromRank);
            return Join(Zalesak.Session.CurrentUser, joining, asGarant);
        }

        public async Task<bool> Join(User user, ZAL.Joining joining, bool asGarant = false) {
            UserPermision.Validate(Zalesak.Session.CurrentUser, user, ZAL.Rank.Vedouci);          
            var requestModel = new ActionUserJoinModel {
                Id_User = user.Id,
                Id_Action = Id,
                IsGarant = asGarant,
                IsJoining = (int)joining
            };
            await MembersLazyLoad();
            bool respond = await Gateway.JoinAsync(requestModel);
            if (respond)
            {
                if (Model.Members.Contains(Zalesak.Session.CurrentUser.Id))
                {
                    int index = Model.Members.IndexOf(Zalesak.Session.CurrentUser.Id);
                    Model.IsJoining[index] = (int)joining;
                    users[index].Joining = joining;
                    users[index].IsGarant = asGarant;
                }
                else
                {
                    Model.Members.Add(user.Id);
                    Model.IsJoining.Add((int)joining);
                    users.Add(new UserJoiningAction
                    {
                        IsGarant = asGarant,
                        Joining = joining,
                        Member = user,
                    });
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MembersCount"));
            }
            return respond;
        }


        internal static async Task<ChangedActiveRecords<ActionEvent>> GetChangedAsync(int userRank, DateTime lastCheck, int currentYear, int count) {
            var requestModel = new ActionChangesRequestModel {
                Rank = userRank,
                LastCheck = lastCheck,
                Year = currentYear,
                Count = count
            };
            var rawChanges = await Gateway.GetAllChangedAsync(requestModel, Zalesak.Session.Token);
            var items = rawChanges.Changed.Select(x => new ActionEvent(x));
            return new ChangedActiveRecords<ActionEvent>(rawChanges, items);
        }

        public override string ToString() {
            return Model.Name;
        }

        [Obsolete]
        public async void Synchronize(DateTime lastCheck) {//?
            Model = await Gateway.GetChangedAsync(Id, lastCheck);
        }

        internal async static Task<AllActiveRecords<ActionEvent>> GetActionsByYear(int userRank, int year) {//vrátit respond model se serverovým časem 
            var requestModel = new ActionRequestModel {
                Rank = userRank,
                Year = year
            };
            var respond = await Gateway.GetPastByYearAsync(requestModel, Zalesak.Session.Token);
            return new AllActiveRecords<ActionEvent>() {
                Timestamp = respond.Timestamp,
                ActiveRecords = respond.GetItems().Select(model => new ActionEvent(model)),
            };
        }

        public async static Task<ActionEvent> Get(int id) {
            return new ActionEvent(await Gateway.GetAsync(id));
        }

        internal JToken GetJson() {
            return JObject.FromObject(Model);
        }

        internal static ActionEvent LoadFrom(JToken json) {
            var model = json.ToObject<ActionModel>();
            return new ActionEvent(model);
        }
    }
}
