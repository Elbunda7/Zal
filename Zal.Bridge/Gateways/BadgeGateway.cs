using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;
using Zal.Bridge.Tools;

namespace Zal.Bridge
{
    public class BadgeGateway:Gateway
    {
        public BadgeGateway() : base(API.ENDPOINT.BADGES) { }

        public Task<BadgeModel> GetAsync(int id) {
            return SendRequestFor<BadgeModel>(API.METHOD.GET, id);
        }

        public async Task<BaseChangesRespondModel<BadgeModel>> IfNeededGetAllAsync(BadgeRequestModel model) {
            var respond = await SendRequestForNullable<BaseChangesRespondModel<BadgeModel>>(API.METHOD.GET_ALL, model);
            return respond ?? new BaseChangesRespondModel<BadgeModel>();
        }

        public async Task<Collection<BadgeModel>> GetBadgesOwnedByUserAsync(string userEmail, bool haveThem) {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(BadgeModel model, string userToken) {
            return SendRequestFor<bool>(API.METHOD.UPDATE, model, userToken);
        }

        public async Task<bool> AddAsync(BadgeModel model, string userToken) {
            model.Id = await SendRequestFor<int>(API.METHOD.ADD, model, userToken);
            return model.Id != -1;
        }

    }
}
