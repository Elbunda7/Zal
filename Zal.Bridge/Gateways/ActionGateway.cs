using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Zal.Bridge.Tools;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;

namespace Zal.Bridge
{
    public class ActionGateway:Gateway
    {
        public ActionGateway() : base(API.ENDPOINT.ACTIONS) { }

        public Task<ActionModel> GetAsync(int id) {
            return SendRequestFor<ActionModel>(API.METHOD.GET, id);
        }

        public Task<IEnumerable<ActionModel>> GetAllAsync() {
            return SendRequestFor<IEnumerable<ActionModel>>(API.METHOD.GET_ALL);
        }

        public Task<AllRespondModel<ActionModel>> GetPastByYearAsync(ActionRequestModel model, string token) {
            return SendRequestFor<AllRespondModel<ActionModel>>(API.METHOD.GET_PAST_BY_YEAR, model, token);
        }

        public async Task<bool> AddAsync(ActionModel model, string token) {
            model.Id = await SendRequestFor<int>(API.METHOD.ADD, model, token);
            return model.Id != -1;
        }

        public Task<bool> JoinAsync(ActionUserJoinModel model) {
            return SendRequestFor<bool>(API.METHOD.JOIN, model);
        }

        public Task<bool> UnJoinAsync(ActionUserModel model) {
            return SendRequestFor<bool>(API.METHOD.UNJOIN, model);
        }

        public Task<bool> DeleteAsync(int idAction, string token) {
            return SendRequestFor<bool>(API.METHOD.DELETE, idAction, token);
        }

        public Task<IEnumerable<MembersOnActionModel>> GetUsersOnActionAsync(int id) {
            return SendRequestFor<IEnumerable<MembersOnActionModel>>(API.METHOD.GET_USERS_ON_ACTION, id);
        }

        public Task<bool> UpdateAsync(ActionModel model, string token) {
            return SendRequestFor<bool>(API.METHOD.UPDATE, model, token);
        }

        public async Task<ActionModel> GetChangedAsync(int id, DateTime lastCheck) {
            throw new NotImplementedException();
        }

        public async Task<FullChangesRespondModel<ActionModel>> GetAllChangedAsync(ActionChangesRequestModel model, string token) {
            var respond = await SendRequestForNullable<FullChangesRespondModel<ActionModel>>(API.METHOD.GET_CHANGED, model, token);
            return respond ?? new FullChangesRespondModel<ActionModel>();
        }
    }
}
