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
    public class ActionEvent : IActiveRecord
    {
        private ActionModel Model;

        private IEnumerable<User> garants;
        private IEnumerable<User> members;
        private Article report;
        private Article info;
        
        public int Id => Model.Id;
        public string Type => Model.EventType;
        public string Name => Model.Name;
        public int FromRank => Model.FromRank;
        public DateTime DateFrom => Model.Date_start;
        public DateTime DateTo => Model.Date_end;
        public int MembersCount => Model.Members.Length;
        public bool DoIParticipate => Model.Members.Contains(Zalesak.Session.CurrentUser.Id);//todo jak aktualizovat?
        public bool IsOfficial => Model.IsOfficial;//todo přejmenovat nebo přidat IsPublished
        public bool HasInfo => Model.Id_Info.HasValue;
        public bool HasReport => Model.Id_Report.HasValue;
        public int Days {
            get {
                TimeSpan ts = Model.Date_end - Model.Date_start;
                return (int)ts.TotalDays;
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

        public async Task<IEnumerable<User>> MembersLazyLoad(ZAL.ActionUserRole role, bool reload = false) {
            await MembersLazyLoad(reload);
            switch (role) {
                case ZAL.ActionUserRole.Garant: return garants;
                case ZAL.ActionUserRole.Member: return members;
                default: return garants.Union(members);
            }
        }

        private async Task MembersLazyLoad(bool reload = false) {
            if (reload || (garants == null && members == null)) {
                var respond = await Gateway.GetUsersOnActionAsync(Id);
                garants = respond.Where(x => x.IsGarant).Select(x => new User(x.Member)).ToList();
                members = respond.Where(x => !x.IsGarant).Select(x => new User(x.Member)).ToList();
            }
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

        public Task<bool> Join(bool asGarant = false) {
            //token uživatele
            return Join(Zalesak.Session.CurrentUser, asGarant);
        }

        public async Task<bool> Join(User user, bool asGarant = false) {
            await MembersLazyLoad();
            UpdateLocalMember(user, asGarant);
            var requestModel = new ActionUserJoinModel {
                Id_User = user.Id,
                Id_Action = Id,
                IsGarant = asGarant,
            };
            return await Gateway.JoinAsync(requestModel);
        }

        public Task<bool> UnJoin() {
            return UnJoin(Zalesak.Session.CurrentUser);
        }

        public async Task<bool> UnJoin(User user) {
            await MembersLazyLoad();
            RemoveLocalMember(user);
            var requestModel = new ActionUserModel {
                Id_User = user.Id,
                Id_Action = Id,
            };
            return await Gateway.UnJoinAsync(requestModel);
        }

        private void UpdateLocalMember(User user, bool asGarant) {
            RemoveLocalMember(user);
            if (asGarant) {
                (garants as List<User>).Add(user);
            }
            else {
                (members as List<User>).Add(user);
            }
        }

        private void RemoveLocalMember(User user) {
            if (garants.Contains(user, ActiveRecordEqualityComparer.Instance)) {
                (garants as List<User>).Remove(garants.Single(x => x.Id == user.Id));
            }
            else if (members.Contains(user, ActiveRecordEqualityComparer.Instance)) {
                (members as List<User>).Remove(members.Single(x => x.Id == user.Id));
            }
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
