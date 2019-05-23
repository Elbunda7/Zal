using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Consts;
using Zal.Domain.Models;
using Zal.Domain.Tools;
using Zal.Domain.Tools.ARSets;

namespace Zal.Domain.ItemSets
{
    public class UserSet : BaseSet
    {
        public UserObservableSortedSet Users { get; set; }
        private HashSet<User> AllUsers;
        private UserFilterModel currentFilter;
        private UserFilterModel cumulativeFilter;

        public UserSet() {
            Users = new UserObservableSortedSet();
            AllUsers = new HashSet<User>(ActiveRecordEqualityComparer.Instance);
            currentFilter = new UserFilterModel();
            cumulativeFilter = UserFilterModel.Default;
        }

        public async Task ReSynchronize(UserFilterModel filter = null)//todo empty params?
        {
            currentFilter.Clear();
            cumulativeFilter = UserFilterModel.Default;
            AllUsers.Clear();
            Users.Clear();
            await Synchronize(filter);
        }

        public async Task Synchronize(UserFilterModel filter = null)
        {
            filter = filter ?? UserFilterModel.Default;
            if (filter != currentFilter)
            {
                Users.RemoveAll();
                Users.AddAll(AllUsers.Where(x => x.Match(filter)));
                currentFilter = filter;
            }
            await LoadChangedUsers(cumulativeFilter);
            if (filter.IsExtending(cumulativeFilter))
            {
                var extendingFilter = cumulativeFilter.GetOnlyExtendigFilterFrom(filter);
                await LoadMoreUsers(extendingFilter);
                cumulativeFilter.CombineWith(filter);
            }
        }

        private async Task LoadMoreUsers(UserFilterModel filter) {
            var task = User.GetMore(filter);
            var respond = await ExecuteTask(task);
            Users.AddAll(respond.ActiveRecords);
            AllUsers.UnionWith(respond.ActiveRecords);
        }

        private async Task LoadChangedUsers(UserFilterModel filter) {
            var task = User.GetChanged(filter, Users.LastSynchronization, AllUsers.Count());
            var respond = await ExecuteTask(task);
            if (respond.IsHardChanged)
            {
                Users.Clear();
                Users.AddAll(respond.Changed);
                AllUsers.Clear();
                AllUsers.UnionWith(respond.Changed);
                Users.LastSynchronization = respond.Timestamp;
            }
            else if (respond.IsChanged)
            {
                Users.RemoveByIds(respond.Deleted);
                Users.AddOrUpdateAll(respond.Changed);
                AllUsers.RemoveWhere(x => respond.Deleted.Contains(x.Id));
                AllUsers.UnionWith(respond.Changed);
                Users.LastSynchronization = respond.Timestamp;
            }
        }

        

        //internal Collection<User> GetByEmailList(List<string> list) {
        //    Collection<User> users = new Collection<User>();
        //    foreach(string email in list) {
        //        users.Add(GetByEmail(email));
        //    }
        //    return users;
        //}

        public async Task AddNewUser(string name, string surname, ZAL.Group group, string nickname = null, string phone = null, string email = null, DateTime? birthDate = null) {
            //UserPermision.HasRank(Zal.Session.CurrentUser, ZAL.Rank.Vedouci);
            var task = User.AddNewUser(name, surname, group, nickname, phone, email, birthDate);
            Users.Add(await ExecuteTask(task));
        }

        public void SetSelections(List<int> ids)
        {
            var selected = AllUsers.Where(x => ids.Contains(x.Id));
            foreach (User user in AllUsers) user.IsSelected = false;
            foreach (User user in selected) user.IsSelected = true;
        }

        public List<int> GetSelections()
        {
            return AllUsers.Where(x => x.IsSelected).Select(x => x.Id).ToList();
        }


        //public User GetByEmail(string email) {
        //    foreach (User u in Users) {
        //        if (u.Has(CONST.USER.EMAIL, email)) {
        //            return u;
        //        }
        //    }
        //    User user = User.Select(email);
        //    if (user.Email != null) {
        //        Users.Add(user);
        //        return user;
        //    }
        //    return User.Empty();
        //}

        internal async Task<User> Get(int id)
        {
            User a;
            if (AllUsers.Any(user => user.Id == id))
            {
                a = AllUsers.Single(user => user.Id == id);
            }
            else
            {
                var task = User.GetAsync(id);
                a = await ExecuteTask(task);
                AllUsers.Add(a);
            }
            return a;
        }

        internal async Task<IEnumerable<User>> Get(List<int> ids) {
            IEnumerable<User> users = Users.Where(user => ids.Any(id => id == user.Id));
            var notLoadedIds = ids.Where(id => users.All(user => user.Id != id));
            var task = User.GetAsync(notLoadedIds);
            users.Union(await ExecuteTask(task));
            return users;
        }

        internal void RemoveLocal(int id) => AllUsers.RemoveWhere(x => x.Id == id);

        internal void AddLocal(User user) => AllUsers.Add(user);

        internal JToken GetJson()
        {
            JArray jArray = new JArray();
            foreach (User user in AllUsers.Where(x => x.Match(UserFilterModel.Default)))
            {
                jArray.Add(user.GetJson());
            }
            JToken jToken = new JObject{
                {"timestamp", Users.LastSynchronization },
                {"items", jArray }
            };
            return jToken;
        }

        internal void LoadFrom(JToken json)
        {
            var users = json.Value<JArray>("items").Select(x => User.LoadFrom(x));
            AllUsers.UnionWith(users);
            Users.AddAll(users);
            Users.LastSynchronization = json.Value<DateTime>("timestamp");
            cumulativeFilter = currentFilter = UserFilterModel.Default;
        }
    }
}
