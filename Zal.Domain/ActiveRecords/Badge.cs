
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;
using Zal.Domain.Consts;
using Zal.Domain.Models;
using Zal.Domain.Tools;

namespace Zal.Domain.ActiveRecords
{
    public class Badge : IActiveRecord, ISimpleItem
    {
        private BadgeModel model;

        public int Id => model.Id;
        public string Title => model.Name;
        public string Text => model.Text;
        public string Image => model.Image;

        private static BadgeGateway gateway;
        private static BadgeGateway Gateway => gateway ?? (gateway = new BadgeGateway());

        private UnitOfWork<BadgeUpdateModel> unitOfWork;
        public UnitOfWork<BadgeUpdateModel> UnitOfWork => unitOfWork ?? (unitOfWork = new UnitOfWork<BadgeUpdateModel>(model, OnUpdateCommited));

        private Task<bool> OnUpdateCommited() {
            return Gateway.UpdateAsync(model, Zalesak.Session.Token);
        }

        internal static async Task<BaseChangedActiveRecords<Badge>> IfNeededGetAllAsync(ZAL.Rank userRank, DateTime lastCheck) {
            var requestModel = new BadgeRequestModel {
                LastCheck = lastCheck,
                Rank = (int)userRank,
            };
            var rawRespond = await Gateway.IfNeededGetAllAsync(requestModel);
            var badges = rawRespond.Changed.Select(model => new Badge(model));
            return new BaseChangedActiveRecords<Badge>(rawRespond, badges);
        }

        private Badge(BadgeModel model) {
            this.model = model;
        }

        public static async Task<IEnumerable<Badge>> GetAcquiredAsync(User uzivatel) {
            IEnumerable<BadgeModel> rawModels = await Gateway.GetBadgesOwnedByUserAsync(uzivatel.Email, true);
            IEnumerable<Badge> badges = rawModels.Select(model => new Badge(model));
            return badges;
        }

        public static async Task<IEnumerable<Badge>> GetNotAcquired(User uzivatel) {
            IEnumerable<BadgeModel> rawModels = await Gateway.GetBadgesOwnedByUserAsync(uzivatel.Email, false);
            IEnumerable<Badge> badges = rawModels.Select(model => new Badge(model));
            return badges;
        }

        internal static async Task<Badge> Add(string name, string text, string image) {
            var badgeModel = new BadgeModel {
                Name = name,
                Text = text,
                Image = image
            };
            if(await Gateway.AddAsync(badgeModel, Zalesak.Session.Token)) {
                return new Badge(badgeModel);
            }
            return null;
        }

        public override string ToString() {
            return model.Name;
        }
    }
}
