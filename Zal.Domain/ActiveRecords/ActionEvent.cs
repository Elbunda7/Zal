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

namespace Zal.Domain.ActiveRecords
{
    public class ActionEvent : IActiveRecord//todo observable
    {
        private ActionModel Model;

        private List<UserJoiningAction> users;
        private Article report;
        private Article info;
        
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

        //public int Price { get { return Data.Price; } }
        //public decimal GPS_lon { get { return Data.GPS_lon; } }
        //public decimal GPS_lat { get { return Data.GPS_lat; } }

        private static ActionGateway gateway;
        private static ActionGateway Gateway => gateway ?? (gateway = new ActionGateway());

        public ActionEvent(IModel model) {
            Model = model as ActionModel;
        }

        private UnitOfWork<ActionUpdateModel> unitOfWork;
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
                var respond = await Gateway.GetUsersOnActionAsync(Id);
                users = respond.Select(x => new UserJoiningAction
                {
                    Member = new User(x.Member),
                    IsGarant = x.IsGarant,
                    Joining = (ZAL.Joining)x.Joining
                }).ToList();
            }
            return users;
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

        public Task<bool> Join(ZAL.Joining joining, bool asGarant = false) {
            //token uživatele
            UserPermision.HasRank(Zalesak.Session.CurrentUser, (ZAL.Rank)FromRank);
            return Join(Zalesak.Session.CurrentUser, joining, asGarant);
        }

        public async Task<bool> Join(User user, ZAL.Joining joining, bool asGarant = false) {
            UserPermision.Validate(Zalesak.Session.CurrentUser, user, ZAL.Rank.Vedouci);
            if ((await MembersLazyLoad()).Select(x => x.Member).Contains(user, ActiveRecordEqualityComparer.Instance))
            {
                var userOnAction = users.Single(x => x.Member.Id == user.Id);
                userOnAction.IsGarant = asGarant;
                userOnAction.Joining = joining;
            }
            else
            {
                users.Add(new UserJoiningAction
                {
                    IsGarant = asGarant,
                    Joining = joining,
                    Member = user,
                });
            }

            if (Model.Members.Contains(Zalesak.Session.CurrentUser.Id))
            {
                int index = Model.Members.IndexOf(Zalesak.Session.CurrentUser.Id);
                Model.IsJoining[index] = (int)joining;
            }
            else
            {
                Model.Members.Add(user.Id);
                Model.IsJoining.Add((int)joining);
            }

            var requestModel = new ActionUserJoinModel {
                Id_User = user.Id,
                Id_Action = Id,
                IsGarant = asGarant,
                IsJoining = (int)joining
            };
            return await Gateway.JoinAsync(requestModel);
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
