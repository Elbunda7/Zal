using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zal.Domain.ActiveRecords;
using Zal.Domain.Consts;
using Zal.Domain.Models;
using Zal.Domain.Tools;
using Zal.Domain.Tools.ARSets;
using static Zal.Domain.Consts.ZAL;

namespace Zal.Domain.ItemSets
{
    public class UserSet : BaseSet
    {
        public UserObservableSortedSet Users { get; set; }
        private UserFilterModel filterInMemory;

        public UserSet() {
            Users = new UserObservableSortedSet();
            filterInMemory = new UserFilterModel();
        }

        public async Task ReSynchronize(UserFilterModel filter = null) {
            filterInMemory.Clear();
            Users.LastSynchronization = DATE_OF_ORIGIN;
            await SynchronizeUsers(filter);
        }

        public async Task SynchronizeUsers(UserFilterModel filter = null) {//todo kontrola korektnosti při změně filtru
            filter = filter ?? UserFilterModel.Default;
            if (Users.LastSynchronization == DATE_OF_ORIGIN) {
                await LoadUsers(filter, isAndMode: true);
            }
            else {
                await LoadUserChanges(filterInMemory);
                if (filterInMemory.WillBeExtendedWith(filter)) {
                    UserFilterModel extendingFilter = filterInMemory.GetOnlyExtendigFilterFrom(filter);//todo filtermode
                    await LoadUsers(extendingFilter, isAndMode: false);
                }
            }
            filterInMemory.CombineWith(filter);
        }

        private async Task LoadUsers(UserFilterModel filter, bool isAndMode) {
            var task = User.GetAll(filter, isAndMode);
            var respond = await ExecuteTask(task);
            Users.AddOrUpdateAll(respond.ActiveRecords);
            Users.LastSynchronization = respond.Timestamp;
        }

        private async Task LoadUserChanges(UserFilterModel filter) {
            var task = User.GetChanged(filter, Users.LastSynchronization, Users.Where(x=>x.Meets(filter)).Count());
            var respond = await ExecuteTask(task);
            if (respond.IsHardChanged) {
                Users.Clear();
                Users.AddAll(respond.Changed);
                Users.LastSynchronization = respond.Timestamp;
            }
            else if (respond.IsChanged) {
                Users.RemoveByIds(respond.Deleted);
                Users.AddOrUpdateAll(respond.Changed);
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

        public async Task AddNewUser(string name, string surname, int group, string nickname = null, string phone = null, string email = null, DateTime? birthDate = null) {
            //UserPermision.HasRank(Zal.Session.CurrentUser, ZAL.Rank.Vedouci);
            var task = User.AddNewUser(name, surname, group, nickname, phone, email, birthDate);
            Users.Add(await ExecuteTask(task));
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

        internal async Task<User> Get(int id) {
            //return User.Select(email);

            User a = Users.Single(user => user.Id == id);
            if (a == null) {
                var task = User.GetAsync(id);
                a = await ExecuteTask(task);
                Users.Add(a);
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
    }
}
